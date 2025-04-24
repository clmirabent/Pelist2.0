using System;
using System.Collections.Generic;
using Films.Models;
using Microsoft.EntityFrameworkCore;

namespace Films.Context;

public partial class FilmsDbContext : DbContext
{
    public FilmsDbContext()
    {
    }

    public FilmsDbContext(DbContextOptions<FilmsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Friend> Friends { get; set; }

    public virtual DbSet<List> Lists { get; set; }

    public virtual DbSet<MovieReview> MovieReviews { get; set; }

    public virtual DbSet<Preference> Preferences { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<TypeList> TypeLists { get; set; }

    public DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(e => e.IdFriend).HasName("PK__Friend__30F0D95255E854BC");

           entity.ToTable("Friend");

            entity.HasOne(d => d.FkIdFriendNavigation).WithMany(p => p.FriendFkIdFriendNavigations)
                .HasForeignKey(d => d.FkIdFriend)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friend_User2");

            entity.HasOne(d => d.FkIdUserNavigation).WithMany(p => p.FriendFkIdUserNavigations)
                .HasForeignKey(d => d.FkIdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friend_User1");
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => e.IdList).HasName("PK__List__31D88DBE0157BFC1");

            entity.ToTable("List");

            entity.HasOne(d => d.FkIdTypeListNavigation).WithMany(p => p.Lists)
                .HasForeignKey(d => d.FkIdTypeList)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_List_TypeList");

            entity.HasOne(d => d.FkIdUserNavigation).WithMany(p => p.Lists)
                .HasForeignKey(d => d.FkIdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_List_User");
        });

        modelBuilder.Entity<MovieReview>(entity =>
        {
            entity.HasKey(e => e.IdMovieReview).HasName("PK__MovieRev__2C11472A4465890B");

            entity.ToTable("MovieReview");

            entity.Property(e => e.AverageRating).HasColumnType("decimal(3, 2)");
        });

        modelBuilder.Entity<Preference>(entity =>
        {
            entity.HasKey(e => e.IdPreference).HasName("PK__Preferen__5756D25A9880B3EE");

            entity.ToTable("Preference");

            entity.HasOne(d => d.FkIdUserNavigation).WithMany(p => p.Preferences)
                .HasForeignKey(d => d.FkIdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Preference_User");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.IdReview).HasName("PK__Review__BB56047DC8658AB8");

            entity.ToTable("Review");

            entity.Property(e => e.Title).HasMaxLength(256);

            entity.HasOne(d => d.FkIdUserNavigation).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.FkIdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_User");
        });

        modelBuilder.Entity<TypeList>(entity =>
        {
            entity.HasKey(e => e.IdListType).HasName("PK__TypeList__61838F3DE356E3E7");

            entity.ToTable("TypeList");

            entity.Property(e => e.ListName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__User__B7C92638C4626B14");

            entity.ToTable("User");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Image).HasMaxLength(256);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.PasswordSalt).HasMaxLength(256);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
