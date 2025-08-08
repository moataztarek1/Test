using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KidZone.API.Migrations
{
    /// <inheritdoc />
    public partial class V05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgeGroup",
                table: "Contents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgeGroup",
                table: "Contents",
                type: "int",
                nullable: true);
        }
    }
}
