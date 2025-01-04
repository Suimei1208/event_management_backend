using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace event_service.Migrations
{
    /// <inheritdoc />
    public partial class AddEventAttendancies2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "eventId",
                table: "EventAttendances",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "eventId",
                table: "EventAttendances");
        }
    }
}
