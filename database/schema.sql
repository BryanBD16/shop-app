CREATE DATABASE IF NOT EXISTS shop_app;
USE shop_app;

DROP TABLE IF EXISTS products;

CREATE TABLE products (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    image_path VARCHAR(255) NOT NULL,
    description VARCHAR(255) NOT NULL DEFAULT '',
    stock_quantity INT NOT NULL DEFAULT 0,
    is_published BOOLEAN NOT NULL DEFAULT true
);

-- Same image placeholder for all products
-- (ex: backend/ShopApi/wwwroot/images/products/placeholder.jpg)

INSERT INTO products (name, price, image_path, description, stock_quantity, is_published) VALUES
('Keyboard', 49.99, '/images/products/placeholder.jpg', 'A standard mechanical keyboard for everyday use.', 45, true),
('Mouse', 19.99, '/images/products/mouse.jpg', 'Ergonomic mouse with adjustable DPI.', 80, true),
('Monitor 24"', 129.99, '/images/products/placeholder.jpg', '24-inch Full HD monitor with vibrant colors.', 20, true),
('Monitor 27"', 179.99, '/images/products/placeholder.jpg', '27-inch monitor ideal for gaming and productivity.', 15, true),
('USB Cable', 9.99, '/images/products/placeholder.jpg', 'Durable USB cable for charging and data transfer.', 200, true),
('HDMI Cable', 14.99, '/images/products/placeholder.jpg', 'High-speed HDMI cable for HD video and audio.', 150, true),
('Laptop Stand', 29.99, '/images/products/placeholder.jpg', 'Adjustable laptop stand for ergonomic setup.', 60, true),
('Webcam', 59.99, '/images/products/placeholder.jpg', 'HD webcam perfect for video calls and streaming.', 35, true),
('Headphones', 89.99, '/images/products/placeholder.jpg', 'Over-ear headphones with clear sound quality.', 40, true),
('Speakers', 49.99, '/images/products/placeholder.jpg', 'Compact speakers with powerful sound.', 30, true),
('Microphone', 69.99, '/images/products/placeholder.jpg', 'USB microphone suitable for podcasting and streaming.', 25, true),
('Mouse Pad', 12.99, '/images/products/placeholder.jpg', 'Smooth mouse pad for precise control.', 120, true),
('External Hard Drive 1TB', 79.99, '/images/products/placeholder.jpg', 'Reliable 1TB external hard drive.', 28, true),
('External Hard Drive 2TB', 119.99, '/images/products/placeholder.jpg', 'High-capacity 2TB external hard drive.', 18, true),
('SSD 500GB', 69.99, '/images/products/placeholder.jpg', 'Fast 500GB SSD for quick storage access.', 35, true),
('SSD 1TB', 119.99, '/images/products/placeholder.jpg', '1TB SSD with excellent read/write speeds.', 22, true),
('Laptop 13"', 999.99, '/images/products/placeholder.jpg', 'Portable 13-inch laptop for everyday tasks.', 10, true),
('Laptop 15"', 1299.99, '/images/products/placeholder.jpg', 'Powerful 15-inch laptop for work and gaming.', 6, true),
('Graphics Card', 399.99, '/images/products/placeholder.jpg', 'High-performance graphics card for gaming.', 8, true),
('Motherboard', 179.99, '/images/products/placeholder.jpg', 'Motherboard compatible with latest CPUs.', 14, true),
('CPU i5', 249.99, '/images/products/placeholder.jpg', 'Intel i5 processor suitable for most tasks.', 16, true),
('CPU i7', 349.99, '/images/products/placeholder.jpg', 'High-end Intel i7 processor for performance.', 9, true),
('RAM 8GB', 49.99, '/images/products/placeholder.jpg', '8GB RAM module for everyday computing.', 70, true),
('RAM 16GB', 89.99, '/images/products/placeholder.jpg', '16GB RAM module for multitasking.', 45, true),
('Power Supply 500W', 59.99, '/images/products/placeholder.jpg', 'Reliable 500W power supply unit.', 30, true),
('Power Supply 750W', 79.99, '/images/products/placeholder.jpg', '750W PSU for high-performance systems.', 20, true),
('Case', 69.99, '/images/products/placeholder.jpg', 'Durable PC case with airflow design.', 18, true),
('Cooling Fan', 19.99, '/images/products/placeholder.jpg', 'Cooling fan to keep your PC temperature down.', 90, true),
('Gaming Chair', 199.99, '/images/products/placeholder.jpg', 'Ergonomic gaming chair for comfort.', 12, true),
('Desk Lamp', 29.99, '/images/products/placeholder.jpg', 'LED desk lamp with adjustable brightness.', 55, true),
('Router', 79.99, '/images/products/placeholder.jpg', 'Wi-Fi router for fast home networking.', 25, true),
('Switch', 129.99, '/images/products/placeholder.jpg', 'Network switch to expand your LAN.', 10, true),
('Network Cable', 12.99, '/images/products/placeholder.jpg', 'Ethernet cable for stable connections.', 150, true),
('Printer', 149.99, '/images/products/placeholder.jpg', 'All-in-one printer for home or office.', 14, true),
('Scanner', 89.99, '/images/products/placeholder.jpg', 'High-resolution document scanner.', 11, true),
('Laptop Bag', 39.99, '/images/products/placeholder.jpg', 'Protective laptop bag with compartments.', 65, true),
('Tablet 10"', 299.99, '/images/products/placeholder.jpg', '10-inch tablet for media and light work.', 18, true),
('Tablet 12"', 399.99, '/images/products/placeholder.jpg', '12-inch tablet for productivity and media.', 12, true),
('Smartphone', 699.99, '/images/products/placeholder.jpg', 'Latest smartphone with high-resolution camera.', 20, true),
('Smartwatch', 199.99, '/images/products/placeholder.jpg', 'Smartwatch with health tracking features.', 22, true),
('Phone Case', 19.99, '/images/products/placeholder.jpg', 'Protective phone case with stylish design.', 100, true),
('Screen Protector', 14.99, '/images/products/placeholder.jpg', 'Screen protector to prevent scratches.', 140, true),
('Portable Charger', 29.99, '/images/products/placeholder.jpg', 'High-capacity portable charger.', 60, true),
('Bluetooth Speaker', 49.99, '/images/products/placeholder.jpg', 'Portable Bluetooth speaker with good sound.', 35, true),
('USB Hub', 24.99, '/images/products/placeholder.jpg', 'USB hub to expand your connectivity.', 50, true),
('Memory Card 32GB', 14.99, '/images/products/placeholder.jpg', '32GB memory card for storage expansion.', 110, true),
('Memory Card 64GB', 24.99, '/images/products/placeholder.jpg', '64GB memory card for more storage.', 85, true),
('Camera', 499.99, '/images/products/placeholder.jpg', 'High-resolution camera for photography.', 5, false),
('Tripod', 39.99, '/images/products/placeholder.jpg', 'Tripod stand for cameras.', 7, false),
('Lens', 299.99, '/images/products/placeholder.jpg', 'Camera lens for versatile shooting.', 4, false),
('Action Camera', 199.99, '/images/products/placeholder.jpg', 'Action camera for sports and adventures.', 6, false);
