using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace event_service.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeToSpending : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "Spendings",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type",
                table: "Spendings");
        }
    }
}
