using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommereceApi.Migrations
{
    /// <inheritdoc />
    public partial class addVerificationColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "User",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VertificationCode",
                table: "User",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

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
                name: "IsVerified",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VertificationCode",
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
