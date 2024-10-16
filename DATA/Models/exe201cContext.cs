﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DATA.Models;

public partial class exe201cContext : DbContext
{
    public exe201cContext(DbContextOptions<exe201cContext> options)
       : base(options)
    {
    }
    public exe201cContext()
    {
    }
    public static string GetConnectionString(string connectionStringName)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection"));
        }
    }


    public virtual DbSet<Membership> Memberships { get; set; }

    public virtual DbSet<MembershipPlan> MembershipPlans { get; set; }

    public virtual DbSet<MembershipPlanAssignment> MembershipPlanAssignments { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostType> PostTypes { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(e => e.MembershipId).HasName("PK__MEMBERSH__CAE49DDDEB5DF97B");

            entity.ToTable("MEMBERSHIPS");

            entity.Property(e => e.MembershipId).HasColumnName("membership_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Memberships)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__MEMBERSHI__user___5BE2A6F2");
        });

        modelBuilder.Entity<MembershipPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__MEMBERSH__BE9F8F1D34DFE364");

            entity.ToTable("MEMBERSHIP_PLANS");

            entity.HasIndex(e => e.Name, "UQ__MEMBERSH__72E12F1B43C85AB3").IsUnique();

            entity.Property(e => e.PlanId).HasColumnName("plan_id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.DurationDays).HasColumnName("duration_days");
            entity.Property(e => e.Features)
                .HasColumnType("text")
                .HasColumnName("features");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
        });

        modelBuilder.Entity<MembershipPlanAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__MEMBERSH__DA891814861EB6AB");

            entity.ToTable("MEMBERSHIP_PLAN_ASSIGNMENTS");

            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.MembershipId).HasColumnName("membership_id");
            entity.Property(e => e.PlanId).HasColumnName("plan_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Membership).WithMany(p => p.MembershipPlanAssignments)
                .HasForeignKey(d => d.MembershipId)
                .HasConstraintName("FK__MEMBERSHI__membe__5EBF139D");

            entity.HasOne(d => d.Plan).WithMany(p => p.MembershipPlanAssignments)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK__MEMBERSHI__plan___5FB337D6");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__POSTS__3ED78766A90A54DD");

            entity.ToTable("POSTS");

            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.BudgetOrSalary)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("budget_or_salary");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.ImgUrl)
                .HasColumnType("text")
                .HasColumnName("img_url");
            entity.Property(e => e.PostTypeId).HasColumnName("post_type_id");
            entity.Property(e => e.Skills)
                .HasColumnType("text")
                .HasColumnName("skills");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.PostType).WithMany(p => p.Posts)
                .HasForeignKey(d => d.PostTypeId)
                .HasConstraintName("FK__POSTS__post_type__5165187F");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__POSTS__user_id__5070F446");
        });

        modelBuilder.Entity<PostType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__POST_TYP__2C00059809CDDEC8");

            entity.ToTable("POST_TYPES");

            entity.HasIndex(e => e.TypeName, "UQ__POST_TYP__543C4FD99A637A46").IsUnique();

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.TypeName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type_name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__REVIEWS__60883D90D49D4245");

            entity.ToTable("REVIEWS");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ImgUrl)
                .HasColumnType("text")
                .HasColumnName("img_url");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.RevieweeId).HasColumnName("reviewee_id");
            entity.Property(e => e.ReviewerId).HasColumnName("reviewer_id");

            entity.HasOne(d => d.Post).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__REVIEWS__post_id__5441852A");

            entity.HasOne(d => d.Reviewee).WithMany(p => p.ReviewReviewees)
                .HasForeignKey(d => d.RevieweeId)
                .HasConstraintName("FK__REVIEWS__reviewe__5629CD9C");

            entity.HasOne(d => d.Reviewer).WithMany(p => p.ReviewReviewers)
                .HasForeignKey(d => d.ReviewerId)
                .HasConstraintName("FK__REVIEWS__reviewe__5535A963");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__USERS__B9BE370FCC887423");

            entity.ToTable("USERS");

            entity.HasIndex(e => e.Email, "UQ__USERS__AB6E6164BCC1F735").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__USERS__F3DBC572D574CF56").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ContactInfo)
                .HasColumnType("text")
                .HasColumnName("contact_info");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeliveryTime)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.ImgUrl)
                .HasColumnType("text")
                .HasColumnName("img_url");
            entity.Property(e => e.LanguageLevel)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Location)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.NumberJobDone).HasColumnName("numberJobDone");
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserType)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("user_type");
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}