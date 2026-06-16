using TreeTrack.Server.Models;

namespace TreeTrack.Server.DTOs;

public static class IssueDtoMapper
{
    public static string ToApiValue(IssueType type) => type switch
    {
        IssueType.Epic => "epic",
        IssueType.Story => "story",
        IssueType.Task => "task",
        IssueType.Bug => "bug",
        IssueType.Subtask => "subtask",
        _ => type.ToString().ToLowerInvariant()
    };

    public static string ToApiValue(IssueStatus status) => status switch
    {
        IssueStatus.Open => "open",
        IssueStatus.InProgress => "in-progress",
        IssueStatus.Review => "review",
        IssueStatus.Blocked => "blocked",
        IssueStatus.Closed => "closed",
        _ => status.ToString().ToLowerInvariant()
    };

    public static string ToApiValue(IssuePriority priority) => priority switch
    {
        IssuePriority.Critical => "critical",
        IssuePriority.High => "high",
        IssuePriority.Medium => "medium",
        IssuePriority.Low => "low",
        _ => priority.ToString().ToLowerInvariant()
    };

    public static IssueType ParseType(string value) => value.ToLowerInvariant() switch
    {
        "epic" => IssueType.Epic,
        "story" => IssueType.Story,
        "task" => IssueType.Task,
        "bug" => IssueType.Bug,
        "subtask" => IssueType.Subtask,
        _ => throw new ArgumentException($"Invalid issue type: {value}")
    };

    public static IssueStatus ParseStatus(string value) => value.ToLowerInvariant() switch
    {
        "open" => IssueStatus.Open,
        "in-progress" => IssueStatus.InProgress,
        "review" => IssueStatus.Review,
        "blocked" => IssueStatus.Blocked,
        "closed" => IssueStatus.Closed,
        _ => throw new ArgumentException($"Invalid issue status: {value}")
    };

    public static IssuePriority ParsePriority(string value) => value.ToLowerInvariant() switch
    {
        "critical" => IssuePriority.Critical,
        "high" => IssuePriority.High,
        "medium" => IssuePriority.Medium,
        "low" => IssuePriority.Low,
        _ => throw new ArgumentException($"Invalid issue priority: {value}")
    };

    public static string? FormatDueDate(DateOnly? dueDate) =>
        dueDate?.ToString("yyyy-MM-dd");

    public static DateOnly? ParseDueDate(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : DateOnly.Parse(value);

    public static string BuildKey(string projectKey, int issueNumber) =>
        $"{projectKey}-{issueNumber}";

    public static IssueTreeNodeDto ToTreeNode(Issue issue, string projectKey)
    {
        return new IssueTreeNodeDto
        {
            Id = issue.Id,
            Key = BuildKey(projectKey, issue.IssueNumber),
            Type = ToApiValue(issue.Type),
            Title = issue.Title,
            Status = ToApiValue(issue.Status),
            Priority = ToApiValue(issue.Priority),
            Assignee = issue.Assignee,
            DueDate = FormatDueDate(issue.DueDate),
            Description = issue.Description,
            Children = issue.Children
                .OrderBy(child => child.IssueNumber)
                .Select(child => ToTreeNode(child, projectKey))
                .ToList()
        };
    }

    public static IssueDetailDto ToDetail(Issue issue, string projectKey)
    {
        return new IssueDetailDto
        {
            Id = issue.Id,
            Key = BuildKey(projectKey, issue.IssueNumber),
            ParentIssueId = issue.ParentIssueId,
            Type = ToApiValue(issue.Type),
            Title = issue.Title,
            Status = ToApiValue(issue.Status),
            Priority = ToApiValue(issue.Priority),
            Assignee = issue.Assignee,
            DueDate = FormatDueDate(issue.DueDate),
            Description = issue.Description
        };
    }
}
