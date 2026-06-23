using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeTrack.Server.Data;
using TreeTrack.Server.DTOs;
using TreeTrack.Server.Models;
using TreeTrack.Server.Services;

namespace TreeTrack.Server.Controllers;

[ApiController]
[Route("api/projects/{projectId:int}/issues")]
[Authorize]
public class IssuesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ProjectAccessService _projectAccess;
    private readonly ILogger<IssuesController> _logger;

    public IssuesController(
        ApplicationDbContext db,
        ProjectAccessService projectAccess,
        ILogger<IssuesController> logger)
    {
        _db = db;
        _projectAccess = projectAccess;
        _logger = logger;
    }

    [HttpGet("tree")]
    public async Task<ActionResult<List<IssueTreeNodeDto>>> GetTree(int projectId)
    {
        var project = await _projectAccess.GetAccessibleProjectAsync(projectId);
        if (project is null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var issues = await _db.Issues
            .Where(i => i.ProjectId == project.Id)
            .OrderBy(i => i.IssueNumber)
            .ToListAsync();

        var tree = BuildTree(issues, project.Key, null);
        return Ok(tree);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<IssueDetailDto>> GetById(int projectId, int id)
    {
        var project = await _projectAccess.GetAccessibleProjectAsync(projectId);
        if (project is null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var issue = await _db.Issues
            .FirstOrDefaultAsync(i => i.Id == id && i.ProjectId == project.Id);

        if (issue is null)
        {
            return NotFound(new { message = "Issue not found" });
        }

        return Ok(IssueDtoMapper.ToDetail(issue, project.Key));
    }

    [HttpPost]
    public async Task<ActionResult<IssueDetailDto>> Create(int projectId, [FromBody] CreateIssueRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new { message = "Title is required" });
        }

        var project = await _projectAccess.GetAccessibleProjectAsync(projectId);
        if (project is null)
        {
            return NotFound(new { message = "Project not found" });
        }

        if (request.ParentIssueId.HasValue)
        {
            var parentExists = await _db.Issues.AnyAsync(i =>
                i.Id == request.ParentIssueId.Value && i.ProjectId == project.Id);

            if (!parentExists)
            {
                return BadRequest(new { message = "Parent issue not found" });
            }
        }

        try
        {
            var issue = new Issue
            {
                ProjectId = project.Id,
                IssueNumber = await GetNextIssueNumberAsync(project.Id),
                ParentIssueId = request.ParentIssueId,
                Type = IssueDtoMapper.ParseType(request.Type),
                Title = request.Title.Trim(),
                Status = string.IsNullOrWhiteSpace(request.Status)
                    ? IssueStatus.Open
                    : IssueDtoMapper.ParseStatus(request.Status),
                Priority = string.IsNullOrWhiteSpace(request.Priority)
                    ? IssuePriority.Medium
                    : IssueDtoMapper.ParsePriority(request.Priority),
                Assignee = string.IsNullOrWhiteSpace(request.Assignee) ? null : request.Assignee.Trim(),
                DueDate = IssueDtoMapper.ParseDueDate(request.DueDate),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Issues.Add(issue);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Created issue {IssueKey}", IssueDtoMapper.BuildKey(project.Key, issue.IssueNumber));
            return CreatedAtAction(nameof(GetById), new { projectId, id = issue.Id }, IssueDtoMapper.ToDetail(issue, project.Key));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<IssueDetailDto>> Update(int projectId, int id, [FromBody] UpdateIssueRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new { message = "Title is required" });
        }

        var project = await _projectAccess.GetAccessibleProjectAsync(projectId);
        if (project is null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var issue = await _db.Issues
            .FirstOrDefaultAsync(i => i.Id == id && i.ProjectId == project.Id);

        if (issue is null)
        {
            return NotFound(new { message = "Issue not found" });
        }

        try
        {
            issue.Title = request.Title.Trim();
            issue.Type = IssueDtoMapper.ParseType(request.Type);
            issue.Status = IssueDtoMapper.ParseStatus(request.Status);
            issue.Priority = IssueDtoMapper.ParsePriority(request.Priority);
            issue.Assignee = string.IsNullOrWhiteSpace(request.Assignee) ? null : request.Assignee.Trim();
            issue.DueDate = IssueDtoMapper.ParseDueDate(request.DueDate);
            issue.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
            issue.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            _logger.LogInformation("Updated issue {IssueId}", issue.Id);
            return Ok(IssueDtoMapper.ToDetail(issue, project.Key));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int projectId, int id)
    {
        var project = await _projectAccess.GetAccessibleProjectAsync(projectId);
        if (project is null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var issue = await _db.Issues
            .FirstOrDefaultAsync(i => i.Id == id && i.ProjectId == project.Id);

        if (issue is null)
        {
            return NotFound(new { message = "Issue not found" });
        }

        _db.Issues.Remove(issue);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Deleted issue {IssueId}", issue.Id);
        return NoContent();
    }

    private async Task<int> GetNextIssueNumberAsync(int projectId)
    {
        var maxNumber = await _db.Issues
            .Where(i => i.ProjectId == projectId)
            .Select(i => (int?)i.IssueNumber)
            .MaxAsync();

        return (maxNumber ?? 0) + 1;
    }

    private static List<IssueTreeNodeDto> BuildTree(IReadOnlyList<Issue> issues, string projectKey, int? parentId)
    {
        return issues
            .Where(i => i.ParentIssueId == parentId)
            .OrderBy(i => i.IssueNumber)
            .Select(issue =>
            {
                var node = IssueDtoMapper.ToTreeNode(issue, projectKey);
                node.Children = BuildTree(issues, projectKey, issue.Id);
                return node;
            })
            .ToList();
    }
}
