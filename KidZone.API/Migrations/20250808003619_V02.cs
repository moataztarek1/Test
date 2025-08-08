using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KidZone.API.Migrations
{
    /// <inheritdoc />
    public partial class V02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age_Group",
                table: "Children");

            migrationBuilder.AddColumn<int>(
                name: "AgeGroup",
                table: "Contents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Contents",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgeGroup",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Contents");

            migrationBuilder.AddColumn<int>(
                name: "Age_Group",
                table: "Children",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
