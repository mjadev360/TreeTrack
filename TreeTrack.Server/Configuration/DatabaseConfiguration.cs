using Npgsql;

namespace TreeTrack.Server.Configuration;

public static class DatabaseConfiguration
{
    public static string? ResolveConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var databaseUrl = configuration["DATABASE_URL"];

        var raw = !string.IsNullOrWhiteSpace(connectionString)
            ? connectionString
            : databaseUrl;

        if (string.IsNullOrWhiteSpace(raw))
            return null;

        if (raw.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
            raw.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return ParsePostgresUrl(raw);
        }

        return raw;
    }

    private static string ParsePostgresUrl(string url)
    {
        var uri = new Uri(url);
        var userInfo = uri.UserInfo.Split(':', 2);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
            SslMode = SslMode.Require,
        };

        var query = uri.Query.TrimStart('?');
        if (query.Contains("sslmode=disable", StringComparison.OrdinalIgnoreCase))
            builder.SslMode = SslMode.Disable;

        return builder.ConnectionString;
    }
}
