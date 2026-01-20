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

    public virtual DbSet<Building> Buildings { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<County> Counties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Building>(entity =>
        {
            entity.HasKey(e => e.BuildingId).HasName("PK__Building__5463CDC43DF497B4");

            entity.ToTable("Building", "core");

            entity.HasIndex(e => e.BuildingKey, "UQ__Building__94E7067EB2C4F6A2").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.BuildingType).HasMaxLength(50);

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

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.ClientType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.RegistrationNumber).HasMaxLength(50);
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
