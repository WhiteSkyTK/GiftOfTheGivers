using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gift_Of_The_Givers_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class AddDonationToIncidentRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donations_ReliefProjects_ProjectID",
                table: "Donations");

            migrationBuilder.RenameColumn(
                name: "ProjectID",
                table: "Donations",
                newName: "ReliefProjectProjectID");

            migrationBuilder.RenameIndex(
                name: "IX_Donations_ProjectID",
                table: "Donations",
                newName: "IX_Donations_ReliefProjectProjectID");

            migrationBuilder.AddColumn<int>(
                name: "DisasterIncidentID",
                table: "Donations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Donations_DisasterIncidentID",
                table: "Donations",
                column: "DisasterIncidentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_DisasterIncidents_DisasterIncidentID",
                table: "Donations",
                column: "DisasterIncidentID",
                principalTable: "DisasterIncidents",
                principalColumn: "IncidentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_ReliefProjects_ReliefProjectProjectID",
                table: "Donations",
                column: "ReliefProjectProjectID",
                principalTable: "ReliefProjects",
                principalColumn: "ProjectID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donations_DisasterIncidents_DisasterIncidentID",
                table: "Donations");

            migrationBuilder.DropForeignKey(
                name: "FK_Donations_ReliefProjects_ReliefProjectProjectID",
                table: "Donations");

            migrationBuilder.DropIndex(
                name: "IX_Donations_DisasterIncidentID",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "DisasterIncidentID",
                table: "Donations");

            migrationBuilder.RenameColumn(
                name: "ReliefProjectProjectID",
                table: "Donations",
                newName: "ProjectID");

            migrationBuilder.RenameIndex(
                name: "IX_Donations_ReliefProjectProjectID",
                table: "Donations",
                newName: "IX_Donations_ProjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_ReliefProjects_ProjectID",
                table: "Donations",
                column: "ProjectID",
                principalTable: "ReliefProjects",
                principalColumn: "ProjectID");
        }
    }
}
