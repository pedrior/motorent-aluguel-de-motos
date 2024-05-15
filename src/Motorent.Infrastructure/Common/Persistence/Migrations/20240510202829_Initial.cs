using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Motorent.Infrastructure.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    data = table.Column<string>(type: "character", maxLength: 8192, nullable: false),
                    error = table.Column<string>(type: "character", maxLength: 2048, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_messages", x => x.id);
                });

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

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    roles = table.Column<string[]>(type: "jsonb", nullable: true),
                    claims = table.Column<IDictionary<string, string>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "claims", "email", "password_hash", "roles" },
                values: new object[] { "01HXJ32R78AMGH0CGPKPZRA0WB", new Dictionary<string, string> { ["given_name"] = "John", ["family_name"] = "Doe", ["birthdate"] = "2000-09-05" }, "john@admin.com", "0p9n6E8pTwcJOjVe1IXdBw5nLkzwAii7N9eupIqm8vo=:fMaufrO/TnCt5h3ChfffUw==:50000:SHA256", new[] { "admin" } });

            migrationBuilder.CreateIndex(
                name: "ix_motorcycles_license_plate",
                table: "motorcycles",
                column: "license_plate",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "motorcycles");

            migrationBuilder.DropTable(
                name: "outbox_messages");

            migrationBuilder.DropTable(
                name: "renters");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
