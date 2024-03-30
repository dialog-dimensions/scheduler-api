using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class AutoProcessTableGetsScheduleAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Strategy",
                table: "Processes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
            
            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduleStart",
                table: "AutoScheduleProcesses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            
            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduleEnd",
                table: "AutoScheduleProcesses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ScheduleShiftDuration",
                table: "AutoScheduleProcesses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduleEnd",
                table: "AutoScheduleProcesses");

            migrationBuilder.DropColumn(
                name: "ScheduleShiftDuration",
                table: "AutoScheduleProcesses");

            migrationBuilder.DropColumn(
                name: "ScheduleStart",
                table: "AutoScheduleProcesses");

            migrationBuilder.AlterColumn<string>(
                name: "Strategy",
                table: "Processes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
