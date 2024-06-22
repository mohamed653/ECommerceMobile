using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBase.Migrations
{
    /// <inheritdoc />
    public partial class changeCategorySubCategorytabletohaveIdasPKsothatwecanchangevalueofanyrecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorySubCategoryValues",
                table: "CategorySubCategoryValues");

            migrationBuilder.DropIndex(
                name: "IX_CategorySubCategoryValues_ProductId",
                table: "CategorySubCategoryValues");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CategorySubCategoryValues",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorySubCategoryValues",
                table: "CategorySubCategoryValues",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CategorySubCategoryValues_CategorySubCategoryId",
                table: "CategorySubCategoryValues",
                column: "CategorySubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorySubCategoryValues_ProductId_CategorySubCategoryId_Value",
                table: "CategorySubCategoryValues",
                columns: new[] { "ProductId", "CategorySubCategoryId", "Value" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorySubCategoryValues",
                table: "CategorySubCategoryValues");

            migrationBuilder.DropIndex(
                name: "IX_CategorySubCategoryValues_CategorySubCategoryId",
                table: "CategorySubCategoryValues");

            migrationBuilder.DropIndex(
                name: "IX_CategorySubCategoryValues_ProductId_CategorySubCategoryId_Value",
                table: "CategorySubCategoryValues");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CategorySubCategoryValues");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorySubCategoryValues",
                table: "CategorySubCategoryValues",
                columns: new[] { "CategorySubCategoryId", "Value", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_CategorySubCategoryValues_ProductId",
                table: "CategorySubCategoryValues",
                column: "ProductId");
        }
    }
}
