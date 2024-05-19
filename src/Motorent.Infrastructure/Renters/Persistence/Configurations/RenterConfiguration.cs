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

        builder.Property(r => r.CNPJ)
            .HasMaxLength(18)
            .HasConversion(v => v.Value, v => CNPJ.Create(v).Value);

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

        builder.OwnsOne(v => v.CNH, b =>
        {
            b.HasIndex(c => c.Number)
                .IsUnique();

            b.Property(c => c.Number)
                .HasMaxLength(11);

            b.Property(c => c.Category)
                .HasMaxLength(5)
                .HasConversion(v => v.Name, v => CNHCategory.FromName(v, true));

            b.Property(c => c.ExpirationDate)
                .HasColumnName("cnh_exp");
        });

        builder.Property(r => r.CNHStatus)
            .HasMaxLength(20)
            .HasConversion(v => v.Name, v => CNHStatus.FromName(v, true));

        builder.Property(r => r.CNHImageUrl)
            .HasMaxLength(2048)
            .HasColumnName("cnh_image_url");
        
        builder.ToTable("renters");
    }
}