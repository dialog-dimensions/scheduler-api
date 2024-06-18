using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessNextPhaseJobId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NextPhaseJobId",
                table: "AutoScheduleProcesses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextPhaseJobId",
                table: "AutoScheduleProcesses");
        }
    }
}
