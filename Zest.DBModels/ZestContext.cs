using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Zest.DBModels.Models;

namespace Zest.DBModels;

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

    public virtual DbSet<CommunityModerator> CommunityModerators { get; set; }

    public virtual DbSet<Follower> Followers { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Post> Posts { get; set; }
    public virtual DbSet<PostResources> PostResources { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
          
            string connectionString = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");

           
            if (!string.IsNullOrEmpty(connectionString))
            {
               
                optionsBuilder.UseSqlServer(connectionString).EnableSensitiveDataLogging();
            }
            else
            {
                
                Console.WriteLine("Warning: AZURE_SQL_CONNECTIONSTRING environment variable is not set.");
            }
        }

        base.OnConfiguring(optionsBuilder);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property(e => e.Id);
        
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
           
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Text).IsUnicode(false);

            entity.HasOne(d => d.Account).WithMany(p => p.Comments)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Accounts");

            entity.HasOne(d => d.CommentNavigation).WithMany(p => p.Replies)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("FK_Comments_Comments");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Posts");

            entity.HasMany(d => d.Likes).WithOne(p => p.Comment)
                .HasForeignKey(d => d.CommentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Likes_Comments");

        });

        modelBuilder.Entity<Community>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Information)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Creator).WithMany(p => p.Communities)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Communities_Accounts");
            
        });

        modelBuilder.Entity<CommunityFollower>(entity =>
        {
            entity.HasKey(k => new { k.AccountId, k.CommunityId });

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany()
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_CommunityFollowers_Accounts");

            entity.HasOne(d => d.Community).WithMany()
                .HasForeignKey(d => d.CommunityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CommunityFollowers_Communities");
        });

        modelBuilder.Entity<CommunityModerator>(entity =>
        {
            entity.HasKey(k => new { k.AccountId, k.CommunityId });

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany()
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CommunityModerators_Accounts");

            entity.HasOne(d => d.Community).WithMany()
                .HasForeignKey(d => d.CommunityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CommunityModerators_Communities");
        });

        modelBuilder.Entity<Follower>(entity =>
        {
            entity.HasKey(k => new { k.FollowerId, k.FollowedId });

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Followed).WithMany()
                .HasForeignKey(d => d.FollowedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Followers_Accounts1");

            entity.HasOne(d => d.FollowerNavigation).WithMany()
                .HasForeignKey(d => d.FollowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Followers_Accounts");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Likes)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Likes_Accounts");

            entity.HasOne(d => d.Comment).WithMany(p => p.Likes)
                .HasForeignKey(d => d.CommentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Likes_Comments");

            entity.HasOne(d => d.Post).WithMany(p => p.Likes)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_Likes_Posts");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Accounts");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Accounts1");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Text).IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Account).WithMany(p => p.Posts)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Posts_Accounts");

            entity.HasOne(d => d.Community).WithMany(p => p.Posts)
                .HasForeignKey(d => d.CommunityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Posts_Communities");

			entity.HasMany(d => d.Likes).WithOne(p => p.Post)
			  .HasForeignKey(d => d.PostId)
			  .OnDelete(DeleteBehavior.Cascade)
			  .HasConstraintName("FK_Likes_Posts");
		});

        modelBuilder.Entity<PostResources>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(10).IsUnicode(false);
            entity.Property(e => e.Path).IsUnicode(false);
            entity.Property(e => e.Name).IsUnicode(false);

			entity.HasOne(d => d.Post).WithMany(p => p.PostResources)
			   .HasForeignKey(d => d.PostId)
			   .OnDelete(DeleteBehavior.Cascade)
			   .HasConstraintName("FK_PostResources_Posts");
		});
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
