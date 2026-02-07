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

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<Policy> Policies { get; set; }

    public virtual DbSet<PremiumRule> PremiumRules { get; set; }

    public virtual DbSet<RiskCategory> RiskCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Broker>(entity =>
        {
            entity.ToTable("Broker", "core");

            entity.HasIndex(e => e.BrokerKey, "UQ_Broker_BrokerKey").IsUnique();

            entity.HasIndex(e => e.Code, "UQ_Broker_Code").IsUnique();

            entity.Property(e => e.BrokerKey).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CommissionPercentage).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);
        });

        modelBuilder.Entity<Building>(entity =>
        {
            entity.ToTable("Building", "core");

            entity.HasIndex(e => e.CityId, "IX_Building_CityId");

            entity.HasIndex(e => e.OwnerClientId, "IX_Building_OwnerClientKey");

            entity.HasIndex(e => e.BuildingKey, "UQ_Building_BuildingKey").IsUnique();

            entity.Property(e => e.BuildingKey).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.BuildingType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsuredValueAmount).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.InsuredValueCurrencyCode).HasMaxLength(10);
            entity.Property(e => e.Number).HasMaxLength(20);
            entity.Property(e => e.Street).HasMaxLength(100);

            entity.HasOne(d => d.City).WithMany(p => p.Buildings)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Building_City");

            entity.HasOne(d => d.InsuredValueCurrencyCodeNavigation).WithMany(p => p.Buildings)
                .HasForeignKey(d => d.InsuredValueCurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Building_InsuredValueCurrency");

            entity.HasOne(d => d.OwnerClient).WithMany(p => p.Buildings)
                .HasPrincipalKey(p => p.ClientKey)
                .HasForeignKey(d => d.OwnerClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Building_Client");

            entity.HasMany(d => d.RiskCategories).WithMany(p => p.BuildingKeys)
                .UsingEntity<Dictionary<string, object>>(
                    "RiskBuilding",
                    r => r.HasOne<RiskCategory>().WithMany()
                        .HasForeignKey("RiskCategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RiskBuilding_RiskCategory"),
                    l => l.HasOne<Building>().WithMany()
                        .HasPrincipalKey("BuildingKey")
                        .HasForeignKey("BuildingKey")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RiskBuilding_Building"),
                    j =>
                    {
                        j.HasKey("BuildingKey", "RiskCategoryId");
                        j.ToTable("RiskBuilding", "core");
                        j.HasIndex(new[] { "RiskCategoryId" }, "IX_RiskBuilding_RiskCategoryId");
                    });
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("City", "core");

            entity.HasIndex(e => e.CountyId, "IX_City_CountyId");

            entity.HasIndex(e => new { e.CountyId, e.Name }, "UQ_City").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.County).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_County");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Client", "core");

            entity.HasIndex(e => e.ClientKey, "UQ_Client_ClientKey").IsUnique();

            entity.HasIndex(e => e.IdentificationNumber, "UQ_Client_IdentificationNumber").IsUnique();

            entity.Property(e => e.ClientKey).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.ClientType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IdentificationNumber).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Number).HasMaxLength(20);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Street).HasMaxLength(100);
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("Country", "core");

            entity.HasIndex(e => e.Name, "UQ_Country_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<County>(entity =>
        {
            entity.ToTable("County", "core");

            entity.HasIndex(e => e.CountryId, "IX_County_CountryId");

            entity.HasIndex(e => new { e.CountryId, e.Name }, "UQ_County").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Country).WithMany(p => p.Counties)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_County_Country");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.ToTable("Currency", "core");

            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.ExchangeRateToBase).HasColumnType("decimal(19, 8)");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.ToTable("Policy", "core");

            entity.HasIndex(e => e.BrokerKey, "IX_Policy_BrokerKey");

            entity.HasIndex(e => e.BuildingKey, "IX_Policy_BuildingKey");

            entity.HasIndex(e => e.ClientKey, "IX_Policy_ClientKey");

            entity.HasIndex(e => new { e.Status, e.StartDate, e.EndDate }, "IX_Policy_Status_Dates");

            entity.HasIndex(e => e.PolicyNumber, "UQ_Policy_PolicyNumber").IsUnique();

            entity.Property(e => e.BasePremiumAmount).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.CancellationReason).HasMaxLength(200);
            entity.Property(e => e.CurrencyCode).HasMaxLength(10);
            entity.Property(e => e.FinalPremiumAmount).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.PolicyNumber).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.BrokerKeyNavigation).WithMany(p => p.Policies)
                .HasPrincipalKey(p => p.BrokerKey)
                .HasForeignKey(d => d.BrokerKey)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Policy_Broker");

            entity.HasOne(d => d.BuildingKeyNavigation).WithMany(p => p.Policies)
                .HasPrincipalKey(p => p.BuildingKey)
                .HasForeignKey(d => d.BuildingKey)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Policy_Building");

            entity.HasOne(d => d.ClientKeyNavigation).WithMany(p => p.Policies)
                .HasPrincipalKey(p => p.ClientKey)
                .HasForeignKey(d => d.ClientKey)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Policy_Client");

            entity.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.Policies)
                .HasForeignKey(d => d.CurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Policy_Currency");
        });

        modelBuilder.Entity<PremiumRule>(entity =>
        {
            entity.ToTable("PremiumRules", "core");

            entity.HasIndex(e => new { e.RuleKind, e.IsActive }, "IX_PremiumRules_RuleKind_Active");

            entity.HasIndex(e => e.PremiumRuleKey, "UQ_PremiumRules_PremiumRuleKey").IsUnique();

            entity.Property(e => e.BuildingType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FeeType)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Percentage).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.PremiumRuleKey).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.RuleKind)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ZoneRiskCategoryCode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.City).WithMany(p => p.PremiumRules)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_PremiumRules_City");

            entity.HasOne(d => d.Country).WithMany(p => p.PremiumRules)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("FK_PremiumRules_Country");

            entity.HasOne(d => d.County).WithMany(p => p.PremiumRules)
                .HasForeignKey(d => d.CountyId)
                .HasConstraintName("FK_PremiumRules_County");

            entity.HasOne(d => d.ZoneRiskCategoryCodeNavigation).WithMany(p => p.PremiumRules)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.ZoneRiskCategoryCode)
                .HasConstraintName("FK_PremiumRules_ZoneRiskCategoryCode");
        });

        modelBuilder.Entity<RiskCategory>(entity =>
        {
            entity.ToTable("RiskCategory", "core");

            entity.HasIndex(e => e.Code, "UQ_RiskCategory_Code").IsUnique();

            entity.HasIndex(e => e.Name, "UQ_RiskCategory_Name").IsUnique();

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
