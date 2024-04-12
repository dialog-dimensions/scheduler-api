using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationalEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShiftExceptions_Shifts_ShiftKey",
                table: "ShiftExceptions");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftSwaps_Shifts_ShiftKey",
                table: "ShiftSwaps");

            migrationBuilder.DropIndex(
                name: "IX_ShiftSwaps_ShiftKey",
                table: "ShiftSwaps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShiftExceptions",
                table: "ShiftExceptions");

            migrationBuilder.RenameColumn(
                name: "ShiftKey",
                table: "ShiftSwaps",
                newName: "ShiftStart");

            migrationBuilder.RenameColumn(
                name: "ScheduleKey",
                table: "Shifts",
                newName: "ScheduleStartDateTime");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_ScheduleKey",
                table: "Shifts",
                newName: "IX_Shifts_ScheduleStartDateTime");

            migrationBuilder.RenameColumn(
                name: "ShiftKey",
                table: "ShiftExceptions",
                newName: "ShiftStartDateTime");

            migrationBuilder.AddColumn<string>(
                name: "DeskId",
                table: "ShiftSwaps",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeskId",
                table: "Shifts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeskId",
                table: "ShiftExceptions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UnitId",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeskId",
                table: "AutoScheduleProcesses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts",
                columns: new[] { "DeskId", "StartDateTime" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShiftExceptions",
                table: "ShiftExceptions",
                columns: new[] { "DeskId", "ShiftStartDateTime", "EmployeeId" });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentUnitId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Units_Units_ParentUnitId",
                        column: x => x.ParentUnitId,
                        principalTable: "Units",
                        principalColumn: "Id");
                });
            
            migrationBuilder.Sql("INSERT INTO Units (Id, Name) VALUES ('1', 'Default')");
            migrationBuilder.Sql("UPDATE Employees SET UnitId = '1'");

            migrationBuilder.CreateTable(
                name: "Desks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Desks_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id");
                });
            
            migrationBuilder.Sql("INSERT INTO Desks (Id, Name, UnitId, Active) VALUES ('1', 'Default', '1', 1)");
            migrationBuilder.Sql("UPDATE AutoScheduleProcesses SET DeskId = '1'");
            migrationBuilder.Sql("UPDATE ShiftExceptions SET DeskId = '1'");
            migrationBuilder.Sql("UPDATE Shifts SET DeskId = '1'");

            migrationBuilder.CreateTable(
                name: "DeskAssignments",
                columns: table => new
                {
                    DeskId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeskAssignments", x => new { x.DeskId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_DeskAssignments_Desks_DeskId",
                        column: x => x.DeskId,
                        principalTable: "Desks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeskAssignments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSwaps_DeskId_ShiftStart",
                table: "ShiftSwaps",
                columns: new[] { "DeskId", "ShiftStart" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UnitId",
                table: "Employees",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoScheduleProcesses_DeskId",
                table: "AutoScheduleProcesses",
                column: "DeskId");

            migrationBuilder.CreateIndex(
                name: "IX_DeskAssignments_DeskId",
                table: "DeskAssignments",
                column: "DeskId");

            migrationBuilder.CreateIndex(
                name: "IX_DeskAssignments_EmployeeId",
                table: "DeskAssignments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Desks_UnitId",
                table: "Desks",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_ParentUnitId",
                table: "Units",
                column: "ParentUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_AutoScheduleProcesses_Desks_DeskId",
                table: "AutoScheduleProcesses",
                column: "DeskId",
                principalTable: "Desks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Units_UnitId",
                table: "Employees",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftExceptions_Desks_DeskId",
                table: "ShiftExceptions",
                column: "DeskId",
                principalTable: "Desks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftExceptions_Shifts_DeskId_ShiftStartDateTime",
                table: "ShiftExceptions",
                columns: new[] { "DeskId", "ShiftStartDateTime" },
                principalTable: "Shifts",
                principalColumns: new[] { "DeskId", "StartDateTime" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Desks_DeskId",
                table: "Shifts",
                column: "DeskId",
                principalTable: "Desks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftSwaps_Desks_DeskId",
                table: "ShiftSwaps",
                column: "DeskId",
                principalTable: "Desks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftSwaps_Shifts_DeskId_ShiftStart",
                table: "ShiftSwaps",
                columns: new[] { "DeskId", "ShiftStart" },
                principalTable: "Shifts",
                principalColumns: new[] { "DeskId", "StartDateTime" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutoScheduleProcesses_Desks_DeskId",
                table: "AutoScheduleProcesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Units_UnitId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftExceptions_Desks_DeskId",
                table: "ShiftExceptions");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftExceptions_Shifts_DeskId_ShiftStartDateTime",
                table: "ShiftExceptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Desks_DeskId",
                table: "Shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftSwaps_Desks_DeskId",
                table: "ShiftSwaps");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftSwaps_Shifts_DeskId_ShiftStart",
                table: "ShiftSwaps");

            migrationBuilder.DropTable(
                name: "DeskAssignments");

            migrationBuilder.DropTable(
                name: "Desks");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropIndex(
                name: "IX_ShiftSwaps_DeskId_ShiftStart",
                table: "ShiftSwaps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShiftExceptions",
                table: "ShiftExceptions");

            migrationBuilder.DropIndex(
                name: "IX_Employees_UnitId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_AutoScheduleProcesses_DeskId",
                table: "AutoScheduleProcesses");

            migrationBuilder.DropColumn(
                name: "DeskId",
                table: "ShiftSwaps");

            migrationBuilder.DropColumn(
                name: "DeskId",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "DeskId",
                table: "ShiftExceptions");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DeskId",
                table: "AutoScheduleProcesses");

            migrationBuilder.RenameColumn(
                name: "ShiftStart",
                table: "ShiftSwaps",
                newName: "ShiftKey");

            migrationBuilder.RenameColumn(
                name: "ScheduleStartDateTime",
                table: "Shifts",
                newName: "ScheduleKey");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_ScheduleStartDateTime",
                table: "Shifts",
                newName: "IX_Shifts_ScheduleKey");

            migrationBuilder.RenameColumn(
                name: "ShiftStartDateTime",
                table: "ShiftExceptions",
                newName: "ShiftKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts",
                column: "StartDateTime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShiftExceptions",
                table: "ShiftExceptions",
                columns: new[] { "ShiftKey", "EmployeeId" });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSwaps_ShiftKey",
                table: "ShiftSwaps",
                column: "ShiftKey");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftExceptions_Shifts_ShiftKey",
                table: "ShiftExceptions",
                column: "ShiftKey",
                principalTable: "Shifts",
                principalColumn: "StartDateTime",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftSwaps_Shifts_ShiftKey",
                table: "ShiftSwaps",
                column: "ShiftKey",
                principalTable: "Shifts",
                principalColumn: "StartDateTime",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
