using Microsoft.AspNetCore.Identity;

namespace TreeTrack.Server.Models;

public class ProjectMember
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }

    public Project Project { get; set; } = null!;
    public IdentityUser User { get; set; } = null!;
}
