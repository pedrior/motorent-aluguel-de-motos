using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Motorent.Infrastructure.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Motorcycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: "01HWK4ZX63YBPQGECPPP9TJZ9W");

            migrationBuilder.CreateTable(
                name: "motorcycles",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    model = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    brand = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    daily_price = table.Column<decimal>(type: "numeric", nullable: false),
                    license_plate = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_motorcycles", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "claims", "email", "password_hash", "roles" },
                values: new object[] { "01HWP53QGGBC4RKY96EY9CSEHD", new Dictionary<string, string> { ["given_name"] = "John", ["family_name"] = "Doe", ["birthdate"] = "2000-09-05" }, "john@admin.com", "ceQA+E7mEpX8zmZXGvrZNyGYWZeNdeshN74Jl/Z7yg4=:ypAcjejRbbbZdav0oErfog==:50000:SHA256", new[] { "admin" } });

            migrationBuilder.CreateIndex(
                name: "ix_motorcycles_license_plate",
                table: "motorcycles",
                column: "license_plate",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "motorcycles");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: "01HWP53QGGBC4RKY96EY9CSEHD");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "claims", "email", "password_hash", "roles" },
                values: new object[] { "01HWK4ZX63YBPQGECPPP9TJZ9W", new Dictionary<string, string> { ["given_name"] = "John", ["family_name"] = "Doe", ["birthdate"] = "2000-09-05" }, "john@admin.com", "btA5LPKpTsNiUys+q4QskCl0vuzSDD/iyVcRxwCXWHE=:sUjicLxNXpieNsx++rNjpQ==:50000:SHA256", new[] { "admin" } });
        }
    }
}
