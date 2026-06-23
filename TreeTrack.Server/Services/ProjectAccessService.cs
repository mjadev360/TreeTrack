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

    public async Task<Project?> GetAccessibleProjectAsync(int id)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return null;
        }

        return await _db.Projects
            .Include(p => p.Issues)
            .FirstOrDefaultAsync(p =>
                p.Id == id &&
                (p.OwnerId == userId || p.Members.Any(m => m.UserId == userId)));
    }

    public async Task<bool> IsOwnerAsync(int projectId)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return false;
        }

        return await _db.Projects.AnyAsync(p => p.Id == projectId && p.OwnerId == userId);
    }

    public async Task<List<int>> GetAccessibleProjectIdsAsync()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return [];
        }

        return await _db.Projects
            .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
            .Select(p => p.Id)
            .ToListAsync();
    }
}
