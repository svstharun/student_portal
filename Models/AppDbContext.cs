using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StudentPortalDBFirst.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<UsersMaster> UsersMasters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Server=localhost;Database= Master1;User Id=sa;Password=qipcbsiiu;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)  // part is critical for customizing how your model maps to the database.
    {
        modelBuilder.Entity<UsersMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UsersMas__3214EC0752C5A699");

            entity.ToTable("UsersMaster");

            entity.HasIndex(e => e.Username, "UQ__UsersMas__536C85E4734ADF8A").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Mobile).HasMaxLength(15);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
