using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBase.Migrations
{
    /// <inheritdoc />
    public partial class makeCategorySubCategoryIdvalueProductIdtheprimarykeyfortable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_CategorySubCategoryValues_CategorySubCategoryValuesCategorySubCategoryId_CategorySubCategoryValuesValue",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_CategorySubCategoryValuesCategorySubCategoryId_CategorySubCategoryValuesValue",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorySubCategoryValues",
                table: "CategorySubCategoryValues");

            migrationBuilder.DropColumn(
                name: "CategorySubCategoryValuesCategorySubCategoryId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "CategorySubCategoryValuesValue",
                table: "Product");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "CategorySubCategoryValues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorySubCategoryValues",
                table: "CategorySubCategoryValues",
                columns: new[] { "CategorySubCategoryId", "Value", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_CategorySubCategoryValues_ProductId",
                table: "CategorySubCategoryValues",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySubCategoryValues_Product_ProductId",
                table: "CategorySubCategoryValues",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategorySubCategoryValues_Product_ProductId",
                table: "CategorySubCategoryValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorySubCategoryValues",
                table: "CategorySubCategoryValues");

            migrationBuilder.DropIndex(
                name: "IX_CategorySubCategoryValues_ProductId",
                table: "CategorySubCategoryValues");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "CategorySubCategoryValues");

            migrationBuilder.AddColumn<int>(
                name: "CategorySubCategoryValuesCategorySubCategoryId",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategorySubCategoryValuesValue",
                table: "Product",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorySubCategoryValues",
                table: "CategorySubCategoryValues",
                columns: new[] { "CategorySubCategoryId", "Value" });

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategorySubCategoryValuesCategorySubCategoryId_CategorySubCategoryValuesValue",
                table: "Product",
                columns: new[] { "CategorySubCategoryValuesCategorySubCategoryId", "CategorySubCategoryValuesValue" });

            migrationBuilder.AddForeignKey(
                name: "FK_Product_CategorySubCategoryValues_CategorySubCategoryValuesCategorySubCategoryId_CategorySubCategoryValuesValue",
                table: "Product",
                columns: new[] { "CategorySubCategoryValuesCategorySubCategoryId", "CategorySubCategoryValuesValue" },
                principalTable: "CategorySubCategoryValues",
                principalColumns: new[] { "CategorySubCategoryId", "Value" });
        }
    }
}
