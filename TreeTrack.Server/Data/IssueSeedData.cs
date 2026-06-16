using TreeTrack.Server.Models;

namespace TreeTrack.Server.Data;

public static class IssueSeedData
{
    private static readonly DateTime SeedTime = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Project DefaultProject = new()
    {
        Id = 1,
        Name = "ProjectAlpha",
        Key = "PA",
        CreatedAt = SeedTime
    };

    public static readonly Issue[] Issues =
    [
        Issue(1, null, IssueType.Epic, "Authentication & User Management", IssueStatus.InProgress, IssuePriority.High, "SR", new DateOnly(2026, 4, 30),
            "Complete overhaul of the authentication system including OAuth, SSO, and role-based access control."),
        Issue(2, 1, IssueType.Story, "OAuth 2.0 Integration", IssueStatus.InProgress, IssuePriority.High, "SR", new DateOnly(2026, 4, 15),
            "Integrate OAuth 2.0 with support for Google, GitHub, and Microsoft providers."),
        Issue(3, 2, IssueType.Task, "Set up OAuth provider config", IssueStatus.Closed, IssuePriority.High, "SR", new DateOnly(2026, 4, 8),
            "Configure OAuth credentials and callback URLs."),
        Issue(4, 2, IssueType.Task, "Implement token refresh logic", IssueStatus.InProgress, IssuePriority.Medium, "LK", new DateOnly(2026, 4, 12),
            "Handle silent token refresh before expiry."),
        Issue(5, 2, IssueType.Bug, "Refresh token not persisted on reload", IssueStatus.Blocked, IssuePriority.Critical, "LK", new DateOnly(2026, 4, 9),
            "After page reload the refresh token is lost from session storage."),
        Issue(6, 1, IssueType.Story, "Role-Based Access Control", IssueStatus.Open, IssuePriority.Medium, "MJ", new DateOnly(2026, 4, 25),
            "Implement granular RBAC with admin, member, and viewer roles."),
        Issue(7, 6, IssueType.Task, "Define permission matrix", IssueStatus.Open, IssuePriority.Medium, "MJ", new DateOnly(2026, 4, 18),
            "Document all permissions per role."),
        Issue(8, 6, IssueType.Task, "Middleware guard implementation", IssueStatus.Open, IssuePriority.Medium, "SR", new DateOnly(2026, 4, 22),
            "Add route guards based on user role."),
        Issue(9, 6, IssueType.Subtask, "Write unit tests for guards", IssueStatus.Open, IssuePriority.Low, "MJ", new DateOnly(2026, 4, 24),
            "Jest test coverage for all middleware guards."),
        Issue(10, 1, IssueType.Task, "Audit login event logging", IssueStatus.Review, IssuePriority.Low, "DT", new DateOnly(2026, 4, 20),
            "Log all authentication events to the audit trail."),
        Issue(11, null, IssueType.Epic, "Dashboard & Analytics", IssueStatus.Open, IssuePriority.High, "DT", new DateOnly(2026, 5, 15),
            "Build the main analytics dashboard with real-time charts, KPI widgets, and custom date ranges."),
        Issue(12, 11, IssueType.Story, "KPI Widget System", IssueStatus.Open, IssuePriority.High, "DT", new DateOnly(2026, 5, 5),
            "Draggable, configurable widget grid for key metrics."),
        Issue(13, 12, IssueType.Task, "Grid layout engine", IssueStatus.InProgress, IssuePriority.High, "DT", new DateOnly(2026, 4, 28),
            "Implement responsive drag-and-drop grid."),
        Issue(14, 12, IssueType.Task, "Widget data connectors", IssueStatus.Open, IssuePriority.Medium, "LK", new DateOnly(2026, 5, 2),
            "Connect widget slots to API data sources."),
        Issue(15, 12, IssueType.Bug, "Widget overlap on resize at 768px", IssueStatus.Open, IssuePriority.Medium, "DT", new DateOnly(2026, 4, 26),
            "At tablet breakpoint widgets overlap instead of reflow."),
        Issue(16, 11, IssueType.Story, "Charts & Visualizations", IssueStatus.Open, IssuePriority.Medium, "LK", new DateOnly(2026, 5, 12),
            "Line, bar, and pie charts for all tracked metrics."),
        Issue(17, 16, IssueType.Task, "Integrate charting library", IssueStatus.Closed, IssuePriority.Medium, "LK", new DateOnly(2026, 4, 14),
            "Evaluated and integrated Recharts."),
        Issue(18, 16, IssueType.Task, "Date range picker component", IssueStatus.Review, IssuePriority.Medium, "LK", new DateOnly(2026, 4, 22),
            "Custom date picker with presets (7d, 30d, custom)."),
        Issue(19, null, IssueType.Epic, "Performance & Infrastructure", IssueStatus.InProgress, IssuePriority.Critical, "MJ", new DateOnly(2026, 4, 18),
            "Address P95 latency regressions and scale the backend for 10x traffic."),
        Issue(20, 19, IssueType.Task, "Database query profiling", IssueStatus.InProgress, IssuePriority.Critical, "MJ", new DateOnly(2026, 4, 10),
            "Profile slow queries with EXPLAIN ANALYZE."),
        Issue(21, 19, IssueType.Bug, "N+1 query on /api/projects", IssueStatus.Blocked, IssuePriority.Critical, "MJ", new DateOnly(2026, 4, 11),
            "Projects list endpoint triggers 40+ individual queries due to missing eager loading."),
        Issue(22, 19, IssueType.Task, "Add Redis caching layer", IssueStatus.Open, IssuePriority.High, "SR", new DateOnly(2026, 4, 16),
            "Cache frequently-read data with 60s TTL."),
        Issue(23, 19, IssueType.Task, "CDN setup for static assets", IssueStatus.Closed, IssuePriority.Low, "DT", new DateOnly(2026, 4, 5),
            "Configure CloudFront distribution for JS/CSS/images."),
        Issue(24, null, IssueType.Epic, "Mobile Responsiveness", IssueStatus.Open, IssuePriority.Medium, "LK", new DateOnly(2026, 5, 30),
            "Ensure full mobile support across all screens from 320px to 1440px."),
        Issue(25, 24, IssueType.Task, "Audit existing breakpoints", IssueStatus.Open, IssuePriority.Medium, "LK", new DateOnly(2026, 5, 10),
            "List all components that break below 768px."),
        Issue(26, 24, IssueType.Bug, "Navbar collapses incorrectly on iOS Safari", IssueStatus.Open, IssuePriority.High, "LK", new DateOnly(2026, 5, 8),
            "The hamburger menu fails to open on iPhone 14 Safari 17.")
    ];

    private static Issue Issue(
        int number,
        int? parentNumber,
        IssueType type,
        string title,
        IssueStatus status,
        IssuePriority priority,
        string assignee,
        DateOnly? dueDate,
        string description)
    {
        return new Issue
        {
            Id = number,
            ProjectId = 1,
            IssueNumber = number,
            ParentIssueId = parentNumber,
            Type = type,
            Title = title,
            Status = status,
            Priority = priority,
            Assignee = assignee,
            DueDate = dueDate,
            Description = description,
            CreatedAt = SeedTime,
            UpdatedAt = SeedTime
        };
    }
}
