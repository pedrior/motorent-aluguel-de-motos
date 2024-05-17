using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Motorent.Infrastructure.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: "01HXJ32R78AMGH0CGPKPZRA0WB");

            migrationBuilder.AlterColumn<string>(
                name: "error",
                table: "outbox_messages",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "data",
                table: "outbox_messages",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(1)");

            migrationBuilder.CreateTable(
                name: "message_logs",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    identifier = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    data = table.Column<string>(type: "character varying(65536)", maxLength: 65536, nullable: false),
                    sent_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_message_logs", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "claims", "email", "password_hash", "roles" },
                values: new object[] { "01HY4E9RZNNQ6T1RZ4KRFSBNJM", new Dictionary<string, string> { ["given_name"] = "John", ["family_name"] = "Doe", ["birthdate"] = "2000-09-05" }, "john@admin.com", "bSgM8RqIft6Cj7o+zY9NCzKmRyKK0KwYuKi0LiP9w1E=:8jVDEfRE5J5BPerrhshZ9w==:50000:SHA256", new[] { "admin" } });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "message_logs");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: "01HY4E9RZNNQ6T1RZ4KRFSBNJM");

            migrationBuilder.AlterColumn<string>(
                name: "error",
                table: "outbox_messages",
                type: "character(1)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "data",
                table: "outbox_messages",
                type: "character(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "claims", "email", "password_hash", "roles" },
                values: new object[] { "01HXJ32R78AMGH0CGPKPZRA0WB", new Dictionary<string, string> { ["given_name"] = "John", ["family_name"] = "Doe", ["birthdate"] = "2000-09-05" }, "john@admin.com", "0p9n6E8pTwcJOjVe1IXdBw5nLkzwAii7N9eupIqm8vo=:fMaufrO/TnCt5h3ChfffUw==:50000:SHA256", new[] { "admin" } });
        }
    }
}
