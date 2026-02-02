SET NOCOUNT ON;
USE ObuveStore;

INSERT INTO dbo.Warehouses (Name) VALUES (N'Основной склад');

INSERT INTO dbo.Roles (Name) VALUES (N'Администратор'), (N'Менеджер'), (N'Авторизированный клиент');

INSERT INTO dbo.Users (FullName, Login, Password, RoleId)
VALUES (N'Никифорова Весения Николаевна', N'94d5ous@gmail.com', N'uzWC67', (SELECT RoleId FROM dbo.Roles WHERE Name = N'Администратор'));
INSERT INTO dbo.Users (FullName, Login, Password, RoleId)
VALUES (N'Сазонов Руслан Германович', N'uth4iz@mail.com', N'2L6KZG', (SELECT RoleId FROM dbo.Roles WHERE Name = N'Администратор'));
INSERT INTO dbo.Users (FullName, Login, Password, RoleId)
VALUES (N'Одинцов Серафим Артёмович', N'yzls62@outlook.com', N'JlFRCZ', (SELECT RoleId FROM dbo.Roles WHERE Name = N'Администратор'));
INSERT INTO dbo.Users (FullName, Login, Password, RoleId)
VALUES (N'Степанов Михаил Артёмович', N'1diph5e@tutanota.com', N'8ntwUp', (SELECT RoleId FROM dbo.Roles WHERE Name = N'Менеджер'));
INSERT INTO dbo.Users (FullName, Login, Password, RoleId)
VALUES (N'Ворсин Петр Евгеньевич', N'tjde7c@yahoo.com', N'YOyhfR', (SELECT RoleId FROM dbo.Roles WHERE Name = N'Менеджер'));
INSERT INTO dbo.Users (FullName, Login, Password, RoleId)
VALUES (N'Старикова Елена Павловна', N'wpmrc3do@tutanota.com', N'RSbvHv', (SELECT RoleId FROM dbo.Roles WHERE Name = N'Менеджер'));
INSERT INTO dbo.Users (FullName, Login, Password, RoleId)
VALUES (N'Михайлюк Анна Вячеславовна', N'5d4zbu@tutanota.com', N'rwVDh9', (SELECT RoleId FROM dbo.Roles WHERE Name = N'Авторизированный клиент'));
INSERT INTO dbo.Users (FullName, Login, Password, RoleId)
VALUES (N'Ситдикова Елена Анатольевна', N'ptec8ym@yahoo.com', N'LdNyos', (SELECT RoleId FROM dbo.Roles WHERE Name = N'Авторизированный клиент'));
INSERT INTO dbo.Users (FullName, Login, Password, RoleId)
VALUES (N'Ворсин Петр Евгеньевич', N'1qz4kw@mail.com', N'gynQMT', (SELECT RoleId FROM dbo.Roles WHERE Name = N'Авторизированный клиент'));
INSERT INTO dbo.Users (FullName, Login, Password, RoleId)
VALUES (N'Старикова Елена Павловна', N'4np6se@mail.com', N'AtnDjr', (SELECT RoleId FROM dbo.Roles WHERE Name = N'Авторизированный клиент'));

INSERT INTO dbo.Units (Name) VALUES (N'шт.');

INSERT INTO dbo.Suppliers (Name) VALUES (N'Kari'), (N'Обувь для вас');

INSERT INTO dbo.Manufacturers (Name) VALUES (N'Kari'), (N'Marco Tozzi'), (N'Рос'), (N'Rieker'), (N'Alessio Nesca'), (N'CROSBY');

INSERT INTO dbo.Categories (Name) VALUES (N'Женская обувь'), (N'Мужская обувь');

INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'А112Т4', N'Ботинки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 4990, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Kari'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 3, N'Женские Ботинки демисезонные kari', N'1.jpg');
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'А112Т4', 6);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'F635R4', N'Ботинки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 3244, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Обувь для вас'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Marco Tozzi'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 2, N'Ботинки Marco Tozzi женские демисезонные, размер 39, цвет бежевый', N'2.jpg');
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'F635R4', 13);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'H782T5', N'Туфли', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 4499, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Kari'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Мужская обувь'), 4, N'Туфли kari мужские классика MYZ21AW-450A, размер 43, цвет: черный', N'3.jpg');
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'H782T5', 5);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'G783F5', N'Ботинки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 5900, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Рос'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Мужская обувь'), 2, N'Мужские ботинки Рос-Обувь кожаные с натуральным мехом', N'4.jpg');
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'G783F5', 8);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'J384T6', N'Ботинки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 3800, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Обувь для вас'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Rieker'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Мужская обувь'), 2, N'B3430/14 Полуботинки мужские Rieker', N'5.jpg');
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'J384T6', 16);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'D572U8', N'Кроссовки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 4100, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Обувь для вас'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Рос'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Мужская обувь'), 3, N'129615-4 Кроссовки мужские', N'6.jpg');
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'D572U8', 6);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'F572H7', N'Туфли', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 2700, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Marco Tozzi'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 2, N'Туфли Marco Tozzi женские летние, размер 39, цвет черный', N'7.jpg');
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'F572H7', 14);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'D329H3', N'Полуботинки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 1890, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Обувь для вас'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Alessio Nesca'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 4, N'Полуботинки Alessio Nesca женские 3-30797-47, размер 37, цвет: бордовый', N'8.jpg');
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'D329H3', 4);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'B320R5', N'Туфли', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 4300, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Rieker'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 2, N'Туфли Rieker женские демисезонные, размер 41, цвет коричневый', N'9.jpg');
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'B320R5', 6);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'G432E4', N'Туфли', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 2800, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Kari'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 3, N'Туфли kari женские TR-YR-413017, размер 37, цвет: черный', N'10.jpg');
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'G432E4', 15);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'S213E3', N'Полуботинки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 2156, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Обувь для вас'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'CROSBY'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Мужская обувь'), 3, N'407700/01-01 Полуботинки мужские CROSBY', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'S213E3', 6);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'E482R4', N'Полуботинки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 1800, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Kari'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 2, N'Полуботинки kari женские MYZ20S-149, размер 41, цвет: черный', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'E482R4', 14);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'S634B5', N'Кеды', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 5500, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Обувь для вас'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'CROSBY'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Мужская обувь'), 3, N'Кеды Caprice мужские демисезонные, размер 42, цвет черный', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'S634B5', 0);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'K345R4', N'Полуботинки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 2100, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Обувь для вас'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'CROSBY'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Мужская обувь'), 2, N'407700/01-02 Полуботинки мужские CROSBY', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'K345R4', 3);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'O754F4', N'Туфли', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 5400, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Обувь для вас'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Rieker'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 4, N'Туфли женские демисезонные Rieker артикул 55073-68/37', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'O754F4', 18);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'G531F4', N'Ботинки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 6600, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Kari'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 12, N'Ботинки женские зимние ROMER арт. 893167-01 Черный', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'G531F4', 9);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'J542F5', N'Тапочки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 500, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Kari'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Мужская обувь'), 13, N'Тапочки мужские Арт.70701-55-67син р.41', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'J542F5', 0);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'B431R5', N'Ботинки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 2700, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Обувь для вас'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Rieker'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Мужская обувь'), 2, N'Мужские кожаные ботинки/мужские ботинки', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'B431R5', 5);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'P764G4', N'Туфли', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 6800, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'CROSBY'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 15, N'Туфли женские, ARGO, размер 38', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'P764G4', 15);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'C436G5', N'Ботинки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 10200, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Alessio Nesca'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 15, N'Ботинки женские, ARGO, размер 40', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'C436G5', 9);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'F427R5', N'Ботинки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 11800, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Обувь для вас'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Rieker'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 15, N'Ботинки на молнии с декоративной пряжкой FRAU', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'F427R5', 11);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'N457T5', N'Полуботинки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 4600, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'CROSBY'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 3, N'Полуботинки Ботинки черные зимние, мех', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'N457T5', 13);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'D364R4', N'Туфли', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 12400, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Kari'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 16, N'Туфли Luiza Belly женские Kate-lazo черные из натуральной замши', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'D364R4', 5);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'S326R5', N'Тапочки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 9900, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Обувь для вас'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'CROSBY'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Мужская обувь'), 17, N'Мужские кожаные тапочки "Профиль С.Дали" ', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'S326R5', 15);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'L754R4', N'Полуботинки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 1700, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Kari'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 2, N'Полуботинки kari женские WB2020SS-26, размер 38, цвет: черный', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'L754R4', 7);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'M542T5', N'Кроссовки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 2800, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Обувь для вас'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Rieker'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Мужская обувь'), 18, N'Кроссовки мужские TOFA', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'M542T5', 3);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'D268G5', N'Туфли', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 4399, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Обувь для вас'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Rieker'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 3, N'Туфли Rieker женские демисезонные, размер 36, цвет коричневый', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'D268G5', 12);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'T324F5', N'Сапоги', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 4699, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'CROSBY'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 2, N'Сапоги замша Цвет: синий', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'T324F5', 5);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'K358H6', N'Тапочки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 599, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Kari'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Rieker'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Мужская обувь'), 20, N'Тапочки мужские син р.41', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'K358H6', 2);
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (N'H535R5', N'Ботинки', (SELECT UnitId FROM dbo.Units WHERE Name = N'шт.'), 2300, (SELECT SupplierId FROM dbo.Suppliers WHERE Name = N'Обувь для вас'), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = N'Rieker'), (SELECT CategoryId FROM dbo.Categories WHERE Name = N'Женская обувь'), 2, N'Женские Ботинки демисезонные', NULL);
INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)
VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), N'H535R5', 7);

INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'344288', N'г. Лесной', N'ул. Чехова', N'1');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'614164', N'г.Лесной', N'ул. Степная', N'30');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'394242', N'г. Лесной', N'ул. Коммунистическая', N'43');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'660540', N'г. Лесной', N'ул. Солнечная', N'25');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'125837', N'г. Лесной', N'ул. Шоссейная', N'40');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'125703', N'г. Лесной', N'ул. Партизанская', N'49');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'625283', N'г. Лесной', N'ул. Победы', N'46');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'614611', N'г. Лесной', N'ул. Молодежная', N'50');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'454311', N'г.Лесной', N'ул. Новая', N'19');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'660007', N'г.Лесной', N'ул. Октябрьская', N'19');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'603036', N'г. Лесной', N'ул. Садовая', N'4');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'450983', N'г.Лесной', N'ул. Комсомольская', N'26');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'394782', N'г. Лесной', N'ул. Чехова', N'3');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'603002', N'г. Лесной', N'ул. Дзержинского', N'28');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'450558', N'г. Лесной', N'ул. Набережная', N'30');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'394060', N'г.Лесной', N'ул. Фрунзе', N'43');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'410661', N'г. Лесной', N'ул. Школьная', N'50');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'625590', N'г. Лесной', N'ул. Коммунистическая', N'20');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'625683', N'г. Лесной', N'ул. 8 Марта', NULL);
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'400562', N'г. Лесной', N'ул. Зеленая', N'32');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'614510', N'г. Лесной', N'ул. Маяковского', N'47');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'410542', N'г. Лесной', N'ул. Светлая', N'46');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'620839', N'г. Лесной', N'ул. Цветочная', N'8');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'443890', N'г. Лесной', N'ул. Коммунистическая', N'1');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'603379', N'г. Лесной', N'ул. Спортивная', N'46');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'603721', N'г. Лесной', N'ул. Гоголя', N'41');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'410172', N'г. Лесной', N'ул. Северная', N'13');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'420151', N'г. Лесной', N'ул. Вишневая', N'32');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'125061', N'г. Лесной', N'ул. Подгорная', N'8');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'630370', N'г. Лесной', N'ул. Шоссейная', N'24');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'614753', N'г. Лесной', N'ул. Полевая', N'35');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'426030', N'г. Лесной', N'ул. Маяковского', N'44');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'450375', N'г. Лесной ул. Клубная', N'44', NULL);
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'625560', N'г. Лесной', N'ул. Некрасова', N'12');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'630201', N'г. Лесной', N'ул. Комсомольская', N'17');
INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (N'190949', N'г. Лесной', N'ул. Мичурина', N'26');

INSERT INTO dbo.OrderStatuses (Name) VALUES (N'Завершен');

