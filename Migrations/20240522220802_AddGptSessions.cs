using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class AddGptSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SchedulerGptSessions",
                columns: table => new
                {
                    ThreadId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    DeskId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduleStartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConversationState = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValue: "NotCreated")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulerGptSessions", x => x.ThreadId);
                    table.ForeignKey(
                        name: "FK_SchedulerGptSessions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerGptSessions_EmployeeId",
                table: "SchedulerGptSessions",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchedulerGptSessions");
        }
    }
}
