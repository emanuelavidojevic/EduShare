using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduShareBackend.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIsReadToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Notifications",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateComments",
                table: "Comments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 6, 17, 28, 27, 506, DateTimeKind.Local).AddTicks(38),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 3, 6, 14, 28, 47, 559, DateTimeKind.Local).AddTicks(8762));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Notifications");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateComments",
                table: "Comments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 6, 14, 28, 47, 559, DateTimeKind.Local).AddTicks(8762),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 3, 6, 17, 28, 27, 506, DateTimeKind.Local).AddTicks(38));
        }
    }
}
