CREATE DATABASE PetBox
GO

USE PetBox
GO

--這個區塊是建立資料表

--CustomerLoginName是登入用的帳號
--CustomerName是本名


CREATE TABLE Customers(
CustomerID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
CustomerLoginName VARCHAR(20),
CustomerName NVARCHAR(30),
CustomerEmail VARCHAR(255),
CustomerPassword VARCHAR(255),
CustomerMobilePhone NVARCHAR(24),
CustomerAddress NVARCHAR(255),
CustomerPostalCode NVARCHAR(10),
CustomerCountry NVARCHAR(50),
CustomerRole INT DEFAULT 2
)
GO

--OpinionStatus 不需要空值 是因為客戶寄來是0 解決以後是1

CREATE TABLE Opinions(
OpinionID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
CustomerID INT NOT NULL,
EmployeeID INT DEFAULT 1,
ProductID INT NOT NULL DEFAULT 1,
OpinionTitle NVARCHAR(255),
OpinionContent NVARCHAR(1000),
OpinionStatus INT NOT NULL DEFAULT 0,
OpinionDateTime CHAR(16),
OpinionFeedback NVARCHAR(1000) DEFAULT '',
OpinionFeedbackTime CHAR(16) DEFAULT ''
)
GO


CREATE TABLE Products (
    ProductID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    ProductCode VARCHAR(100) NOT NULL,
    CONSTRAINT AK_ProductCode UNIQUE(ProductCode),
    ProductName NVARCHAR(100),
	ProductDescription NVARCHAR(500),
	CategoryID INT NOT NULL,
	ProductImageLocation VARCHAR(250),
	ProductQuantity INT,
	ProductUnit NVARCHAR(50),
	ProductPrice MONEY NOT NULL
); 

CREATE TABLE Categories (
    CategoryID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	CategoryName NVARCHAR(100) NOT NULL,
    CategoryDescription NVARCHAR(255),
	CategoryPictureLocation VARCHAR(255)
);


CREATE TABLE UserRoles (
    UserRoleID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	UserRoleName VARCHAR(100) NOT NULL,
	UserRoleDescription NVARCHAR(200)
);




CREATE TABLE Employees (
    EmployeeID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	EmployeeLoginName VARCHAR(20) NOT NULL,
	EmployeePassword VARCHAR(20) NOT NULL,
	EmployeeRole INT DEFAULT 1
);


CREATE TABLE Orders (
    OrderID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	CustomerID INT NOT NULL,
	OrderDateTime CHAR(16),
	OrderShipAddress NVARCHAR(200),
	OrderShipPostalCode NVARCHAR(10),
    OrderShipCountry NVARCHAR(50),
);

CREATE TABLE OrderDetails (
    OrderDetailID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    OrderID INT NOT NULL,
	ProductID INT NOT NULL,
	Quantity INT NOT NULL,
	UnitPrice MONEY NOT NULL,
	Discount REAL DEFAULT 0
);


-- entity framework 6.x does not support use foriegn key refer to 
-- a UNIQUE column in another table. So no ProductCode column here.

CREATE TABLE OptionalItemImages (
    OptionalItemImageID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    ProductID INT NOT NULL,
    OptionalItemImageLocation VARCHAR(250),
    OptionalItemImageWidth INT,
    OptionalItemImageTop INT,
    OptionalItemImageLeft INT,
    OptionalItemImageZ INT DEFAULT 2
);

-- create forign key

ALTER TABLE Customers
ADD CONSTRAINT FK_Customers_UserRoles FOREIGN KEY (CustomerRole)
    REFERENCES UserRoles (UserRoleID)
	ON DELETE SET DEFAULT
	ON UPDATE CASCADE
;
GO


ALTER TABLE Opinions
ADD CONSTRAINT FK_Opinions_Customers FOREIGN KEY (CustomerID)
    REFERENCES Customers (CustomerID)
	ON DELETE CASCADE
	ON UPDATE CASCADE
;
GO

/*
if delete one row of UserRole, Custormers or Employees will be affected by this action, and 
there is another table 'Opinions' refer to Both customerID and EmployeeID  
to avoid error "may cause cycles or multiple cascade paths.", without using trigger. I set `NO ACTION` here
*/

ALTER TABLE Opinions
ADD CONSTRAINT FK_Opinions_Employees FOREIGN KEY (EmployeeID)
    REFERENCES Employees (EmployeeID)
	ON DELETE NO ACTION
	ON UPDATE NO ACTION
;
GO

ALTER TABLE Opinions
ADD CONSTRAINT FK_Opinions_Products FOREIGN KEY (ProductID)
    REFERENCES Products (ProductID)
	ON DELETE SET DEFAULT
	ON UPDATE CASCADE
