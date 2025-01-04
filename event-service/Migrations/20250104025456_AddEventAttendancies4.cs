using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace event_service.Migrations
{
    /// <inheritdoc />
    public partial class AddEventAttendancies4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "checkIn",
                table: "EventAttendances",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "checkOut",
                table: "EventAttendances",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "checkIn",
                table: "EventAttendances");

            migrationBuilder.DropColumn(
                name: "checkOut",
                table: "EventAttendances");
        }
    }
}
