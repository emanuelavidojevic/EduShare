using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduShareBackend.Api.Migrations
{
    /// <inheritdoc />
    public partial class DodatTypeUMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateRegistration",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 11, 19, 17, 47, 163, DateTimeKind.Local).AddTicks(84),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 11, 14, 45, 43, 337, DateTimeKind.Local).AddTicks(699));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Materials",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 11, 19, 17, 47, 151, DateTimeKind.Local).AddTicks(9387),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 11, 14, 45, 43, 326, DateTimeKind.Local).AddTicks(1273));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateComments",
                table: "Comments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 11, 19, 17, 47, 139, DateTimeKind.Local).AddTicks(8249),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 11, 14, 45, 43, 314, DateTimeKind.Local).AddTicks(1842));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateRegistration",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 11, 14, 45, 43, 337, DateTimeKind.Local).AddTicks(699),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 11, 19, 17, 47, 163, DateTimeKind.Local).AddTicks(84));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Materials",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 11, 14, 45, 43, 326, DateTimeKind.Local).AddTicks(1273),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 11, 19, 17, 47, 151, DateTimeKind.Local).AddTicks(9387));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateComments",
                table: "Comments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 11, 14, 45, 43, 314, DateTimeKind.Local).AddTicks(1842),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 11, 19, 17, 47, 139, DateTimeKind.Local).AddTicks(8249));
        }
    }
}
