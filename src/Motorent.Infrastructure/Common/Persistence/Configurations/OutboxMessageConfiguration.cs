using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
        
        builder.Property(o => o.ErrorType)
            .HasMaxLength(OutboxMessageConstants.ErrorTypeMaxLength);
        
        builder.Property(o => o.ErrorMessage)
            .HasMaxLength(OutboxMessageConstants.ErrorMessageMaxLength);
        
        builder.Property(o => o.ErrorDetails)
            .HasMaxLength(OutboxMessageConstants.ErrorDetailsMaxLength);

        builder.Property(o => o.Status)
            .HasConversion<EnumToStringConverter<OutboxMessageStatus>>()
            .HasMaxLength(OutboxMessageConstants.StatusMaxLength);
        
        builder.ToTable(OutboxMessageConstants.TableName);
    }
}