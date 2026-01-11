using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class DataSample : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "ImagePath", "IsPublished", "Name", "Price", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "A standard mechanical keyboard for everyday use.", "/images/products/keyboard.jpg", true, "Keyboard", 49.99m, 45 },
                    { 2, "Ergonomic mouse with adjustable DPI.", "/images/products/mouse.jpg", true, "Mouse", 19.99m, 80 },
                    { 3, "24-inch Full HD monitor with vibrant colors.", "/images/products/monitor24.jpg", true, "Monitor 24\"", 129.99m, 20 },
                    { 4, "27-inch monitor ideal for gaming and productivity.", "/images/products/monitor27.jpg", true, "Monitor 27\"", 179.99m, 15 },
                    { 5, "Durable USB cable for charging and data transfer.", "/images/products/usbCable.jpg", true, "USB Cable", 9.99m, 200 },
                    { 6, "High-speed HDMI cable for HD video and audio.", "/images/products/placeholder.jpg", true, "HDMI Cable", 14.99m, 150 },
                    { 7, "Adjustable laptop stand for ergonomic setup.", "/images/products/placeholder.jpg", true, "Laptop Stand", 29.99m, 60 },
                    { 8, "HD webcam perfect for video calls and streaming.", "/images/products/placeholder.jpg", true, "Webcam", 59.99m, 35 },
                    { 9, "Over-ear headphones with clear sound quality.", "/images/products/placeholder.jpg", true, "Headphones", 89.99m, 40 },
                    { 10, "Compact speakers with powerful sound.", "/images/products/placeholder.jpg", true, "Speakers", 49.99m, 30 },
                    { 11, "USB microphone suitable for podcasting and streaming.", "/images/products/placeholder.jpg", true, "Microphone", 69.99m, 25 },
                    { 12, "Smooth mouse pad for precise control.", "/images/products/placeholder.jpg", true, "Mouse Pad", 12.99m, 120 },
                    { 13, "Reliable 1TB external hard drive.", "/images/products/placeholder.jpg", true, "External Hard Drive 1TB", 79.99m, 28 },
                    { 14, "High-capacity 2TB external hard drive.", "/images/products/placeholder.jpg", true, "External Hard Drive 2TB", 119.99m, 18 },
                    { 15, "Fast 500GB SSD for quick storage access.", "/images/products/placeholder.jpg", true, "SSD 500GB", 69.99m, 35 },
                    { 16, "1TB SSD with excellent read/write speeds.", "/images/products/placeholder.jpg", true, "SSD 1TB", 119.99m, 22 },
                    { 17, "Portable 13-inch laptop for everyday tasks.", "/images/products/placeholder.jpg", true, "Laptop 13\"", 999.99m, 10 },
                    { 18, "Powerful 15-inch laptop for work and gaming.", "/images/products/placeholder.jpg", true, "Laptop 15\"", 1299.99m, 6 },
                    { 19, "High-performance graphics card for gaming.", "/images/products/placeholder.jpg", true, "Graphics Card", 399.99m, 8 },
                    { 20, "Motherboard compatible with latest CPUs.", "/images/products/placeholder.jpg", true, "Motherboard", 179.99m, 14 },
                    { 21, "Intel i5 processor suitable for most tasks.", "/images/products/placeholder.jpg", true, "CPU i5", 249.99m, 16 },
                    { 22, "High-end Intel i7 processor for performance.", "/images/products/placeholder.jpg", true, "CPU i7", 349.99m, 9 },
                    { 23, "8GB RAM module for everyday computing.", "/images/products/placeholder.jpg", true, "RAM 8GB", 49.99m, 70 },
                    { 24, "16GB RAM module for multitasking.", "/images/products/placeholder.jpg", true, "RAM 16GB", 89.99m, 45 },
                    { 25, "Reliable 500W power supply unit.", "/images/products/placeholder.jpg", true, "Power Supply 500W", 59.99m, 30 },
                    { 26, "750W PSU for high-performance systems.", "/images/products/placeholder.jpg", true, "Power Supply 750W", 79.99m, 20 },
                    { 27, "Durable PC case with airflow design.", "/images/products/placeholder.jpg", true, "Case", 69.99m, 18 },
                    { 28, "Cooling fan to keep your PC temperature down.", "/images/products/placeholder.jpg", true, "Cooling Fan", 19.99m, 90 },
                    { 29, "Ergonomic gaming chair for comfort.", "/images/products/placeholder.jpg", true, "Gaming Chair", 199.99m, 12 },
                    { 30, "LED desk lamp with adjustable brightness.", "/images/products/placeholder.jpg", true, "Desk Lamp", 29.99m, 55 },
                    { 31, "Wi-Fi router for fast home networking.", "/images/products/placeholder.jpg", true, "Router", 79.99m, 25 },
                    { 32, "Network switch to expand your LAN.", "/images/products/placeholder.jpg", true, "Switch", 129.99m, 10 },
                    { 33, "Ethernet cable for stable connections.", "/images/products/placeholder.jpg", true, "Network Cable", 12.99m, 150 },
                    { 34, "All-in-one printer for home or office.", "/images/products/placeholder.jpg", true, "Printer", 149.99m, 14 },
                    { 35, "High-resolution document scanner.", "/images/products/placeholder.jpg", true, "Scanner", 89.99m, 11 },
                    { 36, "Protective laptop bag with compartments.", "/images/products/placeholder.jpg", true, "Laptop Bag", 39.99m, 65 },
                    { 37, "10-inch tablet for media and light work.", "/images/products/placeholder.jpg", true, "Tablet 10\"", 299.99m, 18 },
                    { 38, "12-inch tablet for productivity and media.", "/images/products/placeholder.jpg", true, "Tablet 12\"", 399.99m, 12 },
                    { 39, "Latest smartphone with high-resolution camera.", "/images/products/placeholder.jpg", true, "Smartphone", 699.99m, 20 },
                    { 40, "Smartwatch with health tracking features.", "/images/products/placeholder.jpg", true, "Smartwatch", 199.99m, 22 },
                    { 41, "Protective phone case with stylish design.", "/images/products/placeholder.jpg", true, "Phone Case", 19.99m, 100 },
                    { 42, "Screen protector to prevent scratches.", "/images/products/placeholder.jpg", true, "Screen Protector", 14.99m, 140 },
                    { 43, "High-capacity portable charger.", "/images/products/placeholder.jpg", true, "Portable Charger", 29.99m, 60 },
                    { 44, "Portable Bluetooth speaker with good sound.", "/images/products/placeholder.jpg", true, "Bluetooth Speaker", 49.99m, 35 },
                    { 45, "USB hub to expand your connectivity.", "/images/products/placeholder.jpg", true, "USB Hub", 24.99m, 50 },
                    { 46, "32GB memory card for storage expansion.", "/images/products/placeholder.jpg", true, "Memory Card 32GB", 14.99m, 110 },
                    { 47, "64GB memory card for more storage.", "/images/products/placeholder.jpg", true, "Memory Card 64GB", 24.99m, 85 },
                    { 48, "High-resolution camera for photography.", "/images/products/placeholder.jpg", false, "Camera", 499.99m, 5 },
                    { 49, "Tripod stand for cameras.", "/images/products/placeholder.jpg", false, "Tripod", 39.99m, 7 },
                    { 50, "Camera lens for versatile shooting.", "/images/products/placeholder.jpg", false, "Lens", 299.99m, 4 },
                    { 51, "Action camera for sports and adventures.", "/images/products/placeholder.jpg", false, "Action Camera", 199.99m, 6 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 51);
        }
    }
}
