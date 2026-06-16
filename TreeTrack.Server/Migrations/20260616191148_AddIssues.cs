using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TreeTrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Key = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Issues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    IssueNumber = table.Column<int>(type: "integer", nullable: false),
                    ParentIssueId = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Assignee = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Issues_Issues_ParentIssueId",
                        column: x => x.ParentIssueId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Issues_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "CreatedAt", "Key", "Name" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PA", "ProjectAlpha" });

            migrationBuilder.InsertData(
                table: "Issues",
                columns: new[] { "Id", "Assignee", "CreatedAt", "Description", "DueDate", "IssueNumber", "ParentIssueId", "Priority", "ProjectId", "Status", "Title", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "SR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Complete overhaul of the authentication system including OAuth, SSO, and role-based access control.", new DateOnly(2026, 4, 30), 1, null, "High", 1, "InProgress", "Authentication & User Management", "Epic", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, "DT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Build the main analytics dashboard with real-time charts, KPI widgets, and custom date ranges.", new DateOnly(2026, 5, 15), 11, null, "High", 1, "Open", "Dashboard & Analytics", "Epic", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, "MJ", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Address P95 latency regressions and scale the backend for 10x traffic.", new DateOnly(2026, 4, 18), 19, null, "Critical", 1, "InProgress", "Performance & Infrastructure", "Epic", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, "LK", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ensure full mobile support across all screens from 320px to 1440px.", new DateOnly(2026, 5, 30), 24, null, "Medium", 1, "Open", "Mobile Responsiveness", "Epic", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "SR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Integrate OAuth 2.0 with support for Google, GitHub, and Microsoft providers.", new DateOnly(2026, 4, 15), 2, 1, "High", 1, "InProgress", "OAuth 2.0 Integration", "Story", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, "MJ", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Implement granular RBAC with admin, member, and viewer roles.", new DateOnly(2026, 4, 25), 6, 1, "Medium", 1, "Open", "Role-Based Access Control", "Story", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, "DT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Log all authentication events to the audit trail.", new DateOnly(2026, 4, 20), 10, 1, "Low", 1, "Review", "Audit login event logging", "Task", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, "DT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Draggable, configurable widget grid for key metrics.", new DateOnly(2026, 5, 5), 12, 11, "High", 1, "Open", "KPI Widget System", "Story", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, "LK", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Line, bar, and pie charts for all tracked metrics.", new DateOnly(2026, 5, 12), 16, 11, "Medium", 1, "Open", "Charts & Visualizations", "Story", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, "MJ", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Profile slow queries with EXPLAIN ANALYZE.", new DateOnly(2026, 4, 10), 20, 19, "Critical", 1, "InProgress", "Database query profiling", "Task", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, "MJ", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Projects list endpoint triggers 40+ individual queries due to missing eager loading.", new DateOnly(2026, 4, 11), 21, 19, "Critical", 1, "Blocked", "N+1 query on /api/projects", "Bug", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, "SR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cache frequently-read data with 60s TTL.", new DateOnly(2026, 4, 16), 22, 19, "High", 1, "Open", "Add Redis caching layer", "Task", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, "DT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Configure CloudFront distribution for JS/CSS/images.", new DateOnly(2026, 4, 5), 23, 19, "Low", 1, "Closed", "CDN setup for static assets", "Task", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, "LK", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "List all components that break below 768px.", new DateOnly(2026, 5, 10), 25, 24, "Medium", 1, "Open", "Audit existing breakpoints", "Task", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, "LK", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The hamburger menu fails to open on iPhone 14 Safari 17.", new DateOnly(2026, 5, 8), 26, 24, "High", 1, "Open", "Navbar collapses incorrectly on iOS Safari", "Bug", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "SR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Configure OAuth credentials and callback URLs.", new DateOnly(2026, 4, 8), 3, 2, "High", 1, "Closed", "Set up OAuth provider config", "Task", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, "LK", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Handle silent token refresh before expiry.", new DateOnly(2026, 4, 12), 4, 2, "Medium", 1, "InProgress", "Implement token refresh logic", "Task", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, "LK", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "After page reload the refresh token is lost from session storage.", new DateOnly(2026, 4, 9), 5, 2, "Critical", 1, "Blocked", "Refresh token not persisted on reload", "Bug", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, "MJ", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Document all permissions per role.", new DateOnly(2026, 4, 18), 7, 6, "Medium", 1, "Open", "Define permission matrix", "Task", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, "SR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Add route guards based on user role.", new DateOnly(2026, 4, 22), 8, 6, "Medium", 1, "Open", "Middleware guard implementation", "Task", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, "MJ", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Jest test coverage for all middleware guards.", new DateOnly(2026, 4, 24), 9, 6, "Low", 1, "Open", "Write unit tests for guards", "Subtask", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, "DT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Implement responsive drag-and-drop grid.", new DateOnly(2026, 4, 28), 13, 12, "High", 1, "InProgress", "Grid layout engine", "Task", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, "LK", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Connect widget slots to API data sources.", new DateOnly(2026, 5, 2), 14, 12, "Medium", 1, "Open", "Widget data connectors", "Task", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, "DT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "At tablet breakpoint widgets overlap instead of reflow.", new DateOnly(2026, 4, 26), 15, 12, "Medium", 1, "Open", "Widget overlap on resize at 768px", "Bug", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, "LK", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Evaluated and integrated Recharts.", new DateOnly(2026, 4, 14), 17, 16, "Medium", 1, "Closed", "Integrate charting library", "Task", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, "LK", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Custom date picker with presets (7d, 30d, custom).", new DateOnly(2026, 4, 22), 18, 16, "Medium", 1, "Review", "Date range picker component", "Task", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_ParentIssueId",
                table: "Issues",
                column: "ParentIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_ProjectId_IssueNumber",
                table: "Issues",
                columns: new[] { "ProjectId", "IssueNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Key",
                table: "Projects",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Issues");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
