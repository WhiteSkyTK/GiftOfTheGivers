using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gift_Of_The_Givers_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class AddLogisticsToIncident : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MeetingPoint",
                table: "DisasterIncidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OnSiteContact",
                table: "DisasterIncidents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingPoint",
                table: "DisasterIncidents");

            migrationBuilder.DropColumn(
                name: "OnSiteContact",
                table: "DisasterIncidents");
        }
    }
}
