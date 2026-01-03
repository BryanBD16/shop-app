CREATE DATABASE IF NOT EXISTS shop_app;
USE shop_app;

-- Supprimer la table si elle existe
DROP TABLE IF EXISTS products;

-- Créer la table
CREATE TABLE products (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    price DECIMAL(10,2) NOT NULL
);

-- Insérer 50 produits
INSERT INTO products (name, price) VALUES
('Keyboard', 49.99),
('Mouse', 19.99),
('Monitor 24"', 129.99),
('Monitor 27"', 179.99),
('USB Cable', 9.99),
('HDMI Cable', 14.99),
('Laptop Stand', 29.99),
('Webcam', 59.99),
('Headphones', 89.99),
('Speakers', 49.99),
('Microphone', 69.99),
('Mouse Pad', 12.99),
('External Hard Drive 1TB', 79.99),
('External Hard Drive 2TB', 119.99),
('SSD 500GB', 69.99),
('SSD 1TB', 119.99),
('Laptop 13"', 999.99),
('Laptop 15"', 1299.99),
('Graphics Card', 399.99),
('Motherboard', 179.99),
('CPU i5', 249.99),
('CPU i7', 349.99),
('RAM 8GB', 49.99),
('RAM 16GB', 89.99),
('Power Supply 500W', 59.99),
('Power Supply 750W', 79.99),
('Case', 69.99),
('Cooling Fan', 19.99),
('Gaming Chair', 199.99),
('Desk Lamp', 29.99),
('Router', 79.99),
('Switch', 129.99),
('Network Cable', 12.99),
('Printer', 149.99),
('Scanner', 89.99),
('Laptop Bag', 39.99),
('Tablet 10"', 299.99),
('Tablet 12"', 399.99),
('Smartphone', 699.99),
('Smartwatch', 199.99),
('Phone Case', 19.99),
('Screen Protector', 14.99),
('Portable Charger', 29.99),
('Bluetooth Speaker', 49.99),
('USB Hub', 24.99),
('Memory Card 32GB', 14.99),
('Memory Card 64GB', 24.99),
('Camera', 499.99),
('Tripod', 39.99),
('Lens', 299.99),
('Action Camera', 199.99);