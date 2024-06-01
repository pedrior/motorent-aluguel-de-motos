using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Motorent.Infrastructure.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRenterRentalIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: "01HZ3H8RXGJMPPDJ0HGT1C0QHF");

            migrationBuilder.CreateTable(
                name: "renter_rentals",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    renter_id = table.Column<string>(type: "character varying(26)", nullable: true),
                    rental_id = table.Column<string>(type: "char(26)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_renter_rentals", x => x.id);
                    table.ForeignKey(
                        name: "fk_renter_rentals_renters_renter_id",
                        column: x => x.renter_id,
                        principalTable: "renters",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "claims", "email", "password_hash", "roles" },
                values: new object[] { "01HZ9TR1HYY83RTB891RY7BFAM", new Dictionary<string, string> { ["given_name"] = "John", ["family_name"] = "Doe", ["birthdate"] = "2000-09-05" }, "john@admin.com", "fh1kX6tZmXG1mSR9mj5BBf6wvOthfUH/kmyTUhffGaQ=:1iCcbD3NdzpPtoqdIqWb5w==:50000:SHA256", new[] { "admin" } });

            migrationBuilder.CreateIndex(
                name: "ix_renter_rentals_renter_id",
                table: "renter_rentals",
                column: "renter_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "renter_rentals");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: "01HZ9TR1HYY83RTB891RY7BFAM");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "claims", "email", "password_hash", "roles" },
                values: new object[] { "01HZ3H8RXGJMPPDJ0HGT1C0QHF", new Dictionary<string, string> { ["given_name"] = "John", ["family_name"] = "Doe", ["birthdate"] = "2000-09-05" }, "john@admin.com", "PHTA8RCw/S9FdQVK4oDfMR1N41ldki9sd0AtIMwZ1zE=:foQLiPZcUD3Sx8G16tuLow==:50000:SHA256", new[] { "admin" } });
        }
    }
}
