using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBase.Migrations
{
    /// <inheritdoc />
    public partial class extendchangingstructureforcategoryandsubcategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategorySubCategories_Category_CategoryId",
                table: "CategorySubCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CategorySubCategories_SubCategory_SubCategoryId",
                table: "CategorySubCategories");

            migrationBuilder.DropTable(
                name: "ProductSubCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorySubCategories",
                table: "CategorySubCategories");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "CategorySubCategories");

            migrationBuilder.DropColumn(
                name: "ImageUri",
                table: "CategorySubCategories");

            migrationBuilder.RenameTable(
                name: "CategorySubCategories",
                newName: "CategorySubCategory");

            migrationBuilder.RenameIndex(
                name: "IX_CategorySubCategories_SubCategoryId",
                table: "CategorySubCategory",
                newName: "IX_CategorySubCategory_SubCategoryId");

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

            migrationBuilder.AddColumn<int>(
                name: "CategorySubCategoryId",
                table: "CategorySubCategory",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorySubCategory",
                table: "CategorySubCategory",
                column: "CategorySubCategoryId");

            migrationBuilder.CreateTable(
                name: "CategorySubCategoryValues",
                columns: table => new
                {
                    CategorySubCategoryId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImageUri = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorySubCategoryValues", x => new { x.CategorySubCategoryId, x.Value });
                    table.ForeignKey(
                        name: "FK_CategorySubCategoryValues_CategorySubCategory_CategorySubCategoryId",
                        column: x => x.CategorySubCategoryId,
                        principalTable: "CategorySubCategory",
                        principalColumn: "CategorySubCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategorySubCategoryValuesCategorySubCategoryId_CategorySubCategoryValuesValue",
                table: "Product",
                columns: new[] { "CategorySubCategoryValuesCategorySubCategoryId", "CategorySubCategoryValuesValue" });

            migrationBuilder.CreateIndex(
                name: "IX_CategorySubCategory_CategoryId",
                table: "CategorySubCategory",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySubCategory_Category_CategoryId",
                table: "CategorySubCategory",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySubCategory_SubCategory_SubCategoryId",
                table: "CategorySubCategory",
                column: "SubCategoryId",
                principalTable: "SubCategory",
                principalColumn: "SubId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_CategorySubCategoryValues_CategorySubCategoryValuesCategorySubCategoryId_CategorySubCategoryValuesValue",
                table: "Product",
                columns: new[] { "CategorySubCategoryValuesCategorySubCategoryId", "CategorySubCategoryValuesValue" },
                principalTable: "CategorySubCategoryValues",
                principalColumns: new[] { "CategorySubCategoryId", "Value" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategorySubCategory_Category_CategoryId",
                table: "CategorySubCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_CategorySubCategory_SubCategory_SubCategoryId",
                table: "CategorySubCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_CategorySubCategoryValues_CategorySubCategoryValuesCategorySubCategoryId_CategorySubCategoryValuesValue",
                table: "Product");

            migrationBuilder.DropTable(
                name: "CategorySubCategoryValues");

            migrationBuilder.DropIndex(
                name: "IX_Product_CategorySubCategoryValuesCategorySubCategoryId_CategorySubCategoryValuesValue",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorySubCategory",
                table: "CategorySubCategory");

            migrationBuilder.DropIndex(
                name: "IX_CategorySubCategory_CategoryId",
                table: "CategorySubCategory");

            migrationBuilder.DropColumn(
                name: "CategorySubCategoryValuesCategorySubCategoryId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "CategorySubCategoryValuesValue",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "CategorySubCategoryId",
                table: "CategorySubCategory");

            migrationBuilder.RenameTable(
                name: "CategorySubCategory",
                newName: "CategorySubCategories");

            migrationBuilder.RenameIndex(
                name: "IX_CategorySubCategory_SubCategoryId",
                table: "CategorySubCategories",
                newName: "IX_CategorySubCategories_SubCategoryId");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "CategorySubCategories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUri",
                table: "CategorySubCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorySubCategories",
                table: "CategorySubCategories",
                columns: new[] { "CategoryId", "SubCategoryId", "Value" });

            migrationBuilder.CreateTable(
                name: "ProductSubCategory",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SubId = table.Column<int>(type: "int", nullable: false),
                    SubCategoryValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSubCategory", x => new { x.ProductId, x.SubId });
                    table.ForeignKey(
                        name: "FK_ProductSubCategory_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_ProductSubCategory_SubCategory",
                        column: x => x.SubId,
                        principalTable: "SubCategory",
                        principalColumn: "SubId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubCategory_SubId",
                table: "ProductSubCategory",
                column: "SubId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySubCategories_Category_CategoryId",
                table: "CategorySubCategories",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySubCategories_SubCategory_SubCategoryId",
                table: "CategorySubCategories",
                column: "SubCategoryId",
                principalTable: "SubCategory",
                principalColumn: "SubId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
