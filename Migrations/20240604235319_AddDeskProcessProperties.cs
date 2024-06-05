using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class AddDeskProcessProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CatchRange",
                table: "Desks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "4.00:00:00");

            migrationBuilder.AddColumn<string>(
                name: "FileWindowDuration",
                table: "Desks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "1.00:00:00");

            migrationBuilder.AddColumn<string>(
                name: "HeadsUpDuration",
                table: "Desks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "2.12:00:00");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CatchRange",
                table: "Desks");

            migrationBuilder.DropColumn(
                name: "FileWindowDuration",
                table: "Desks");

            migrationBuilder.DropColumn(
                name: "HeadsUpDuration",
                table: "Desks");
        }
    }
}
