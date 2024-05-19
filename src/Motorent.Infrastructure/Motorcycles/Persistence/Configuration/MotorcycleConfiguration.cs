using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Infrastructure.Motorcycles.Persistence.Configuration;

internal sealed class MotorcycleConfiguration : IEntityTypeConfiguration<Motorcycle>
{
    public void Configure(EntityTypeBuilder<Motorcycle> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasIndex(m => m.LicensePlate)
            .IsUnique();

        builder.Property(m => m.Id)
            .HasMaxLength(26)
            .HasConversion(
                v => v.Value.ToString(),
                v => new MotorcycleId(Ulid.Parse(v)));

        builder.Property(u => u.Model)
            .HasMaxLength(30);
        
        builder.Property(u => u.Year)
            .HasConversion(
                v => v.Value,
                v => Year.Create(v).Value);
        
        builder.Property(u => u.LicensePlate)
            .HasMaxLength(7)
            .HasConversion(
                v => v.Value,
                v => LicensePlate.Create(v).Value);
        
        builder.ToTable("motorcycles");
    }
}