;
GO

ALTER TABLE Products
ADD CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryID)
    REFERENCES Categories (CategoryID)
	ON DELETE CASCADE
	ON UPDATE CASCADE
;
GO

ALTER TABLE Employees
ADD CONSTRAINT FK_Employees_UserRoles FOREIGN KEY (EmployeeRole)
    REFERENCES UserRoles (UserRoleID)
	ON DELETE SET DEFAULT
	ON UPDATE CASCADE
;
GO

ALTER TABLE Orders
ADD CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerID)
    REFERENCES Customers (CustomerID)
	ON DELETE CASCADE
	ON UPDATE CASCADE
;
GO


ALTER TABLE OrderDetails
ADD CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductID)
    REFERENCES Products (ProductID)
	ON DELETE CASCADE
	ON UPDATE CASCADE
;
GO

ALTER TABLE OrderDetails
ADD CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderID)
    REFERENCES Orders (OrderID)
	ON DELETE CASCADE
	ON UPDATE CASCADE
;
GO


ALTER TABLE OptionalItemImages
ADD CONSTRAINT FK_OptionalItemImages_Products_ID FOREIGN KEY (ProductID)
    REFERENCES Products (ProductID)
	ON DELETE CASCADE
	ON UPDATE CASCADE
;
GO

--because it will cause may cause cycles or multiple cascade paths. set `NO ACTION` here

--ALTER TABLE OptionalItemImages
--ADD CONSTRAINT FK_OptionalItemImages_Products_ProductCode FOREIGN KEY (ProductCode)
--    REFERENCES Products (ProductCode)
--	ON DELETE NO ACTION
--	ON UPDATE NO ACTION
--;
--GO



--這個區塊是增加資料



--類別名 說明 圖片位子

INSERT INTO Categories(CategoryName,CategoryDescription,CategoryPictureLocation)
VALUES(N'dummy product category', N'for dummy products','C:\img')

INSERT INTO Categories(CategoryName,CategoryDescription,CategoryPictureLocation)
VALUES(N'寵物屋', N'各種大小，型式的寵物屋','C:\img')


INSERT INTO Categories(CategoryName,CategoryDescription,CategoryPictureLocation)
VALUES(N'鏡頭', N'各種監控用鏡頭，包含一般光線、近紅外線等等…','C:\img')

INSERT INTO Categories(CategoryName,CategoryDescription,CategoryPictureLocation)
VALUES(N'感測器', N'感測目標的溫度、溼度、壓力、光線等環境參數','C:\img')

INSERT INTO Categories(CategoryName,CategoryDescription,CategoryPictureLocation)
VALUES(N'餵食器', N'各種尺寸、各種動物專用餵食器','C:\img')
GO

--用戶角色名子 說明

-- to set UserRoleID explicitly, I have to turn the IDENTITY_INSERT ON, and OFF.
-- dummy 這個角色值並不能登入使用網站，只是用來處理


INSERT INTO UserRoles (UserRoleName, UserRoleDescription)
VALUES('dummy', 'this role can not do anything, just for a default value');

INSERT INTO UserRoles (UserRoleName, UserRoleDescription)
VALUES('customer-nonmember', 'customers who do not login in yet');

INSERT INTO UserRoles(UserRoleName,UserRoleDescription)
VALUES('customer-member','member')

INSERT INTO UserRoles(UserRoleName,UserRoleDescription)
VALUES('shelf','shelf people')

INSERT INTO UserRoles(UserRoleName,UserRoleDescription)
VALUES('customer service','customer service people')
GO

--帳號 姓名 信箱 密碼 手機 地址 郵遞區號 國家

INSERT INTO Customers(CustomerLoginName,CustomerName,CustomerEmail,CustomerPassword,CustomerMobilePhone,CustomerAddress,CustomerPostalCode,CustomerCountry, CustomerRole)
VALUES('customer-nonmember', N'','','customer-nonmemberPass','', N'', '', '', 2)



INSERT INTO Customers(CustomerLoginName,CustomerName,CustomerEmail,CustomerPassword,CustomerMobilePhone,CustomerAddress,CustomerPostalCode,CustomerCountry, CustomerRole)
VALUES('dung', N'鄧元','abc@yahoo.com','dungPass','0912987445', N'台中市南屯區公益路二段51號', '123', 'TW', 3)

INSERT INTO Customers(CustomerLoginName,CustomerName,CustomerEmail,CustomerPassword,CustomerMobilePhone,CustomerAddress,CustomerPostalCode,CustomerCountry,CustomerRole)
VALUES('wang', N'王大為','abc@yahoo.com','wangPass','0933340056', N'台中市南屯區公益路二段51號', '200', 'TW', 3)

