using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeTrack.Server.Data;
using TreeTrack.Server.DTOs;
using TreeTrack.Server.Helpers;
using TreeTrack.Server.Models;
using TreeTrack.Server.Services;

namespace TreeTrack.Server.Controllers;

[ApiController]
[Route("api/projects/{projectId:int}/invites")]
[Authorize]
public class ProjectInvitesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ProjectAccessService _projectAccess;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<ProjectInvitesController> _logger;

    public ProjectInvitesController(
        ApplicationDbContext db,
        ProjectAccessService projectAccess,
        UserManager<IdentityUser> userManager,
        ILogger<ProjectInvitesController> logger)
    {
        _db = db;
        _projectAccess = projectAccess;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ProjectCollaboratorsDto>> List(int projectId)
    {
        var project = await _projectAccess.GetOwnedProjectAsync(projectId);
        if (project is null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var members = await _db.ProjectMembers
            .Where(m => m.ProjectId == projectId)
            .Include(m => m.User)
            .OrderBy(m => m.JoinedAt)
            .Select(m => new ProjectMemberDto
            {
                UserId = m.UserId,
                Email = m.User.Email ?? string.Empty,
                JoinedAt = m.JoinedAt
            })
            .ToListAsync();

        var pendingInvites = await _db.ProjectInvites
            .Where(i => i.ProjectId == projectId && i.AcceptedAt == null && i.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => ToInviteDto(i))
            .ToListAsync();

        return Ok(new ProjectCollaboratorsDto
        {
            Members = members,
            PendingInvites = pendingInvites
        });
    }

    [HttpPost]
    public async Task<ActionResult<InviteDto>> Create(int projectId, [FromBody] CreateInviteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { message = "Email is required" });
        }

        var project = await _projectAccess.GetOwnedProjectAsync(projectId);
        if (project is null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var ownerId = _projectAccess.GetCurrentUserId()!;
        var email = InviteHelper.NormalizeEmail(request.Email);

        var owner = await _db.Users.FindAsync(ownerId);
        if (owner?.Email is not null && InviteHelper.NormalizeEmail(owner.Email) == email)
        {
            return BadRequest(new { message = "You cannot invite yourself" });
        }

        var existingMember = await _userManager.FindByEmailAsync(email) is { } invitedUser
            && await _db.ProjectMembers.AnyAsync(m => m.ProjectId == projectId && m.UserId == invitedUser.Id);

        if (existingMember)
        {
            return Conflict(new { message = "This user is already a member of the project" });
        }

        var existingInvite = await _db.ProjectInvites
            .FirstOrDefaultAsync(i =>
                i.ProjectId == projectId &&
                i.Email == email &&
                i.AcceptedAt == null &&
                i.ExpiresAt > DateTime.UtcNow);

        if (existingInvite is not null)
        {
            return Conflict(new { message = "A pending invite already exists for this email" });
        }

        var invite = new ProjectInvite
        {
            ProjectId = projectId,
            Email = email,
            Token = InviteHelper.GenerateToken(),
            InvitedByUserId = ownerId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(InviteHelper.ExpiryDays)
        };

        _db.ProjectInvites.Add(invite);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Created invite for {Email} on project {ProjectId}", email, projectId);
        return CreatedAtAction(nameof(List), new { projectId }, ToInviteDto(invite));
    }

    [HttpDelete("{inviteId:int}")]
    public async Task<ActionResult> Revoke(int projectId, int inviteId)
    {
        var project = await _projectAccess.GetOwnedProjectAsync(projectId);
        if (project is null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var invite = await _db.ProjectInvites
            .FirstOrDefaultAsync(i => i.Id == inviteId && i.ProjectId == projectId && i.AcceptedAt == null);

        if (invite is null)
        {
            return NotFound(new { message = "Invite not found" });
        }

        _db.ProjectInvites.Remove(invite);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Revoked invite {InviteId} on project {ProjectId}", inviteId, projectId);
        return NoContent();
    }

    private static InviteDto ToInviteDto(ProjectInvite invite) => new()
    {
        Id = invite.Id,
        Email = invite.Email,
        Token = invite.Token,
        CreatedAt = invite.CreatedAt,
        ExpiresAt = invite.ExpiresAt
    };
}

[ApiController]
[Route("api/projects/{projectId:int}/members")]
[Authorize]
public class ProjectMembersController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ProjectAccessService _projectAccess;
    private readonly ILogger<ProjectMembersController> _logger;

    public ProjectMembersController(
        ApplicationDbContext db,
        ProjectAccessService projectAccess,
        ILogger<ProjectMembersController> logger)
    {
        _db = db;
        _projectAccess = projectAccess;
        _logger = logger;
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> Remove(int projectId, string userId)
    {
        var project = await _projectAccess.GetOwnedProjectAsync(projectId);
        if (project is null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var member = await _db.ProjectMembers
            .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == userId);

        if (member is null)
        {
            return NotFound(new { message = "Member not found" });
        }

        _db.ProjectMembers.Remove(member);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Removed member {UserId} from project {ProjectId}", userId, projectId);
        return NoContent();
    }
}
