using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduShareBackend.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMaterialType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateRegistration",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 11, 19, 21, 0, 565, DateTimeKind.Local).AddTicks(5693),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 11, 19, 17, 47, 163, DateTimeKind.Local).AddTicks(84));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Materials",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 11, 19, 21, 0, 553, DateTimeKind.Local).AddTicks(2635),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 11, 19, 17, 47, 151, DateTimeKind.Local).AddTicks(9387));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateComments",
                table: "Comments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 11, 19, 21, 0, 544, DateTimeKind.Local).AddTicks(2723),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 11, 19, 17, 47, 139, DateTimeKind.Local).AddTicks(8249));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateRegistration",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 11, 19, 17, 47, 163, DateTimeKind.Local).AddTicks(84),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 11, 19, 21, 0, 565, DateTimeKind.Local).AddTicks(5693));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Materials",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 11, 19, 17, 47, 151, DateTimeKind.Local).AddTicks(9387),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 11, 19, 21, 0, 553, DateTimeKind.Local).AddTicks(2635));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateComments",
                table: "Comments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 11, 19, 17, 47, 139, DateTimeKind.Local).AddTicks(8249),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 11, 19, 21, 0, 544, DateTimeKind.Local).AddTicks(2723));
        }
    }
}
