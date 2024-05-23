using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals;
using Motorent.Domain.Rentals.Enums;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Infrastructure.Rentals.Persistence.Configuration;

internal sealed class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasMaxLength(26)
            .HasConversion(
                v => v.Value.ToString(),
                s => new RentalId(Ulid.Parse(s)));
        
        builder.Property(r => r.RenterId)
            .HasMaxLength(26)
            .HasConversion(
                v => v.Value.ToString(),
                s => new RenterId(Ulid.Parse(s)));
        
        builder.Property(r => r.MotorcycleId)
            .HasMaxLength(26)
            .HasConversion(
                v => v.Value.ToString(),
                s => new MotorcycleId(Ulid.Parse(s)));
        
        builder.Property(r => r.Plan)
            .HasMaxLength(20)
            .HasConversion(
                v => v.Name,
                n => RentalPlan.FromName(n, true));

        builder.OwnsOne(r => r.Period, b =>
        {
            b.Property(p => p.Start)
                .HasColumnName("start");
            
            b.Property(p => p.End)
                .HasColumnName("end");
        });

        builder.Property(r => r.Penalty)
            .HasConversion(
                v => v.Value,
                d => new Money(d));

        builder.ToTable("rentals");
    }
}