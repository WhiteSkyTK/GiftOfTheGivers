using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gift_Of_The_Givers_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidateProjectAndIncidentModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donations_ReliefProjects_ReliefProjectProjectID",
                table: "Donations");

            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerTasks_ReliefProjects_ReliefProjectProjectID",
                table: "VolunteerTasks");

            migrationBuilder.DropTable(
                name: "ReliefProjects");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerTasks_ReliefProjectProjectID",
                table: "VolunteerTasks");

            migrationBuilder.DropIndex(
                name: "IX_Donations_ReliefProjectProjectID",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "ReliefProjectProjectID",
                table: "VolunteerTasks");

            migrationBuilder.DropColumn(
                name: "ReliefProjectProjectID",
                table: "Donations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReliefProjectProjectID",
                table: "VolunteerTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReliefProjectProjectID",
                table: "Donations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReliefProjects",
                columns: table => new
                {
                    ProjectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentID = table.Column<int>(type: "int", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReliefProjects", x => x.ProjectID);
                    table.ForeignKey(
                        name: "FK_ReliefProjects_DisasterIncidents_IncidentID",
                        column: x => x.IncidentID,
                        principalTable: "DisasterIncidents",
                        principalColumn: "IncidentID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerTasks_ReliefProjectProjectID",
                table: "VolunteerTasks",
                column: "ReliefProjectProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_ReliefProjectProjectID",
                table: "Donations",
                column: "ReliefProjectProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_ReliefProjects_IncidentID",
                table: "ReliefProjects",
                column: "IncidentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_ReliefProjects_ReliefProjectProjectID",
                table: "Donations",
                column: "ReliefProjectProjectID",
                principalTable: "ReliefProjects",
                principalColumn: "ProjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerTasks_ReliefProjects_ReliefProjectProjectID",
                table: "VolunteerTasks",
                column: "ReliefProjectProjectID",
                principalTable: "ReliefProjects",
                principalColumn: "ProjectID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
