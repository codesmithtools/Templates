-- PetShop for MySQL: a subset of PetShop.sql (SQL Server's), for the samples. Load it into a database named
-- petshop: mysql -u root -p petshop < PetShop.mysql.sql
CREATE TABLE Category (
  CategoryId VARCHAR(10) NOT NULL,
  Name VARCHAR(80) NULL,
  Descn VARCHAR(255) NULL,
  PRIMARY KEY (CategoryId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
;
CREATE TABLE Supplier (
  SuppId INT NOT NULL,
  Name VARCHAR(80) NULL,
  Status VARCHAR(2) NOT NULL,
  City VARCHAR(80) NULL,
  Zip VARCHAR(5) NULL,
  PRIMARY KEY (SuppId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
;
CREATE TABLE Product (
  ProductId VARCHAR(10) NOT NULL,
  CategoryId VARCHAR(10) NOT NULL,
  Name VARCHAR(80) NULL,
  Descn VARCHAR(255) NULL,
  Image VARCHAR(80) NULL,
  PRIMARY KEY (ProductId),
  KEY IxProductCategory (CategoryId),
  CONSTRAINT FkProductCategory FOREIGN KEY (CategoryId) REFERENCES Category (CategoryId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
;
CREATE TABLE Item (
  ItemId VARCHAR(10) NOT NULL,
  ProductId VARCHAR(10) NOT NULL,
  ListPrice DECIMAL(10,2) NULL,
  UnitCost DECIMAL(10,2) NULL,
  Supplier INT NULL,
  Status VARCHAR(2) NULL,
  Name VARCHAR(80) NULL,
  PRIMARY KEY (ItemId),
  UNIQUE KEY IxItemName (Name),
  KEY IxItemProduct (ProductId),
  KEY IxItemSupplier (Supplier),
  CONSTRAINT FkItemProduct FOREIGN KEY (ProductId) REFERENCES Product (ProductId),
  CONSTRAINT FkItemSupplier FOREIGN KEY (Supplier) REFERENCES Supplier (SuppId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
;
CREATE TABLE Orders (
  OrderId INT NOT NULL AUTO_INCREMENT,
  UserId VARCHAR(20) NOT NULL,
  OrderDate DATETIME NOT NULL,
  ShipCity VARCHAR(80) NOT NULL,
  TotalPrice DECIMAL(10,2) NOT NULL,
  Locale VARCHAR(20) NOT NULL DEFAULT 'en-US',
  PRIMARY KEY (OrderId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
;
CREATE TABLE LineItem (
  OrderId INT NOT NULL,
  LineNum INT NOT NULL,
  ItemId VARCHAR(10) NOT NULL,
  Quantity INT NOT NULL,
  UnitPrice DECIMAL(10,2) NOT NULL,
  PRIMARY KEY (OrderId, LineNum),
  KEY IxLineItemItem (ItemId),
  CONSTRAINT FkLineItemOrder FOREIGN KEY (OrderId) REFERENCES Orders (OrderId),
  CONSTRAINT FkLineItemItem FOREIGN KEY (ItemId) REFERENCES Item (ItemId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
;
-- Lower case: MySQL on Windows (lower_case_table_names=2, as the recorder runs it) lower-cases view names anyway.
CREATE VIEW productcategory AS
SELECT p.ProductId, p.Name AS ProductName, c.CategoryId, c.Name AS CategoryName
FROM Product p INNER JOIN Category c ON c.CategoryId = p.CategoryId
;
DELIMITER //
CREATE PROCEDURE GetProductsByCategory(IN categoryId VARCHAR(10))
BEGIN
  SELECT ProductId, Name, Descn FROM Product WHERE CategoryId = categoryId ORDER BY Name;
END
//
DELIMITER ;
