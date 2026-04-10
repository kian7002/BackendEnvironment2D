using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendEnvironment2d.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRequiredUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Environments_AspNetUsers_UserId",
                table: "Environments");

            migrationBuilder.DropIndex(
                name: "IX_Environments_UserId_Name",
                table: "Environments");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Environments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Environments_UserId_Name",
                table: "Environments",
                columns: new[] { "UserId", "Name" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Environments_AspNetUsers_UserId",
                table: "Environments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Environments_AspNetUsers_UserId",
                table: "Environments");

            migrationBuilder.DropIndex(
                name: "IX_Environments_UserId_Name",
                table: "Environments");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Environments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Environments_UserId_Name",
                table: "Environments",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Environments_AspNetUsers_UserId",
                table: "Environments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
