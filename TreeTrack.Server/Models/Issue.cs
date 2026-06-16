namespace TreeTrack.Server.Models;

public class Issue
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int IssueNumber { get; set; }
    public int? ParentIssueId { get; set; }
    public IssueType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public IssueStatus Status { get; set; }
    public IssuePriority Priority { get; set; }
    public string? Assignee { get; set; }
    public DateOnly? DueDate { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Project Project { get; set; } = null!;
    public Issue? ParentIssue { get; set; }
    public ICollection<Issue> Children { get; set; } = [];
}
