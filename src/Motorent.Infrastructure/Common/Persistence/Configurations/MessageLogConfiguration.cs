using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motorent.Infrastructure.Common.Messaging;

namespace Motorent.Infrastructure.Common.Persistence.Configurations;

internal sealed class MessageLogConfiguration : IEntityTypeConfiguration<MessageLog>
{
    public void Configure(EntityTypeBuilder<MessageLog> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Id)
            .HasMaxLength(26)
            .HasConversion(
                v => v.ToString(),
                s => Ulid.Parse(s));

        builder.Property(m => m.Identifier)
            .HasMaxLength(36);

        builder.Property(m => m.Name)
            .HasMaxLength(255);

        builder.Property(m => m.Data)
            .HasMaxLength(65536);
        
        builder.ToTable("message_logs");
    }
}