using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.JsonWebTokens;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Rentals;
using Motorent.Domain.Renters;
using Motorent.Infrastructure.Common.Identity;
using Motorent.Infrastructure.Common.Outbox;

namespace Motorent.Infrastructure.Common.Persistence;

internal class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Motorcycle> Motorcycles => Set<Motorcycle>();
    
    public DbSet<Rental> Rentals => Set<Rental>();
    
    public DbSet<Renter> Renters => Set<Renter>();

    public DbSet<User> Users => Set<User>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseSnakeCaseNamingConvention();

        base.OnConfiguring(builder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<Uri>()
            .HaveConversion<UriToStringConverter>();

        base.ConfigureConventions(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

        // Apenas para fins de desenvolvimento
        builder.Entity<User>()
            .HasData(new User(
                email: "john@admin.com",
                passwordHash: PasswordHelper.Hash("John@123"),
                roles: ["admin"],
                claims: new Dictionary<string, string>
                {
                    [JwtRegisteredClaimNames.GivenName] = "John",
                    [JwtRegisteredClaimNames.FamilyName] = "Doe",
                    [JwtRegisteredClaimNames.Birthdate] = "2000-09-05"
                }));

        base.OnModelCreating(builder);
    }
}