INSERT INTO Customers(CustomerLoginName,CustomerName,CustomerEmail,CustomerPassword,CustomerMobilePhone,CustomerAddress,CustomerPostalCode,CustomerCountry,CustomerRole)
VALUES('li', N'李小弟','aaa@yahoo.com','liPass','0935112334', N'Blake Gerold 128 N 37th St.', '300', 'USA', 3)

INSERT INTO Customers(CustomerLoginName,CustomerName,CustomerEmail,CustomerPassword,CustomerMobilePhone,CustomerAddress,CustomerPostalCode,CustomerCountry,CustomerRole)
VALUES('cheng', N'陳得峰','bcd@yahoo.com','chengPass','0906567332', N'神奈川県横浜市泉区白百合2-10-14', '400', 'JP', 3)

INSERT INTO Customers(CustomerLoginName,CustomerName,CustomerEmail,CustomerPassword,CustomerMobilePhone,CustomerAddress,CustomerPostalCode,CustomerCountry,CustomerRole)
VALUES('lai', N'賴國徽','cvb@yahoo.com','laiPass','0956887452', N'台北市忠孝北路三段2號', '450', 'TW', 3)
GO

--帳號 密碼 角色

-- 這個員工帳號無法做任何事，只是用來當成預設值，像是Opinion中的EmployeeId的預設值為0，就是這個帳號。

INSERT INTO Employees(EmployeeLoginName,EmployeePassword,EmployeeRole)
VALUES('nobody','nobody',1)

INSERT INTO Employees(EmployeeLoginName,EmployeePassword,EmployeeRole)
VALUES('shelfa','shelfa',4)

INSERT INTO Employees(EmployeeLoginName,EmployeePassword,EmployeeRole)
VALUES('shelfb','shelfb',4)

INSERT INTO Employees(EmployeeLoginName,EmployeePassword,EmployeeRole)
VALUES('service','service',5)
GO



--產品代碼 產品名子 產品說明 類別ID 產品圖片位子 產品數量 產品單位 產品價格


INSERT INTO Products(ProductCode,ProductName,ProductDescription,CategoryID,ProductImageLocation,ProductQuantity,Productunit,ProductPrice)
VALUES('dummy', N'dummy product', N'use as a default value',1,'',0, N'',0)

INSERT INTO Products(ProductCode,ProductName,ProductDescription,CategoryID,ProductImageLocation,ProductQuantity,Productunit,ProductPrice)
VALUES('box_small', N'小型動物專用透明寵物屋', N'適合敘利亞倉鼠等小型動物',2,'C:\img',20, N'組',180)
GO

INSERT INTO Products(ProductCode,ProductName,ProductDescription,CategoryID,ProductImageLocation,ProductQuantity,Productunit,ProductPrice)
VALUES('cam_wide', N'磁性廣角鏡頭', N'強力的磁性底盤，可以吸附於各種角度，隨鏡頭附贈吸盤，提供更多元的安裝方式',3,'/product_images/site_product/cam_wide.png',10, N'台',250)

INSERT INTO Products(ProductCode,ProductName,ProductDescription,CategoryID,ProductImageLocation,ProductQuantity,Productunit,ProductPrice)
VALUES('cam_Normal', N'普通鏡頭', N'鏡頭附贈吸盤，可安裝於平滑表面',3,'C:\img',10, N'台',250)

INSERT INTO Products(ProductCode,ProductName,ProductDescription,CategoryID,ProductImageLocation,ProductQuantity,Productunit,ProductPrice)
VALUES('cam_rf', N'紅外線鏡頭', N'可於夜晚時清楚地拍攝物體',3,'C:\img',10, N'台',250)



INSERT INTO Products(ProductCode,ProductName,ProductDescription,CategoryID,ProductImageLocation,ProductQuantity,Productunit,ProductPrice)
VALUES('sensor_humi_temp', N'DHT11溫濕度感測器', N'同時監測空氣溼度與溫度，運作環境：溼度介於20%~90%，溫度介於0度至50度',4,'/product_images/site_product/sensor_humi_temp.png',15, N'個',100)


INSERT INTO Products(ProductCode,ProductName,ProductDescription,CategoryID,ProductImageLocation,ProductQuantity,Productunit,ProductPrice)
VALUES('sensor_weight', N'壓力感測', N'可以感受五公斤以下重量',4,'C:\img',15, N'個',100)



INSERT INTO Products(ProductCode,ProductName,ProductDescription,CategoryID,ProductImageLocation,ProductQuantity,Productunit,ProductPrice)
VALUES('feeder_small', N'小型動物自動餵食器', N'擁有定時功能，適合一隻倉鼠等小型動物。',5,'/product_images/site_product/feeder_small.png',20, N'組',180)
GO

