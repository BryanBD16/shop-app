CREATE DATABASE IF NOT EXISTS shop_app;
USE shop_app;

CREATE TABLE products (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    price DECIMAL(10,2) NOT NULL
);

INSERT INTO products (name, price) VALUES
('Clavier', 49.99),
('Souris', 19.99);
