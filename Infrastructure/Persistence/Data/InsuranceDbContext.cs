using System;
using System.Collections.Generic;
using Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Data;

public partial class InsuranceDbContext : DbContext
{
    public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Broker> Brokers { get; set; }

    public virtual DbSet<Building> Buildings { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<County> Counties { get; set; }

    public virtual DbSet<Policy> Policies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Broker>(entity =>
        {
            entity.HasKey(e => e.BrokerId).HasName("PK__Broker__5D1D9A50541B3E14");

            entity.ToTable("Broker", "core");

            entity.HasIndex(e => e.BrokerKey, "UQ__Broker__F550972FCE41AACA").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Building>(entity =>
        {
            entity.HasKey(e => e.BuildingId).HasName("PK__Building__5463CDC43DF497B4");

            entity.ToTable("Building", "core");

            entity.HasIndex(e => e.BuildingKey, "UQ__Building__94E7067EB2C4F6A2").IsUnique();

            entity.Property(e => e.BuildingType).HasMaxLength(50);
            entity.Property(e => e.InsuredValueCurrency)
                .HasMaxLength(10)
                .HasDefaultValue("RON");
            entity.Property(e => e.Number).HasMaxLength(20);
            entity.Property(e => e.Street).HasMaxLength(100);

            entity.HasOne(d => d.City).WithMany(p => p.Buildings)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_city");

            entity.HasOne(d => d.Client).WithMany(p => p.Buildings)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_client");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__City__F2D21B76220E30EA");

            entity.ToTable("City", "core");

            entity.HasIndex(e => new { e.Name, e.CountyId }, "uq_city").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.County).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_county");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__Client__E67E1A2429E6C2F3");

            entity.ToTable("Client", "core");

            entity.HasIndex(e => e.ClientKey, "UQ__Client__E6AEDDB425F81C3D").IsUnique();

            entity.HasIndex(e => e.RegistrationNumber, "UQ__Client__E886460267752FE1").IsUnique();

            entity.Property(e => e.ClientType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Number).HasMaxLength(20);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.RegistrationNumber).HasMaxLength(50);
            entity.Property(e => e.Street).HasMaxLength(100);
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__Country__10D1609F5BAD38DF");

            entity.ToTable("Country", "core");

            entity.HasIndex(e => e.Name, "UQ__Country__737584F6250AE12B").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<County>(entity =>
        {
            entity.HasKey(e => e.CountyId).HasName("PK__County__B68F9D97254117E8");

            entity.ToTable("County", "core");

            entity.HasIndex(e => new { e.CountryId, e.Name }, "uq_county").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Country).WithMany(p => p.Counties)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_country");
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(e => e.PolicyId).HasName("PK__Policy__2E1339A4D9C0B346");

            entity.ToTable("Policy", "core");

            entity.HasIndex(e => e.PolicyKey, "UQ__Policy__7DF846F41AC2A907").IsUnique();

            entity.Property(e => e.PremiumAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PremiumCurrency).HasMaxLength(10);

            entity.HasOne(d => d.Broker).WithMany(p => p.Policies)
                .HasForeignKey(d => d.BrokerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_policy_broker");

            entity.HasOne(d => d.Building).WithMany(p => p.Policies)
                .HasForeignKey(d => d.BuildingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_policy_building");

            entity.HasOne(d => d.Client).WithMany(p => p.Policies)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_policy_client");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
