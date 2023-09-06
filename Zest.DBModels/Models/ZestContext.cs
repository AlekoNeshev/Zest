using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zest.DBModels.Models;

public partial class ZestContext : DbContext
{
    public ZestContext()
    {
    }

    public ZestContext(DbContextOptions<ZestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Community> Communities { get; set; }

    public virtual DbSet<CommunityFollower> CommunityFollowers { get; set; }

    public virtual DbSet<Follower> Followers { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-77S9KIH\\SQLEXPRESS;Database=Zest;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Birthdate).HasColumnType("date");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Text).IsUnicode(false);
        });

        modelBuilder.Entity<Community>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Information)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CommunityFollower>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Follower>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Text).IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
