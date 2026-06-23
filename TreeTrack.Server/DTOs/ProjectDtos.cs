namespace TreeTrack.Server.DTOs;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int IssueCount { get; set; }
    public bool IsOwner { get; set; }
}

public class CreateProjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}

public class UpdateProjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}
