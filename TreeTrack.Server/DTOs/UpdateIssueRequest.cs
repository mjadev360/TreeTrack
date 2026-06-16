namespace TreeTrack.Server.DTOs;

public class UpdateIssueRequest
{
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string? Assignee { get; set; }
    public string? DueDate { get; set; }
    public string? Description { get; set; }
}
