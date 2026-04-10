
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendEnvironment2d.Migrations
{
    /// <inheritdoc />
    public partial class AddSlotIndexToEnvironment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SlotIndex",
                table: "Environments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SlotIndex",
                table: "Environments");
        }
    }
}
