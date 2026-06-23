using TreeTrack.Server.Models;

namespace TreeTrack.Server.Helpers;

public static class InviteHelper
{
    public const int ExpiryDays = 7;

    public static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();

    public static string GenerateToken()
    {
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public static bool IsExpired(ProjectInvite invite) =>
        invite.AcceptedAt is not null || invite.ExpiresAt <= DateTime.UtcNow;
}
