using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Enums;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Infrastructure.Renters.Persistence.Configurations;

internal sealed class RenterConfiguration : IEntityTypeConfiguration<Renter>
{
    public void Configure(EntityTypeBuilder<Renter> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => r.UserId)
            .IsUnique();

        builder.HasIndex(r => r.Email)
            .IsUnique();

        builder.Property(r => r.Id)
            .HasMaxLength(26)
            .HasConversion(
                v => v.Value.ToString(),
                v => new RenterId(Ulid.Parse(v)));

        builder.Property(r => r.UserId)
            .HasMaxLength(26);

        builder.Property(r => r.Document)
            .HasMaxLength(18)
            .HasConversion(v => v.Value, v => Document.Create(v).Value);

        builder.Property(r => r.Email)
            .HasMaxLength(255)
            .HasConversion(v => v.Value, v => EmailAddress.Create(v).Value);

        builder.OwnsOne(r => r.FullName, b =>
        {
            b.Property(n => n.GivenName)
                .HasMaxLength(30)
                .HasColumnName("given_name");

            b.Property(n => n.FamilyName)
                .HasMaxLength(30)
                .HasColumnName("family_name");
        });

        builder.Property(r => r.Birthdate)
            .HasConversion(v => v.Value, v => Birthdate.Create(v).Value);

        builder.OwnsOne(v => v.DriverLicense, b =>
        {
            b.HasIndex(c => c.Number)
                .IsUnique();

            b.Property(c => c.Number)
                .HasMaxLength(11)
                .HasColumnName("dl_number");

            b.Property(c => c.Category)
                .HasMaxLength(5)
                .HasColumnName("dl_category")
                .HasConversion(
                    v => v.Name,
                    v => DriverLicenseCategory.FromName(v, true));

            b.Property(c => c.Expiry)
                .HasColumnName("dl_expiry");
        });

        builder.Property(r => r.DriverLicenseStatus)
            .HasMaxLength(20)
            .HasColumnName("dl_status")
            .HasConversion(
                v => v.Name,
                v => DriverLicenseStatus.FromName(v, true));

        builder.Property(r => r.DriverLicenseImageUrl)
            .HasMaxLength(2048)
            .HasColumnName("dl_image");
        
        builder.OwnsMany(r => r.RentalIds, b =>
        {
            b.HasKey("Id");
            
            b.Property(v => v.Value)
                .HasColumnName("rental_id")
                .HasColumnType("char(26)")
                .HasConversion(
                    v => v.ToString(),
                    v => Ulid.Parse(v))
                .ValueGeneratedNever();
            
            b.WithOwner()
                .HasForeignKey("renter_id");
            
            b.ToTable("renter_rentals");
        });

        builder.Metadata.FindNavigation(nameof(Renter.RentalIds))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.ToTable("renters");
    }
}