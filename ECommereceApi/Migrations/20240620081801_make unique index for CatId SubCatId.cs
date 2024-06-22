using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBase.Migrations
{
    /// <inheritdoc />
    public partial class makeuniqueindexforCatIdSubCatId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CategorySubCategory_CategoryId",
                table: "CategorySubCategory");

            migrationBuilder.CreateIndex(
                name: "IX_CategorySubCategory_CategoryId_SubCategoryId",
                table: "CategorySubCategory",
                columns: new[] { "CategoryId", "SubCategoryId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CategorySubCategory_CategoryId_SubCategoryId",
                table: "CategorySubCategory");

            migrationBuilder.CreateIndex(
                name: "IX_CategorySubCategory_CategoryId",
                table: "CategorySubCategory",
                column: "CategoryId");
        }
    }
}
