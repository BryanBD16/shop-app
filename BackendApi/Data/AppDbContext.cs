using Microsoft.EntityFrameworkCore;
using BackendApi.Models;

namespace BackendApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Keyboard", Price = 49.99m, ImagePath = "/images/products/keyboard.jpg", Description = "A standard mechanical keyboard for everyday use.", StockQuantity = 45, IsPublished = true },
            new Product { Id = 2, Name = "Mouse", Price = 19.99m, ImagePath = "/images/products/mouse.jpg", Description = "Ergonomic mouse with adjustable DPI.", StockQuantity = 80, IsPublished = true },
            new Product { Id = 3, Name = "Monitor 24\"", Price = 129.99m, ImagePath = "/images/products/monitor24.jpg", Description = "24-inch Full HD monitor with vibrant colors.", StockQuantity = 20, IsPublished = true },
            new Product { Id = 4, Name = "Monitor 27\"", Price = 179.99m, ImagePath = "/images/products/monitor27.jpg", Description = "27-inch monitor ideal for gaming and productivity.", StockQuantity = 15, IsPublished = true },
            new Product { Id = 5, Name = "USB Cable", Price = 9.99m, ImagePath = "/images/products/usbCable.jpg", Description = "Durable USB cable for charging and data transfer.", StockQuantity = 200, IsPublished = true },
            new Product { Id = 6, Name = "HDMI Cable", Price = 14.99m, ImagePath = "/images/products/placeholder.jpg", Description = "High-speed HDMI cable for HD video and audio.", StockQuantity = 150, IsPublished = true },
            new Product { Id = 7, Name = "Laptop Stand", Price = 29.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Adjustable laptop stand for ergonomic setup.", StockQuantity = 60, IsPublished = true },
            new Product { Id = 8, Name = "Webcam", Price = 59.99m, ImagePath = "/images/products/placeholder.jpg", Description = "HD webcam perfect for video calls and streaming.", StockQuantity = 35, IsPublished = true },
            new Product { Id = 9, Name = "Headphones", Price = 89.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Over-ear headphones with clear sound quality.", StockQuantity = 40, IsPublished = true },
            new Product { Id = 10, Name = "Speakers", Price = 49.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Compact speakers with powerful sound.", StockQuantity = 30, IsPublished = true },
            new Product { Id = 11, Name = "Microphone", Price = 69.99m, ImagePath = "/images/products/placeholder.jpg", Description = "USB microphone suitable for podcasting and streaming.", StockQuantity = 25, IsPublished = true },
            new Product { Id = 12, Name = "Mouse Pad", Price = 12.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Smooth mouse pad for precise control.", StockQuantity = 120, IsPublished = true },
            new Product { Id = 13, Name = "External Hard Drive 1TB", Price = 79.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Reliable 1TB external hard drive.", StockQuantity = 28, IsPublished = true },
            new Product { Id = 14, Name = "External Hard Drive 2TB", Price = 119.99m, ImagePath = "/images/products/placeholder.jpg", Description = "High-capacity 2TB external hard drive.", StockQuantity = 18, IsPublished = true },
            new Product { Id = 15, Name = "SSD 500GB", Price = 69.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Fast 500GB SSD for quick storage access.", StockQuantity = 35, IsPublished = true },
            new Product { Id = 16, Name = "SSD 1TB", Price = 119.99m, ImagePath = "/images/products/placeholder.jpg", Description = "1TB SSD with excellent read/write speeds.", StockQuantity = 22, IsPublished = true },
            new Product { Id = 17, Name = "Laptop 13\"", Price = 999.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Portable 13-inch laptop for everyday tasks.", StockQuantity = 10, IsPublished = true },
            new Product { Id = 18, Name = "Laptop 15\"", Price = 1299.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Powerful 15-inch laptop for work and gaming.", StockQuantity = 6, IsPublished = true },
            new Product { Id = 19, Name = "Graphics Card", Price = 399.99m, ImagePath = "/images/products/placeholder.jpg", Description = "High-performance graphics card for gaming.", StockQuantity = 8, IsPublished = true },
            new Product { Id = 20, Name = "Motherboard", Price = 179.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Motherboard compatible with latest CPUs.", StockQuantity = 14, IsPublished = true },
            new Product { Id = 21, Name = "CPU i5", Price = 249.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Intel i5 processor suitable for most tasks.", StockQuantity = 16, IsPublished = true },
            new Product { Id = 22, Name = "CPU i7", Price = 349.99m, ImagePath = "/images/products/placeholder.jpg", Description = "High-end Intel i7 processor for performance.", StockQuantity = 9, IsPublished = true },
            new Product { Id = 23, Name = "RAM 8GB", Price = 49.99m, ImagePath = "/images/products/placeholder.jpg", Description = "8GB RAM module for everyday computing.", StockQuantity = 70, IsPublished = true },
            new Product { Id = 24, Name = "RAM 16GB", Price = 89.99m, ImagePath = "/images/products/placeholder.jpg", Description = "16GB RAM module for multitasking.", StockQuantity = 45, IsPublished = true },
            new Product { Id = 25, Name = "Power Supply 500W", Price = 59.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Reliable 500W power supply unit.", StockQuantity = 30, IsPublished = true },
            new Product { Id = 26, Name = "Power Supply 750W", Price = 79.99m, ImagePath = "/images/products/placeholder.jpg", Description = "750W PSU for high-performance systems.", StockQuantity = 20, IsPublished = true },
            new Product { Id = 27, Name = "Case", Price = 69.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Durable PC case with airflow design.", StockQuantity = 18, IsPublished = true },
            new Product { Id = 28, Name = "Cooling Fan", Price = 19.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Cooling fan to keep your PC temperature down.", StockQuantity = 90, IsPublished = true },
            new Product { Id = 29, Name = "Gaming Chair", Price = 199.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Ergonomic gaming chair for comfort.", StockQuantity = 12, IsPublished = true },
            new Product { Id = 30, Name = "Desk Lamp", Price = 29.99m, ImagePath = "/images/products/placeholder.jpg", Description = "LED desk lamp with adjustable brightness.", StockQuantity = 55, IsPublished = true },
            new Product { Id = 31, Name = "Router", Price = 79.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Wi-Fi router for fast home networking.", StockQuantity = 25, IsPublished = true },
            new Product { Id = 32, Name = "Switch", Price = 129.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Network switch to expand your LAN.", StockQuantity = 10, IsPublished = true },
            new Product { Id = 33, Name = "Network Cable", Price = 12.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Ethernet cable for stable connections.", StockQuantity = 150, IsPublished = true },
            new Product { Id = 34, Name = "Printer", Price = 149.99m, ImagePath = "/images/products/placeholder.jpg", Description = "All-in-one printer for home or office.", StockQuantity = 14, IsPublished = true },
            new Product { Id = 35, Name = "Scanner", Price = 89.99m, ImagePath = "/images/products/placeholder.jpg", Description = "High-resolution document scanner.", StockQuantity = 11, IsPublished = true },
            new Product { Id = 36, Name = "Laptop Bag", Price = 39.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Protective laptop bag with compartments.", StockQuantity = 65, IsPublished = true },
            new Product { Id = 37, Name = "Tablet 10\"", Price = 299.99m, ImagePath = "/images/products/placeholder.jpg", Description = "10-inch tablet for media and light work.", StockQuantity = 18, IsPublished = true },
            new Product { Id = 38, Name = "Tablet 12\"", Price = 399.99m, ImagePath = "/images/products/placeholder.jpg", Description = "12-inch tablet for productivity and media.", StockQuantity = 12, IsPublished = true },
            new Product { Id = 39, Name = "Smartphone", Price = 699.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Latest smartphone with high-resolution camera.", StockQuantity = 20, IsPublished = true },
            new Product { Id = 40, Name = "Smartwatch", Price = 199.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Smartwatch with health tracking features.", StockQuantity = 22, IsPublished = true },
            new Product { Id = 41, Name = "Phone Case", Price = 19.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Protective phone case with stylish design.", StockQuantity = 100, IsPublished = true },
            new Product { Id = 42, Name = "Screen Protector", Price = 14.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Screen protector to prevent scratches.", StockQuantity = 140, IsPublished = true },
            new Product { Id = 43, Name = "Portable Charger", Price = 29.99m, ImagePath = "/images/products/placeholder.jpg", Description = "High-capacity portable charger.", StockQuantity = 60, IsPublished = true },
            new Product { Id = 44, Name = "Bluetooth Speaker", Price = 49.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Portable Bluetooth speaker with good sound.", StockQuantity = 35, IsPublished = true },
            new Product { Id = 45, Name = "USB Hub", Price = 24.99m, ImagePath = "/images/products/placeholder.jpg", Description = "USB hub to expand your connectivity.", StockQuantity = 50, IsPublished = true },
            new Product { Id = 46, Name = "Memory Card 32GB", Price = 14.99m, ImagePath = "/images/products/placeholder.jpg", Description = "32GB memory card for storage expansion.", StockQuantity = 110, IsPublished = true },
            new Product { Id = 47, Name = "Memory Card 64GB", Price = 24.99m, ImagePath = "/images/products/placeholder.jpg", Description = "64GB memory card for more storage.", StockQuantity = 85, IsPublished = true },
            new Product { Id = 48, Name = "Camera", Price = 499.99m, ImagePath = "/images/products/placeholder.jpg", Description = "High-resolution camera for photography.", StockQuantity = 5, IsPublished = false },
            new Product { Id = 49, Name = "Tripod", Price = 39.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Tripod stand for cameras.", StockQuantity = 7, IsPublished = false },
            new Product { Id = 50, Name = "Lens", Price = 299.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Camera lens for versatile shooting.", StockQuantity = 4, IsPublished = false },
            new Product { Id = 51, Name = "Action Camera", Price = 199.99m, ImagePath = "/images/products/placeholder.jpg", Description = "Action camera for sports and adventures.", StockQuantity = 6, IsPublished = false }
        );
    }
}
