using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options ):base(options)
        {
        }    
        public DbSet<Developers> Developers { get; set; } = null!;
        public DbSet<ProjectManager> ProjectManagers { get; set; } = null!;
        public DbSet<ProjectManagerDeveloper> ProjectManagerDevelopers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Developers>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.HasOne(d => d.User)
                      .WithOne(u => u.Developer)
                      .HasForeignKey<Developers>(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(entity => entity.UserId).IsUnique();
            });

            builder.Entity<ProjectManager>(entity =>
            {
                entity.HasKey(pm => pm.Id);

                entity.HasOne(pm => pm.User)
                      .WithOne(u => u.ProjectManager)
                      .HasForeignKey<ProjectManager>(pm => pm.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(entity => entity.UserId).IsUnique();
            });

            builder.Entity<ProjectManagerDeveloper>(entity =>
            {
                entity.HasKey(pmd => new { pmd.ProjectManagerId, pmd.DeveloperId });

                entity.HasOne(pmd => pmd.ProjectManager)
                      .WithMany(u => u.ProjectManagerDevelopers)
                      .HasForeignKey(pmd => pmd.ProjectManagerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pmd => pmd.Developer)
                      .WithMany(u => u.ProjectManagerDevelopers)
                      .HasForeignKey(pmd => pmd.DeveloperId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(entity => new
                { entity.ProjectManagerId, entity.DeveloperId });
            });
        }
    }
}
