using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace event_service.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Events",
                table: "Participants");

            migrationBuilder.AddColumn<bool>(
                name: "access",
                table: "Events",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Events_eventId",
                table: "Participants",
                column: "eventId",
                principalTable: "Events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Events_eventId",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "access",
                table: "Events");

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Events",
                table: "Participants",
                column: "eventId",
                principalTable: "Events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
