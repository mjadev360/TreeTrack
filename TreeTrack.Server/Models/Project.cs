using Microsoft.AspNetCore.Identity;

namespace TreeTrack.Server.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public IdentityUser Owner { get; set; } = null!;
    public ICollection<Issue> Issues { get; set; } = [];
    public ICollection<ProjectMember> Members { get; set; } = [];
    public ICollection<ProjectInvite> Invites { get; set; } = [];
}
