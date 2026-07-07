using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SharingPictureOnline.Models;

public partial class SpoContext : DbContext
{
    public SpoContext()
    {
    }

    public SpoContext(DbContextOptions<SpoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<AlbumPhoto> AlbumPhotos { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Follow> Follows { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    public virtual DbSet<PhotoTag> PhotoTags { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("MyCnn"));
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.AlbumId).HasName("PK_Album");

            entity.ToTable("ALBUM");

            entity.Property(e => e.AlbumId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("albumID");
            entity.Property(e => e.CoverPhotoId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("coverPhotoID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("userID");
            entity.Property(e => e.Visibility)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("visibility");

            entity.HasOne(d => d.User).WithMany(p => p.Albums)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Album_User");
        });

        modelBuilder.Entity<AlbumPhoto>(entity =>
        {
            entity.HasKey(e => e.AlbumPhotoId).HasName("PK_AlbumPhoto");

            entity.ToTable("ALBUM_PHOTO");

            entity.Property(e => e.AlbumPhotoId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("albumPhotoID");
            entity.Property(e => e.AlbumId).HasColumnName("albumID");
            entity.Property(e => e.PhotoId).HasColumnName("photoID");

            entity.HasOne(d => d.Album).WithMany(p => p.AlbumPhotos)
                .HasForeignKey(d => d.AlbumId)
                .HasConstraintName("FK_AlbumPhoto_Album");

            entity.HasOne(d => d.Photo).WithMany(p => p.AlbumPhotos)
                .HasForeignKey(d => d.PhotoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AlbumPhoto_Photo");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK_Comment");

            entity.ToTable("COMMENT");

            entity.Property(e => e.CommentId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("commentID");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.ParentId).HasColumnName("parentID");
            entity.Property(e => e.PhotoId).HasColumnName("photoID");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Comment_Parent");

            entity.HasOne(d => d.Photo).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PhotoId)
                .HasConstraintName("FK_Comment_Photo");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comment_User");
        });

        modelBuilder.Entity<Follow>(entity =>
        {
            entity.HasKey(e => e.FollowId).HasName("PK_Follow");

            entity.ToTable("FOLLOW");

            entity.HasIndex(e => new { e.FollowerId, e.FollowingId }, "UQ_Follower_Following").IsUnique();

            entity.Property(e => e.FollowId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("followID");
            entity.Property(e => e.FollowedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("followedAt");
            entity.Property(e => e.FollowerId).HasColumnName("followerID");
            entity.Property(e => e.FollowingId).HasColumnName("followingID");

            entity.HasOne(d => d.Follower).WithMany(p => p.FollowFollowers)
                .HasForeignKey(d => d.FollowerId)
                .HasConstraintName("FK_Follow_Follower");

            entity.HasOne(d => d.Following).WithMany(p => p.FollowFollowings)
                .HasForeignKey(d => d.FollowingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Follow_Following");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PK_Like");

            entity.ToTable("LIKE");

            entity.HasIndex(e => new { e.UserId, e.PhotoId }, "UQ_User_Photo_Like").IsUnique();

            entity.Property(e => e.LikeId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("likeID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.PhotoId).HasColumnName("photoID");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.Photo).WithMany(p => p.Likes)
                .HasForeignKey(d => d.PhotoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Like_Photo");

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Like_User");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotifId).HasName("PK_Notification");

            entity.ToTable("NOTIFICATION");

            entity.Property(e => e.NotifId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("notifID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsRead).HasColumnName("isRead");
            entity.Property(e => e.RefId).HasColumnName("refID");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Notification_User");
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.PhotoId).HasName("PK_Photo");

            entity.ToTable("PHOTO");

            entity.Property(e => e.PhotoId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("photoID");
            entity.Property(e => e.CommentCount).HasColumnName("commentCount");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.LikeCount).HasColumnName("likeCount");
            entity.Property(e => e.StorageKey)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("storageKey");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("uploadedAt");
            entity.Property(e => e.UserId).HasColumnName("userID");
            entity.Property(e => e.Visibility)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("visibility");

            entity.HasOne(d => d.User).WithMany(p => p.Photos)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Photo_User");
        });

        modelBuilder.Entity<PhotoTag>(entity =>
        {
            entity.HasKey(e => e.PhotoTagId).HasName("PK_PhotoTag");

            entity.ToTable("PHOTO_TAG");

            entity.Property(e => e.PhotoTagId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("photoTagID");
            entity.Property(e => e.PhotoId).HasColumnName("photoID");
            entity.Property(e => e.TagId).HasColumnName("tagID");

            entity.HasOne(d => d.Photo).WithMany(p => p.PhotoTags)
                .HasForeignKey(d => d.PhotoId)
                .HasConstraintName("FK_PhotoTag_Photo");

            entity.HasOne(d => d.Tag).WithMany(p => p.PhotoTags)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("FK_PhotoTag_Tag");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK_Report");

            entity.ToTable("REPORT");

            entity.Property(e => e.ReportId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("reportID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.ReporterId).HasColumnName("reporterID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("PENDING")
                .HasColumnName("status");
            entity.Property(e => e.TargetId).HasColumnName("targetID");
            entity.Property(e => e.TargetType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("targetType");

            entity.HasOne(d => d.Reporter).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ReporterId)
                .HasConstraintName("FK_Report_Reporter");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK_Tag");

            entity.ToTable("TAG");

            entity.HasIndex(e => e.Name, "UQ_Tag_Name").IsUnique();

            entity.Property(e => e.TagId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("tagID");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UsageCount).HasColumnName("usageCount");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_User");

            entity.ToTable("USER");

            entity.HasIndex(e => e.Email, "UQ_User_Email").IsUnique();

            entity.HasIndex(e => e.Username, "UQ_User_Username").IsUnique();

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("userID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("passwordHash");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK_UserProfile");

            entity.ToTable("USER_PROFILE");

            entity.HasIndex(e => e.UserId, "UQ_UserProfile_User").IsUnique();

            entity.Property(e => e.ProfileId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("profileID");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("avatarURL");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(255)
                .HasColumnName("displayName");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UserId).HasColumnName("userID");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("website");

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.UserId)
                .HasConstraintName("FK_UserProfile_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
