DROP DATABASE IF EXISTS `ProductionDB`;
CREATE DATABASE `ProductionDB`
  DEFAULT CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;
USE `ProductionDB`;

DROP TABLE IF EXISTS users;

CREATE TABLE users (
id INT NOT NULL AUTO_INCREMENT, 
name TEXT NOT NULL, 
email TEXT NOT NULL, 
active TINYINT(1) NOT NULL DEFAULT 0, 
PRIMARY KEY(id)
);


INSERT INTO users
(name, email, active)
VALUES 
('Alice', 'alice@gmail.com', 1), 
('Bob', 'bob@gmail.com', 1), 
('Carol', 'carol@gmail.com', 0), 
('Dan', 'dan@gmail.com', 0), 
('Eve', 'eve@gmail.com', 1); 


-- Drop user (safe if it doesn't exist)
DROP USER IF EXISTS 'ptdbuser'@'%';

-- Create user with password and require SSL
CREATE USER 'ptdbuser'@'%'
  IDENTIFIED BY 'Training2026#@!'
  REQUIRE SSL;

-- Grant permissions on the database
GRANT ALL PRIVILEGES ON `ProductionDB`.* TO 'ptdbuser'@'%';

CREATE USER IF NOT EXISTS 'ptdbuser'@'localhost'
  IDENTIFIED BY 'Training2026#@!';
GRANT ALL PRIVILEGES ON `ProductionDB`.* TO 'ptdbuser'@'localhost';

FLUSH PRIVILEGES;

