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
[Route("api/invites")]
public class InvitesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ProjectAccessService _projectAccess;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<InvitesController> _logger;

    public InvitesController(
        ApplicationDbContext db,
        ProjectAccessService projectAccess,
        UserManager<IdentityUser> userManager,
        ILogger<InvitesController> logger)
    {
        _db = db;
        _projectAccess = projectAccess;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet("{token}")]
    [AllowAnonymous]
    public async Task<ActionResult<InvitePreviewDto>> GetPreview(string token)
    {
        var invite = await _db.ProjectInvites
            .Include(i => i.Project)
            .FirstOrDefaultAsync(i => i.Token == token);

        if (invite is null)
        {
            return NotFound(new { message = "Invite not found" });
        }

        return Ok(new InvitePreviewDto
        {
            ProjectName = invite.Project.Name,
            Email = invite.Email,
            IsExpired = invite.ExpiresAt <= DateTime.UtcNow,
            IsAccepted = invite.AcceptedAt is not null
        });
    }

    [HttpPost("{token}/accept")]
    [Authorize]
    public async Task<ActionResult> Accept(string token)
    {
        var invite = await _db.ProjectInvites
            .Include(i => i.Project)
            .FirstOrDefaultAsync(i => i.Token == token);

        if (invite is null)
        {
            return NotFound(new { message = "Invite not found" });
        }

        if (invite.AcceptedAt is not null)
        {
            return BadRequest(new { message = "This invite has already been accepted" });
        }

        if (invite.ExpiresAt <= DateTime.UtcNow)
        {
            return BadRequest(new { message = "This invite has expired" });
        }

        var userId = _projectAccess.GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user?.Email is null || InviteHelper.NormalizeEmail(user.Email) != invite.Email)
        {
            return BadRequest(new { message = "You must be logged in with the invited email address" });
        }

        if (invite.Project.OwnerId == userId)
        {
            return BadRequest(new { message = "You already own this project" });
        }

        var alreadyMember = await _db.ProjectMembers
            .AnyAsync(m => m.ProjectId == invite.ProjectId && m.UserId == userId);

        if (alreadyMember)
        {
            invite.AcceptedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok(new { message = "You are already a member of this project", projectId = invite.ProjectId });
        }

        _db.ProjectMembers.Add(new ProjectMember
        {
            ProjectId = invite.ProjectId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow
        });

        invite.AcceptedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        _logger.LogInformation("User {UserId} accepted invite to project {ProjectId}", userId, invite.ProjectId);
        return Ok(new { message = "Invite accepted", projectId = invite.ProjectId });
    }
}
