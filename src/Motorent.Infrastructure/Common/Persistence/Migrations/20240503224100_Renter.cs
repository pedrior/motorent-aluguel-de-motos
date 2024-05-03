using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Motorent.Infrastructure.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Renter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: "01HWP53QGGBC4RKY96EY9CSEHD");

            migrationBuilder.CreateTable(
                name: "renters",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    user_id = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    given_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    family_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    birthdate = table.Column<DateOnly>(type: "date", nullable: false),
                    cnh_number = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    cnh_category = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    cnh_exp = table.Column<DateOnly>(type: "date", nullable: false),
                    cnh_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cnh_front_img_url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    cnh_back_img_url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_renters", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "claims", "email", "password_hash", "roles" },
                values: new object[] { "01HX09WBR16929RWN4RTJHMWD9", new Dictionary<string, string> { ["given_name"] = "John", ["family_name"] = "Doe", ["birthdate"] = "2000-09-05" }, "john@admin.com", "4hHwCWvHISBng4NmGp8v8Byd1+88t20zhYnaP0MTb/k=:0w1cbItzUkOlwC6a1iCg4Q==:50000:SHA256", new[] { "admin" } });

            migrationBuilder.CreateIndex(
                name: "ix_renters_cnh_number",
                table: "renters",
                column: "cnh_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_renters_email",
                table: "renters",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_renters_user_id",
                table: "renters",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "renters");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: "01HX09WBR16929RWN4RTJHMWD9");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "claims", "email", "password_hash", "roles" },
                values: new object[] { "01HWP53QGGBC4RKY96EY9CSEHD", new Dictionary<string, string> { ["given_name"] = "John", ["family_name"] = "Doe", ["birthdate"] = "2000-09-05" }, "john@admin.com", "ceQA+E7mEpX8zmZXGvrZNyGYWZeNdeshN74Jl/Z7yg4=:ypAcjejRbbbZdav0oErfog==:50000:SHA256", new[] { "admin" } });
        }
    }
}
