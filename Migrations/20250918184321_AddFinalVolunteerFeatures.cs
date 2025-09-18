using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gift_Of_The_Givers_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalVolunteerFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VolunteerAssignments",
                columns: table => new
                {
                    VolunteerUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TaskID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerAssignments", x => new { x.VolunteerUserID, x.TaskID });
                    table.ForeignKey(
                        name: "FK_VolunteerAssignments_AspNetUsers_VolunteerUserID",
                        column: x => x.VolunteerUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VolunteerAssignments_VolunteerTasks_TaskID",
                        column: x => x.TaskID,
                        principalTable: "VolunteerTasks",
                        principalColumn: "TaskID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerAssignments_TaskID",
                table: "VolunteerAssignments",
                column: "TaskID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VolunteerAssignments");
        }
    }
}
