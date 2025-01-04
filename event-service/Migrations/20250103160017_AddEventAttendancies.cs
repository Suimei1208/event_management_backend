using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace event_service.Migrations
{
    /// <inheritdoc />
    public partial class AddEventAttendancies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventAttendances",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    userId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    checkInTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    checkOutTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAttendances", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventAttendances");
        }
    }
}
