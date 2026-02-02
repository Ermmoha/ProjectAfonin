SET NOCOUNT ON;
USE ObuveStore;

-- Drop tables in dependency order
IF OBJECT_ID('dbo.OrderItems', 'U') IS NOT NULL DROP TABLE dbo.OrderItems;
IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID('dbo.OrderStatuses', 'U') IS NOT NULL DROP TABLE dbo.OrderStatuses;
IF OBJECT_ID('dbo.Stock', 'U') IS NOT NULL DROP TABLE dbo.Stock;
IF OBJECT_ID('dbo.Warehouses', 'U') IS NOT NULL DROP TABLE dbo.Warehouses;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.Manufacturers', 'U') IS NOT NULL DROP TABLE dbo.Manufacturers;
IF OBJECT_ID('dbo.Suppliers', 'U') IS NOT NULL DROP TABLE dbo.Suppliers;
IF OBJECT_ID('dbo.Units', 'U') IS NOT NULL DROP TABLE dbo.Units;
IF OBJECT_ID('dbo.PickupPoints', 'U') IS NOT NULL DROP TABLE dbo.PickupPoints;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;

-- Roles
CREATE TABLE dbo.Roles (
    RoleId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

-- Users
CREATE TABLE dbo.Users (
    UserId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Login NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(50) NOT NULL,
    RoleId INT NOT NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId)
);

-- Units
CREATE TABLE dbo.Units (
    UnitId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(20) NOT NULL UNIQUE
);

-- Suppliers
CREATE TABLE dbo.Suppliers (
    SupplierId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

-- Manufacturers
CREATE TABLE dbo.Manufacturers (
    ManufacturerId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

-- Categories
CREATE TABLE dbo.Categories (
    CategoryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

-- Products (natural key: Article)
CREATE TABLE dbo.Products (
    Article NVARCHAR(20) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    UnitId INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    SupplierId INT NOT NULL,
    ManufacturerId INT NOT NULL,
    CategoryId INT NOT NULL,
    DiscountPercent INT NOT NULL CONSTRAINT DF_Products_Discount DEFAULT 0,
    Description NVARCHAR(500) NULL,
    Photo NVARCHAR(100) NULL,
    CONSTRAINT FK_Products_Units FOREIGN KEY (UnitId) REFERENCES dbo.Units(UnitId),
    CONSTRAINT FK_Products_Suppliers FOREIGN KEY (SupplierId) REFERENCES dbo.Suppliers(SupplierId),
    CONSTRAINT FK_Products_Manufacturers FOREIGN KEY (ManufacturerId) REFERENCES dbo.Manufacturers(ManufacturerId),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(CategoryId)
);

-- Warehouses
CREATE TABLE dbo.Warehouses (
    WarehouseId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

-- Stock by warehouse
CREATE TABLE dbo.Stock (
    WarehouseId INT NOT NULL,
    Article NVARCHAR(20) NOT NULL,
    Quantity INT NOT NULL,
    CONSTRAINT PK_Stock PRIMARY KEY (WarehouseId, Article),
    CONSTRAINT FK_Stock_Warehouses FOREIGN KEY (WarehouseId) REFERENCES dbo.Warehouses(WarehouseId),
    CONSTRAINT FK_Stock_Products FOREIGN KEY (Article) REFERENCES dbo.Products(Article)
);

-- Pickup points
CREATE TABLE dbo.PickupPoints (
    PickupPointId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PostalCode NVARCHAR(10) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    Street NVARCHAR(100) NOT NULL,
    House NVARCHAR(20) NOT NULL
);

-- Order statuses
CREATE TABLE dbo.OrderStatuses (
    StatusId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

-- Orders
CREATE TABLE dbo.Orders (
    OrderNumber INT NOT NULL PRIMARY KEY,
    OrderDate DATE NOT NULL,
    DeliveryDate DATE NOT NULL,
    PickupPointId INT NOT NULL,
    CustomerId INT NOT NULL,
    PickupCode NVARCHAR(20) NOT NULL,
    StatusId INT NOT NULL,
    CONSTRAINT FK_Orders_PickupPoints FOREIGN KEY (PickupPointId) REFERENCES dbo.PickupPoints(PickupPointId),
    CONSTRAINT FK_Orders_Users FOREIGN KEY (CustomerId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Orders_Statuses FOREIGN KEY (StatusId) REFERENCES dbo.OrderStatuses(StatusId)
);

-- Order items
CREATE TABLE dbo.OrderItems (
    OrderNumber INT NOT NULL,
    Article NVARCHAR(20) NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    DiscountPercent INT NOT NULL,
    CONSTRAINT PK_OrderItems PRIMARY KEY (OrderNumber, Article),
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderNumber) REFERENCES dbo.Orders(OrderNumber),
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (Article) REFERENCES dbo.Products(Article)
);

