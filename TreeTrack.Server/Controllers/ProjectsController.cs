using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeTrack.Server.Data;
using TreeTrack.Server.DTOs;
using TreeTrack.Server.Helpers;
using TreeTrack.Server.Models;
using TreeTrack.Server.Services;

namespace TreeTrack.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ProjectAccessService _projectAccess;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(
        ApplicationDbContext db,
        ProjectAccessService projectAccess,
        ILogger<ProjectsController> logger)
    {
        _db = db;
        _projectAccess = projectAccess;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> List()
    {
        var ownerId = _projectAccess.GetCurrentUserId();
        if (ownerId is null)
        {
            return Unauthorized();
        }

        var projects = await _db.Projects
            .Where(p => p.OwnerId == ownerId)
            .OrderBy(p => p.Name)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Key = p.Key,
                CreatedAt = p.CreatedAt,
                IssueCount = p.Issues.Count
            })
            .ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id)
    {
        var project = await _projectAccess.GetOwnedProjectAsync(id);
        if (project is null)
        {
            return NotFound(new { message = "Project not found" });
        }

        return Ok(ToDto(project));
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectRequest request)
    {
        var ownerId = _projectAccess.GetCurrentUserId();
        if (ownerId is null)
        {
            return Unauthorized();
        }

        var validationError = ValidateProjectFields(request.Name, request.Key);
        if (validationError is not null)
        {
            return BadRequest(new { message = validationError });
        }

        var key = ProjectKeyHelper.Normalize(request.Key);

        if (await _db.Projects.AnyAsync(p => p.OwnerId == ownerId && p.Key == key))
        {
            return Conflict(new { message = "A project with this key already exists" });
        }

        var project = new Project
        {
            Name = request.Name.Trim(),
            Key = key,
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Created project {ProjectKey} for user {OwnerId}", project.Key, ownerId);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, ToDto(project));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectDto>> Update(int id, [FromBody] UpdateProjectRequest request)
    {
        var project = await _projectAccess.GetOwnedProjectAsync(id);
        if (project is null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var validationError = ValidateProjectFields(request.Name, request.Key);
        if (validationError is not null)
        {
            return BadRequest(new { message = validationError });
        }

        var key = ProjectKeyHelper.Normalize(request.Key);

        if (await _db.Projects.AnyAsync(p => p.OwnerId == project.OwnerId && p.Key == key && p.Id != id))
        {
            return Conflict(new { message = "A project with this key already exists" });
        }

        project.Name = request.Name.Trim();
        project.Key = key;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Updated project {ProjectId}", project.Id);
        return Ok(ToDto(project));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var project = await _projectAccess.GetOwnedProjectAsync(id);
        if (project is null)
        {
            return NotFound(new { message = "Project not found" });
        }

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Deleted project {ProjectId}", project.Id);
        return NoContent();
    }

    private static string? ValidateProjectFields(string name, string key)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Name is required";
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            return "Key is required";
        }

        var normalizedKey = ProjectKeyHelper.Normalize(key);
        if (!ProjectKeyHelper.IsValid(normalizedKey))
        {
            return "Key must be 2-10 uppercase letters or numbers";
        }

        return null;
    }

    private static ProjectDto ToDto(Project project) => new()
    {
        Id = project.Id,
        Name = project.Name,
        Key = project.Key,
        CreatedAt = project.CreatedAt,
        IssueCount = project.Issues.Count
    };
}
