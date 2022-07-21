using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrackerDemo.Data.Migrations
{
    public partial class _006 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ArchivedByProject",
                table: "Tickets",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivedByProject",
                table: "Tickets");
        }
    }
}