INSERT INTO Products(ProductCode,ProductName,ProductDescription,CategoryID,ProductImageLocation,ProductQuantity,Productunit,ProductPrice)
VALUES('feeder_median', N'中型動物自動餵食器', N'擁有定時功能，適合兩隻左右倉鼠。',5,'C:\img',20, N'組',180)
GO



--顧客ID 員工ID 產品ID 意見標題 內容 狀況 時間
INSERT INTO Opinions(CustomerID, OpinionTitle, OpinionContent, OpinionDateTime)
VALUES(1, N'到貨時間?', N'我今天買了溫溼度感應器，請問什麼時後到貨？', '20180825 12:00')

INSERT INTO Opinions(CustomerID, OpinionTitle, OpinionContent, OpinionDateTime)
VALUES(2, N'產品怎麼操作?', N'我收到了貴公司的產品，可是沒有操作手冊，請問網站有沒有教學文件？', '20180826 15:00')

INSERT INTO Opinions(CustomerID, OpinionTitle, OpinionContent, OpinionDateTime)
VALUES(3, N'要求退貨?', N'不好意思，我買錯商品，想要退貨，請問貴公司的退貨手續怎麼辦理？', '20180827 11:00')
GO

INSERT INTO Opinions(CustomerID, EmployeeID, OpinionTitle, OpinionContent, OpinionDateTime, OpinionStatus, OpinionFeedback, OpinionFeedbackTime)
VALUES(4, 4, N'已回答範例', N'我想要退貨，請問要怎麼退？', '20180827 16:00', 1, N'謝謝您購買本公司產品，在本公司網站的最下方有FAQ，裏面第1項有提供相關訊息，謝謝您。', '20180828 09:30')
GO


--顧客ID 訂單日期時間 訂單地址 訂單明細編號 訂單明細國家
INSERT INTO Orders(CustomerID,OrderDateTime,OrderShipAddress,OrderShipPostalCode,OrderShipCountry)
VALUES(1,'20180820 12:00', N'台中市南屯區公益路二段51號','100','TW')

INSERT INTO Orders(CustomerID,OrderDateTime,OrderShipAddress,OrderShipPostalCode,OrderShipCountry)
VALUES(2,'20180821 13:30', N'Blake Gerold 128 N 37th St.','101','US')

INSERT INTO Orders(CustomerID,OrderDateTime,OrderShipAddress,OrderShipPostalCode,OrderShipCountry)
VALUES(3,'20180823 15:20', N'神奈川県横浜市泉区白百合2-10-14','102','JP')

INSERT INTO Orders(CustomerID,OrderDateTime,OrderShipAddress,OrderShipPostalCode,OrderShipCountry)
VALUES(4,'20180824 11:20', N'台北市忠孝北路三段2號','103','TW')
GO


--訂單ID 產品ID 數量 折扣
INSERT INTO OrderDetails(OrderID,ProductID,Quantity, UnitPrice)
VALUES(1, 2, 2, 200)
INSERT INTO OrderDetails(OrderID,ProductID,Quantity, UnitPrice)
VALUES(1, 3, 1, 200)
INSERT INTO OrderDetails(OrderID,ProductID,Quantity, UnitPrice)
VALUES(1, 4, 3, 200)

INSERT INTO OrderDetails(OrderID,ProductID,Quantity, UnitPrice)
VALUES(2, 3, 4, 200)

INSERT INTO OrderDetails(OrderID,ProductID,Quantity, UnitPrice)
VALUES(3, 3, 1, 200)
INSERT INTO OrderDetails(OrderID,ProductID,Quantity, UnitPrice)
VALUES(3, 4, 1, 200)

INSERT INTO OrderDetails(OrderID,ProductID,Quantity, UnitPrice)
VALUES(4, 2, 4, 200)
GO



INSERT INTO OptionalItemImages(ProductID, OptionalItemImageLocation, OptionalItemImageWidth ,OptionalItemImageTop, OptionalItemImageLeft)
VALUES(2, '/product_images/optional_items/cam_wide.png', 90, 70, 250 )

INSERT INTO OptionalItemImages(ProductID, OptionalItemImageLocation, OptionalItemImageWidth ,OptionalItemImageTop, OptionalItemImageLeft)
VALUES(5, '/product_images/optional_items/sensor_humi_temp.png', 30, 50, 30 )

INSERT INTO OptionalItemImages(ProductID, OptionalItemImageLocation, OptionalItemImageWidth ,OptionalItemImageTop, OptionalItemImageLeft)
VALUES(7, '/product_images/optional_items/feeder_small.png', 90, 140, 190 )

GO