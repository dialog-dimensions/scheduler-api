using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerSupportSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchedulerGptSessions_Employees_EmployeeId",
                table: "SchedulerGptSessions");

            migrationBuilder.DropIndex(
                name: "IX_SchedulerGptSessions_EmployeeId",
                table: "SchedulerGptSessions");
            
            migrationBuilder.CreateTable(
                name: "GptSessions",
                columns: table => new
                {
                    ThreadId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CurrentAssistantId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValue: "NotCreated")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GptSessions", x => x.ThreadId);
                    table.ForeignKey(
                        name: "FK_GptSessions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManagerSupportGptSessions",
                columns: table => new
                {
                    ThreadId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerSupportGptSessions", x => x.ThreadId);
                    table.ForeignKey(
                        name: "FK_ManagerSupportGptSessions_GptSessions_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "GptSessions",
                        principalColumn: "ThreadId",
                        onDelete: ReferentialAction.Cascade);
                });
            
            // Insert default data into GptSessions based on SchedulerGptSessions
            migrationBuilder.Sql(
                @"
            INSERT INTO GptSessions (ThreadId, Type, EmployeeId, CurrentAssistantId, State)
            SELECT ThreadId, 'ExceptionGathering', EmployeeId, 'asst_90CvK3QXuBum8X3OCwFxDqLe', 'Closed'
            FROM SchedulerGptSessions
            WHERE NOT EXISTS (
                SELECT 1 FROM GptSessions WHERE GptSessions.ThreadId = SchedulerGptSessions.ThreadId
            );
            "
            );

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "SchedulerGptSessions");
            
            migrationBuilder.CreateIndex(
                name: "IX_GptSessions_EmployeeId",
                table: "GptSessions",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchedulerGptSessions_GptSessions_ThreadId",
                table: "SchedulerGptSessions",
                column: "ThreadId",
                principalTable: "GptSessions",
                principalColumn: "ThreadId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchedulerGptSessions_GptSessions_ThreadId",
                table: "SchedulerGptSessions");

            migrationBuilder.DropTable(
                name: "ManagerSupportGptSessions");

            migrationBuilder.DropTable(
                name: "GptSessions");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "SchedulerGptSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerGptSessions_EmployeeId",
                table: "SchedulerGptSessions",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchedulerGptSessions_Employees_EmployeeId",
                table: "SchedulerGptSessions",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
