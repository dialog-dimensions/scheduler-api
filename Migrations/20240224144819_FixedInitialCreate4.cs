using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class FixedInitialCreate4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(225)", nullable: false),
                    Balance = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    DifficultBalance = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduleKey = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    ModificationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    ModificationUser = table.Column<string>(type: "nvarchar(225)", nullable: false, defaultValue: "Computer")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.StartDateTime);
                    table.ForeignKey(
                        name: "FK_Shifts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShiftExceptions",
                columns: table => new
                {
                    ShiftKey = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ExceptionType = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModificationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    ModificationUser = table.Column<string>(type: "nvarchar(225)", nullable: false, defaultValue: "Computer")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftExceptions", x => new { x.ShiftKey, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_ShiftExceptions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShiftExceptions_Shifts_ShiftKey",
                        column: x => x.ShiftKey,
                        principalTable: "Shifts",
                        principalColumn: "StartDateTime",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftSwaps",
                columns: table => new
                {
                    SwapId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftKey = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreviousEmployeeId = table.Column<int>(type: "int", nullable: false),
                    ModificationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    ModificationUser = table.Column<string>(type: "nvarchar(225)", nullable: false, defaultValue: "Computer")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftSwaps", x => x.SwapId);
                    table.ForeignKey(
                        name: "FK_ShiftSwaps_Employees_PreviousEmployeeId",
                        column: x => x.PreviousEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShiftSwaps_Shifts_ShiftKey",
                        column: x => x.ShiftKey,
                        principalTable: "Shifts",
                        principalColumn: "StartDateTime",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Name",
                table: "Employees",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftExceptions_EmployeeId",
                table: "ShiftExceptions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_EmployeeId",
                table: "Shifts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSwaps_PreviousEmployeeId",
                table: "ShiftSwaps",
                column: "PreviousEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSwaps_ShiftKey",
                table: "ShiftSwaps",
                column: "ShiftKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftExceptions");

            migrationBuilder.DropTable(
                name: "ShiftSwaps");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
