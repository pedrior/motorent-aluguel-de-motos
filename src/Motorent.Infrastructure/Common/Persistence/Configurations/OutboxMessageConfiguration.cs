using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motorent.Infrastructure.Common.Outbox;
using Motorent.Infrastructure.Common.Persistence.Configurations.Constants;

namespace Motorent.Infrastructure.Common.Persistence.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        builder.Property(o => o.Type)
            .HasMaxLength(OutboxMessageConstants.TypeMaxLength);
        
        builder.Property(o => o.Data)
            .HasMaxLength(OutboxMessageConstants.DataMaxLength);

        builder.Property(o => o.Error)
            .HasMaxLength(OutboxMessageConstants.ErrorMaxLength);

        builder.ToTable(OutboxMessageConstants.TableName);
        
        builder.Property<uint>("version")
            .IsRowVersion();
    }
}