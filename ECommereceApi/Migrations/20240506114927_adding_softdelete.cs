using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommereceApi.Migrations
{
    /// <inheritdoc />
    public partial class adding_softdelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "ProductImages",
                newName: "ImageName");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "User",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "Product",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "ProductImages",
                newName: "Image");
        }
    }
}
