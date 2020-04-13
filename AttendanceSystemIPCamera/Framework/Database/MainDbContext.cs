using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.Database
{
    public class MainDbContext: DbContext
    {
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<AttendeeGroup> AttendeeGroups { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<ChangeRequest> ChangeRequests { get; set; }

        public MainDbContext(DbContextOptions<MainDbContext> options): base(options)
        { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attendee>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code).HasColumnType("varchar ( 50 )");

                entity.Property(e => e.Name).HasColumnType("varchar ( 255 )");
            });

            modelBuilder.Entity<AttendeeGroup>(entity =>
            {
                entity.HasIndex(e => new { e.AttendeeCode, e.GroupCode })
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.AttendeeCode)
                    .IsRequired()
                    .HasColumnType("varchar ( 50 )");

                entity.Property(e => e.GroupCode)
                    .IsRequired()
                    .HasColumnType("varchar ( 50 )");

                entity.Property(e => e.IsActive).HasDefaultValueSql("1");

                entity.HasOne(d => d.Attendee)
                    .WithMany(p => p.AttendeeGroups)
                    .HasForeignKey(d => d.AttendeeCode)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.AttendeeGroups)
                    .HasForeignKey(d => d.GroupCode)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ChangeRequest>(entity =>
            {
                entity.HasIndex(e => e.RecordId)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Comment).HasColumnType("varchar ( 255 )");

                entity.Property(e => e.RecordId).HasColumnType("int");

                entity.Property(e => e.Status).HasColumnType("int");

                entity.HasOne(d => d.Record)
                    .WithOne(p => p.ChangeRequest)
                    .HasForeignKey<ChangeRequest>(d => d.RecordId);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code).HasColumnType("varchar ( 50 )");

                entity.Property(e => e.DateTimeCreated).HasColumnType("varchar ( 255 )");

                entity.Property(e => e.Deleted).HasDefaultValueSql("0");

                entity.Property(e => e.Name).HasColumnType("varchar ( 100 )");
            });

            modelBuilder.Entity<Record>(entity =>
            {
                entity.HasIndex(e => new { e.AttendeeGroupId, e.SessionId })
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.AttendeeCode)
                    .IsRequired()
                    .HasColumnType("varchar ( 50 )");

                entity.Property(e => e.EndTime)
                    .IsRequired()
                    .HasColumnType("varchar ( 255 )");

                entity.Property(e => e.SessionName)
                    .IsRequired()
                    .HasColumnType("varchar ( 255 )");

                entity.Property(e => e.StartTime)
                    .IsRequired()
                    .HasColumnType("varchar ( 255 )");

                entity.Property(e => e.UpdateTime).HasColumnType("varchar ( 255 )");

                entity.HasOne(d => d.AttendeeGroup)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.AttendeeGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            
            modelBuilder.Entity<Session>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.EndTime)
                    .IsRequired()
                    .HasColumnType("varchar ( 255 )");

                entity.Property(e => e.GroupCode)
                    .IsRequired()
                    .HasColumnType("varchar ( 50 )");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("nvarchar ( 50 )");

                entity.Property(e => e.StartTime)
                    .IsRequired()
                    .HasColumnType("varchar ( 255 )");

                entity.Property(e => e.Status).HasColumnType("varchar ( 50 )");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.GroupCode)
                    .OnDelete(DeleteBehavior.ClientSetNull);

            });
        }

    }
}
