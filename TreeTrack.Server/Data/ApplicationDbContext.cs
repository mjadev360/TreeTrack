using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TreeTrack.Server.Models;

namespace TreeTrack.Server.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Issue> Issues => Set<Issue>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Project>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).HasMaxLength(200).IsRequired();
            entity.Property(p => p.Key).HasMaxLength(20).IsRequired();
            entity.Property(p => p.OwnerId).HasMaxLength(450).IsRequired();
            entity.HasIndex(p => new { p.OwnerId, p.Key }).IsUnique();

            entity.HasOne(p => p.Owner)
                .WithMany()
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Issue>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Title).HasMaxLength(500).IsRequired();
            entity.Property(i => i.Assignee).HasMaxLength(100);
            entity.Property(i => i.Description).HasMaxLength(4000);
            entity.Property(i => i.Type).HasConversion<string>().HasMaxLength(20);
            entity.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(i => i.Priority).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(i => new { i.ProjectId, i.IssueNumber }).IsUnique();

            entity.HasOne(i => i.Project)
                .WithMany(p => p.Issues)
                .HasForeignKey(i => i.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.ParentIssue)
                .WithMany(i => i.Children)
                .HasForeignKey(i => i.ParentIssueId)
                .OnDelete(DeleteBehavior.Cascade);
        });

    }
}
