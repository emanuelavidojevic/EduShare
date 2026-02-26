using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduShareBackend.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectIdToMaterials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateRegistration",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 11, 14, 45, 43, 337, DateTimeKind.Local).AddTicks(699),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 10, 10, 23, 35, 772, DateTimeKind.Local).AddTicks(2563));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Materials",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 11, 14, 45, 43, 326, DateTimeKind.Local).AddTicks(1273),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 10, 10, 23, 35, 762, DateTimeKind.Local).AddTicks(5355));

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateComments",
                table: "Comments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 11, 14, 45, 43, 314, DateTimeKind.Local).AddTicks(1842),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 10, 10, 23, 35, 745, DateTimeKind.Local).AddTicks(1428));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Materials");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateRegistration",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 10, 10, 23, 35, 772, DateTimeKind.Local).AddTicks(2563),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 11, 14, 45, 43, 337, DateTimeKind.Local).AddTicks(699));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Materials",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 10, 10, 23, 35, 762, DateTimeKind.Local).AddTicks(5355),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 11, 14, 45, 43, 326, DateTimeKind.Local).AddTicks(1273));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateComments",
                table: "Comments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 10, 10, 23, 35, 745, DateTimeKind.Local).AddTicks(1428),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 2, 11, 14, 45, 43, 314, DateTimeKind.Local).AddTicks(1842));
        }
    }
}