INSERT INTO dbo.Orders (OrderNumber, OrderDate, DeliveryDate, PickupPointId, CustomerId, PickupCode, StatusId)
VALUES (1, '2025-02-27', '2025-04-20', 1, (SELECT UserId FROM dbo.Users WHERE FullName = N'Степанов Михаил Артёмович'), N'901', (SELECT StatusId FROM dbo.OrderStatuses WHERE Name = N'Завершен'));
INSERT INTO dbo.Orders (OrderNumber, OrderDate, DeliveryDate, PickupPointId, CustomerId, PickupCode, StatusId)
VALUES (2, '2022-09-28', '2025-04-21', 11, (SELECT UserId FROM dbo.Users WHERE FullName = N'Никифорова Весения Николаевна'), N'902', (SELECT StatusId FROM dbo.OrderStatuses WHERE Name = N'Завершен'));
INSERT INTO dbo.Orders (OrderNumber, OrderDate, DeliveryDate, PickupPointId, CustomerId, PickupCode, StatusId)
VALUES (3, '2025-03-21', '2025-04-22', 2, (SELECT UserId FROM dbo.Users WHERE FullName = N'Сазонов Руслан Германович'), N'903', (SELECT StatusId FROM dbo.OrderStatuses WHERE Name = N'Завершен'));
INSERT INTO dbo.Orders (OrderNumber, OrderDate, DeliveryDate, PickupPointId, CustomerId, PickupCode, StatusId)
VALUES (4, '2025-02-20', '2025-04-23', 11, (SELECT UserId FROM dbo.Users WHERE FullName = N'Одинцов Серафим Артёмович'), N'904', (SELECT StatusId FROM dbo.OrderStatuses WHERE Name = N'Завершен'));
INSERT INTO dbo.Orders (OrderNumber, OrderDate, DeliveryDate, PickupPointId, CustomerId, PickupCode, StatusId)
VALUES (5, '2025-03-17', '2025-04-24', 2, (SELECT UserId FROM dbo.Users WHERE FullName = N'Степанов Михаил Артёмович'), N'905', (SELECT StatusId FROM dbo.OrderStatuses WHERE Name = N'Завершен'));
INSERT INTO dbo.Orders (OrderNumber, OrderDate, DeliveryDate, PickupPointId, CustomerId, PickupCode, StatusId)
VALUES (6, '2025-03-01', '2025-04-25', 15, (SELECT UserId FROM dbo.Users WHERE FullName = N'Никифорова Весения Николаевна'), N'906', (SELECT StatusId FROM dbo.OrderStatuses WHERE Name = N'Завершен'));
INSERT INTO dbo.Orders (OrderNumber, OrderDate, DeliveryDate, PickupPointId, CustomerId, PickupCode, StatusId)
VALUES (7, '2025-02-28', '2025-04-26', 3, (SELECT UserId FROM dbo.Users WHERE FullName = N'Сазонов Руслан Германович'), N'907', (SELECT StatusId FROM dbo.OrderStatuses WHERE Name = N'Завершен'));
INSERT INTO dbo.Orders (OrderNumber, OrderDate, DeliveryDate, PickupPointId, CustomerId, PickupCode, StatusId)
VALUES (8, '2025-03-31', '2025-04-27', 19, (SELECT UserId FROM dbo.Users WHERE FullName = N'Одинцов Серафим Артёмович'), N'908', (SELECT StatusId FROM dbo.OrderStatuses WHERE Name = N'Завершен'));
INSERT INTO dbo.Orders (OrderNumber, OrderDate, DeliveryDate, PickupPointId, CustomerId, PickupCode, StatusId)
VALUES (9, '2025-04-02', '2025-04-28', 5, (SELECT UserId FROM dbo.Users WHERE FullName = N'Степанов Михаил Артёмович'), N'909', (SELECT StatusId FROM dbo.OrderStatuses WHERE Name = N'Завершен'));
INSERT INTO dbo.Orders (OrderNumber, OrderDate, DeliveryDate, PickupPointId, CustomerId, PickupCode, StatusId)
VALUES (10, '2025-04-03', '2025-04-29', 19, (SELECT UserId FROM dbo.Users WHERE FullName = N'Степанов Михаил Артёмович'), N'910', (SELECT StatusId FROM dbo.OrderStatuses WHERE Name = N'Завершен'));

INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 1, p.Article, 2, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'А112Т4';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 1, p.Article, 2, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'F635R4';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 2, p.Article, 1, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'H782T5';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 2, p.Article, 1, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'G783F5';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 3, p.Article, 10, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'J384T6';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 3, p.Article, 10, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'D572U8';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 4, p.Article, 5, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'F572H7';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 4, p.Article, 4, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'D329H3';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 5, p.Article, 2, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'А112Т4';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 5, p.Article, 2, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'F635R4';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 6, p.Article, 1, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'H782T5';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 6, p.Article, 1, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'G783F5';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 7, p.Article, 10, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'J384T6';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 7, p.Article, 10, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'D572U8';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 8, p.Article, 5, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'F572H7';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 8, p.Article, 4, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'D329H3';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 9, p.Article, 5, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'B320R5';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 9, p.Article, 1, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'G432E4';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 10, p.Article, 5, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'S213E3';
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT 10, p.Article, 5, p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = N'E482R4';


