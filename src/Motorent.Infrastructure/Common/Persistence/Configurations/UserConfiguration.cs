using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motorent.Infrastructure.Common.Identity;
using Motorent.Infrastructure.Common.Persistence.Configurations.Constants;

namespace Motorent.Infrastructure.Common.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Id)
            .HasMaxLength(UserConstants.IdMaxLength);

        builder.Property(u => u.Email)
            .HasConversion(v => v.ToLowerInvariant(), v => v)
            .HasMaxLength(UserConstants.EmailMaxLength);
        
        builder.Property(u => u.PasswordHash)
            .HasMaxLength(UserConstants.PasswordHashMaxLength);

        builder.Property(u => u.Roles)
            .HasColumnType("jsonb");

        builder.Property(u => u.Claims)
            .HasColumnType("jsonb");
        
        builder.ToTable(UserConstants.TableName);
    }
}