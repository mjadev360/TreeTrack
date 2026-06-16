using System.Text.RegularExpressions;

namespace TreeTrack.Server.Helpers;

public static partial class ProjectKeyHelper
{
    [GeneratedRegex("^[A-Z0-9]{2,10}$")]
    private static partial Regex KeyPattern();

    public static string Normalize(string key) => key.Trim().ToUpperInvariant();

    public static bool IsValid(string key) => KeyPattern().IsMatch(key);
}
