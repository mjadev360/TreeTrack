namespace TreeTrack.Server.DTOs;

public class CreateInviteRequest
{
    public string Email { get; set; } = string.Empty;
}

public class InviteDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class InvitePreviewDto
{
    public string ProjectName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsExpired { get; set; }
    public bool IsAccepted { get; set; }
}

public class ProjectMemberDto
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}

public class ProjectCollaboratorsDto
{
    public List<ProjectMemberDto> Members { get; set; } = [];
    public List<InviteDto> PendingInvites { get; set; } = [];
}
