using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using workshopManagementSystem.Domain;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace workshopManagementSystem.Razor
{
    public partial class MyDbContext : DbContext
    {
        public MyDbContext()
        {
        }

        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Admin> Admins { get; set; } = null!;

        public virtual DbSet<Car> Cars { get; set; } = null!;
        public virtual DbSet<Mechanic> Mechanics { get; set; } = null!;
        public virtual DbSet<Service> Services { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //You can add your connection string here if needed
                optionsBuilder.UseMySql("Server=localhost;Port=3306;Database=WorkShopDBProject;Uid=root;Pwd=123;",
                    ServerVersion.AutoDetect("Server=localhost;Port=3306;Database=WorkShopDBProject;Uid=root;Pwd=123;"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerFullName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerEmailAddress)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            // Configure Car entity
            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(e => e.VehicleLicenseNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                // Relationships
                entity.HasOne(c => c.VehicleOwner)
                      .WithMany(cu => cu.CustomerVehicles)
                      .HasForeignKey(c => c.CustomerAccountId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Mechanic entity
            modelBuilder.Entity<Mechanic>(entity =>
            {
                entity.Property(e => e.TechnicianFullName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            // Configure Service entity
            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(e => e.ServiceWorkDescription)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                // Relationships
                entity.HasOne(s => s.ServiceVehicle)
                      .WithMany(c => c.VehicleServiceHistory)
                      .HasForeignKey(s => s.VehicleRegistrationId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.ServiceTechnician)
                      .WithMany(m => m.TechnicianServiceHistory)
                      .HasForeignKey(s => s.TechnicianAccountId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
} 