using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Processes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Strategy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentStep = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutoScheduleProcesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ProcessStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileWindowEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublishDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoScheduleProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutoScheduleProcesses_Processes_Id",
                        column: x => x.Id,
                        principalTable: "Processes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoScheduleProcesses");

            migrationBuilder.DropTable(
                name: "Processes");
        }
    }
}
