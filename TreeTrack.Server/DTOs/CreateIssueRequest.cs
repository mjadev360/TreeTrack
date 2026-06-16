namespace TreeTrack.Server.DTOs;

public class CreateIssueRequest
{
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int? ParentIssueId { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? Assignee { get; set; }
    public string? DueDate { get; set; }
    public string? Description { get; set; }
}
