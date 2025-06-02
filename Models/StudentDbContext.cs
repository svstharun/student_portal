using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StudentPortalDBFirst.Models;

public partial class StudentDbContext : DbContext
{
    public StudentDbContext()
    {
    }

    public StudentDbContext(DbContextOptions<StudentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Students> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Server=localhost;Database=StudentDB1;User Id=sa;Password=qipcbsiiu;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Students>(entity =>
        {
            modelBuilder.Entity<Students>().ToTable("Students");

            /*entity.HasKey(e => e.StudentId).HasName("PK__Students__3214EC07E59FCE7C");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CollegeName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.StudentName).HasMaxLength(100);*/
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
