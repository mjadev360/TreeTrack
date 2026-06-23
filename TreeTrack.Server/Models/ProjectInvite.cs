using Microsoft.AspNetCore.Identity;

namespace TreeTrack.Server.Models;

public class ProjectInvite
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string InvitedByUserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }

    public Project Project { get; set; } = null!;
    public IdentityUser InvitedBy { get; set; } = null!;
}
