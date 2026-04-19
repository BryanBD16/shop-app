using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Percentage = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discounts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Discounts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImagePath",
                value: "/images/products/hdmi.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImagePath",
                value: "/images/products/laptopStand.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImagePath",
                value: "/images/products/webcam.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13,
                column: "ImagePath",
                value: "/images/products/externalDrive.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14,
                column: "ImagePath",
                value: "/images/products/externalDrive.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15,
                column: "ImagePath",
                value: "/images/products/externalDrive.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16,
                column: "ImagePath",
                value: "/images/products/externalDrive.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17,
                column: "ImagePath",
                value: "/images/products/pc.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18,
                column: "ImagePath",
                value: "/images/products/pc.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19,
                column: "ImagePath",
                value: "/images/products/graphicCard.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21,
                column: "ImagePath",
                value: "/images/products/cpu.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22,
                column: "ImagePath",
                value: "/images/products/cpu.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 29,
                column: "ImagePath",
                value: "/images/products/chair.jpg");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_CategoryId",
                table: "Discounts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_ProductId",
                table: "Discounts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImagePath",
                value: "/images/products/placeholder.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImagePath",
                value: "/images/products/placeholder.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImagePath",
                value: "/images/products/placeholder.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13,
                column: "ImagePath",
                value: "/images/products/placeholder.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14,
                column: "ImagePath",
                value: "/images/products/placeholder.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15,
                column: "ImagePath",
                value: "/images/products/placeholder.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16,
                column: "ImagePath",
                value: "/images/products/placeholder.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17,
                column: "ImagePath",
                value: "/images/products/placeholder.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18,
                column: "ImagePath",
                value: "/images/products/placeholder.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19,
                column: "ImagePath",
                value: "/images/products/placeholder.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21,
                column: "ImagePath",
                value: "/images/products/placeholder.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22,
                column: "ImagePath",
                value: "/images/products/placeholder.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 29,
                column: "ImagePath",
                value: "/images/products/placeholder.jpg");
        }
    }
}
