CREATE DATABASE IF NOT EXISTS shop_app;
USE shop_app;

-- Supprimer la table si elle existe
DROP TABLE IF EXISTS products;

-- Créer la table products
CREATE TABLE products (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    image_path VARCHAR(255) NOT NULL,
    is_published BOOLEAN NOT NULL DEFAULT true
);

-- Image placeholder commune à tous les produits
-- (ex: backend/ShopApi/wwwroot/images/products/placeholder.jpg)

INSERT INTO products (name, price, image_path, is_published) VALUES
('Keyboard', 49.99, '/images/products/placeholder.jpg', true),
('Mouse', 19.99, '/images/products/placeholder.jpg', true),
('Monitor 24"', 129.99, '/images/products/placeholder.jpg', true),
('Monitor 27"', 179.99, '/images/products/placeholder.jpg', true),
('USB Cable', 9.99, '/images/products/placeholder.jpg', true),
('HDMI Cable', 14.99, '/images/products/placeholder.jpg', true),
('Laptop Stand', 29.99, '/images/products/placeholder.jpg', true),
('Webcam', 59.99, '/images/products/placeholder.jpg', true),
('Headphones', 89.99, '/images/products/placeholder.jpg', true),
('Speakers', 49.99, '/images/products/placeholder.jpg', true),
('Microphone', 69.99, '/images/products/placeholder.jpg', true),
('Mouse Pad', 12.99, '/images/products/placeholder.jpg', true),
('External Hard Drive 1TB', 79.99, '/images/products/placeholder.jpg', true),
('External Hard Drive 2TB', 119.99, '/images/products/placeholder.jpg', true),
('SSD 500GB', 69.99, '/images/products/placeholder.jpg', true),
('SSD 1TB', 119.99, '/images/products/placeholder.jpg', true),
('Laptop 13"', 999.99, '/images/products/placeholder.jpg', true),
('Laptop 15"', 1299.99, '/images/products/placeholder.jpg', true),
('Graphics Card', 399.99, '/images/products/placeholder.jpg', true),
('Motherboard', 179.99, '/images/products/placeholder.jpg', true),
('CPU i5', 249.99, '/images/products/placeholder.jpg', true),
('CPU i7', 349.99, '/images/products/placeholder.jpg', true),
('RAM 8GB', 49.99, '/images/products/placeholder.jpg', true),
('RAM 16GB', 89.99, '/images/products/placeholder.jpg', true),
('Power Supply 500W', 59.99, '/images/products/placeholder.jpg', true),
('Power Supply 750W', 79.99, '/images/products/placeholder.jpg', true),
('Case', 69.99, '/images/products/placeholder.jpg', true),
('Cooling Fan', 19.99, '/images/products/placeholder.jpg', true),
('Gaming Chair', 199.99, '/images/products/placeholder.jpg', true),
('Desk Lamp', 29.99, '/images/products/placeholder.jpg', true),
('Router', 79.99, '/images/products/placeholder.jpg', true),
('Switch', 129.99, '/images/products/placeholder.jpg', true),
('Network Cable', 12.99, '/images/products/placeholder.jpg', true),
('Printer', 149.99, '/images/products/placeholder.jpg', true),
('Scanner', 89.99, '/images/products/placeholder.jpg', true),
('Laptop Bag', 39.99, '/images/products/placeholder.jpg', true),
('Tablet 10"', 299.99, '/images/products/placeholder.jpg', true),
('Tablet 12"', 399.99, '/images/products/placeholder.jpg', true),
('Smartphone', 699.99, '/images/products/placeholder.jpg', true),
('Smartwatch', 199.99, '/images/products/placeholder.jpg', true),
('Phone Case', 19.99, '/images/products/placeholder.jpg', true),
('Screen Protector', 14.99, '/images/products/placeholder.jpg', true),
('Portable Charger', 29.99, '/images/products/placeholder.jpg', true),
('Bluetooth Speaker', 49.99, '/images/products/placeholder.jpg', true),
('USB Hub', 24.99, '/images/products/placeholder.jpg', true),
('Memory Card 32GB', 14.99, '/images/products/placeholder.jpg', true),
('Memory Card 64GB', 24.99, '/images/products/placeholder.jpg', true),
('Camera', 499.99, '/images/products/placeholder.jpg', false),
('Tripod', 39.99, '/images/products/placeholder.jpg', false),
('Lens', 299.99, '/images/products/placeholder.jpg', false),
('Action Camera', 199.99, '/images/products/placeholder.jpg', false);
