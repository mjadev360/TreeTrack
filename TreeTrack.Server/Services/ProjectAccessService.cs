using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TreeTrack.Server.Data;
using TreeTrack.Server.Models;

namespace TreeTrack.Server.Services;

public class ProjectAccessService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProjectAccessService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentUserId() =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

    public async Task<Project?> GetOwnedProjectAsync(int id)
    {
        var ownerId = GetCurrentUserId();
        if (ownerId is null)
        {
            return null;
        }

        return await _db.Projects
            .Include(p => p.Issues)
            .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == ownerId);
    }
}
