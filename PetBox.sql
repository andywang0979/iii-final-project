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
CustomerRole INT DEFAULT 1
)
GO

--OpinionStatus 不需要空值 是因為客戶寄來是0 解決以後是1

CREATE TABLE Opinions(
OpinionID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
CustomerID INT NOT NULL,
EmployeeID INT DEFAULT 0,
ProductID INT NOT NULL DEFAULT 0,
OpinionTitle NVARCHAR(255),
OpinionContent NVARCHAR(1000),
OpinionStatus INT NOT NULL DEFAULT 0,
OpinionDateTime CHAR(16),
)
GO


CREATE TABLE Products (
    ProductID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    ProductCode VARCHAR(10),
    ProductName NVARCHAR(100),
	ProductDescription NVARCHAR(500),
	CategoryID INT NOT NULL,
	ProductImageLocation VARCHAR(255),
	ProductQuantity INT,
	Productunit NVARCHAR(50),
	ProductPrice INT
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
	EmployeeRole INT DEFAULT 0
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
    OrderID INT NOT NULL,
	ProductID INT NOT NULL,
	Quantity INT NOT NULL,
	Discount REAL DEFAULT 0
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


--刪除表
--DROP DATABASE Customers

--搜尋
--select * from Customers
--GO

--這個區塊是增加資料





-- to set UserRoleID explicitly, I have to turn the IDENTITY_INSERT ON, and OFF.
-- dummy 這個角色值並不能登入使用網站，只是用來處理

SET IDENTITY_INSERT UserRoles ON;  
GO
INSERT INTO UserRoles (UserRoleID, UserRoleName, UserRoleDescription)
VALUES(0, 'dummy', 'this role can not do anything, just for a default value');
SET IDENTITY_INSERT UserRoles OFF;  
GO

-- 需要增加其他角色：顧客、上架人員、客服人員

INSERT INTO UserRoles (UserRoleName, UserRoleDescription)
VALUES('customer-nonmember', 'customers who do not login in yet');


--


-- 這個員工帳號無法做任何事，只是用來當成預設值，像是Opinion中的EmployeeId的預設值為0，就是這個帳號。

SET IDENTITY_INSERT Employees ON;  
GO
INSERT INTO Employees(EmployeeID, EmployeeLoginName, EmployeePassword, EmployeeRole)
VALUES(0, 'nobody', 'nobody', 0)
SET IDENTITY_INSERT Employees OFF;  
GO



--

INSERT INTO Customers(CustomerLoginName,CustomerName,CustomerEmail,CustomerPassword,CustomerMobilePhone,CustomerAddress,CustomerPostalCode,CustomerCountry)
VALUES('Abc','John','abc@yahoo.com','12345','09123456','台中市南屯區公益路二段51號','123','TW')
GO

--類別名 說明 圖片位子
INSERT INTO Categories(CategoryName,CategoryDescription,CategoryPictureLocation)
VALUES('鏡頭','監控用','C:\img')

INSERT INTO Categories(CategoryName,CategoryDescription,CategoryPictureLocation)
VALUES('溫濕度感測器','溫度調節','C:\img')

INSERT INTO Categories(CategoryName,CategoryDescription,CategoryPictureLocation)
VALUES('自動餵食器','定時餵食','C:\img')
GO

--用戶角色名子 說明

INSERT INTO UserRoles(UserRoleName,UserRoleDescription)
VALUES('Customer-member','member')

INSERT INTO UserRoles(UserRoleName,UserRoleDescription)
VALUES('Shelf','Shelf People')

INSERT INTO UserRoles(UserRoleName,UserRoleDescription)
VALUES('Customer service','Customer service People')
GO

--帳號 姓名 信箱 密碼 手機 地址 郵遞區號 國家
INSERT INTO Customers(CustomerLoginName,CustomerName,CustomerEmail,CustomerPassword,CustomerMobilePhone,CustomerAddress,CustomerPostalCode,CustomerCountry,CustomerRole)
VALUES('abc','John','abc@yahoo.com','xyz',09123456,'台中市南屯區公益路二段51號',200,'TW',2)

INSERT INTO Customers(CustomerLoginName,CustomerName,CustomerEmail,CustomerPassword,CustomerMobilePhone,CustomerAddress,CustomerPostalCode,CustomerCountry,CustomerRole)
VALUES('aaa','Tom','aaa@yahoo.com','jqk',408-321-3333,'Blake Gerold 128 N 37th St.',300,'USA',3)

INSERT INTO Customers(CustomerLoginName,CustomerName,CustomerEmail,CustomerPassword,CustomerMobilePhone,CustomerAddress,CustomerPostalCode,CustomerCountry,CustomerRole)
VALUES('bcd','Pan','bcd@yahoo.com','xyz',819-064-75321,'神奈川県横浜市泉区白百合2-10-14',400,'JP',4)

INSERT INTO Customers(CustomerLoginName,CustomerName,CustomerEmail,CustomerPassword,CustomerMobilePhone,CustomerAddress,CustomerPostalCode,CustomerCountry,CustomerRole)
VALUES('cvb','Max','cvb@yahoo.com','tyu',0977817442,'台北市忠孝北路三段2號',450,'TW',5)
GO

--帳號 密碼 角色
INSERT INTO Employees(EmployeeLoginName,EmployeePassword,EmployeeRole)
VALUES('xyz','wxyz',4)

INSERT INTO Employees(EmployeeLoginName,EmployeePassword,EmployeeRole)
VALUES('jkl','hjkl',4)

INSERT INTO Employees(EmployeeLoginName,EmployeePassword,EmployeeRole)
VALUES('asd','asdf',5)
GO

--顧客ID 員工ID 產品ID 意見標題 內容 狀況 時間
INSERT INTO Opinions(CustomerID,EmployeeID,ProductID,OpinionTitle,OpinionContent,OpinionStatus,OpinionDateTime)
VALUES(1,3,1,'到貨時間?','什時到貨','','20180825 12:00')

INSERT INTO Opinions(CustomerID,EmployeeID,ProductID,OpinionTitle,OpinionContent,OpinionStatus,OpinionDateTime)
VALUES(2,3,2,'產品怎麼操作?','有沒有教學','','20180826 15:00')

INSERT INTO Opinions(CustomerID,EmployeeID,ProductID,OpinionTitle,OpinionContent,OpinionStatus,OpinionDateTime)
VALUES(3,3,3,'要求退貨?','買錯商品','','20180827 11:00')
GO

--產品代碼 產品名子 產品說明 類別ID 產品圖片位子 產品數量 產品單位 產品價格
INSERT INTO Products(ProductCode,ProductName,ProductDescription,CategoryID,ProductImageLocation,ProductQuantity,Productunit,ProductPrice)
VALUES('1','監控鏡頭','監控畫面',1,'C:\img',10,'台',250)

INSERT INTO Products(ProductCode,ProductName,ProductDescription,CategoryID,ProductImageLocation,ProductQuantity,Productunit,ProductPrice)
VALUES('2','溫濕度感測器','調節溫度',2,'C:\img',15,'個',100)

INSERT INTO Products(ProductCode,ProductName,ProductDescription,CategoryID,ProductImageLocation,ProductQuantity,Productunit,ProductPrice)
VALUES('3','自動餵食器','定時餵食',3,'C:\img',20,'組',180)
GO

--訂單ID 產品ID 數量 折扣
INSERT INTO OrderDetails(OrderID,ProductID,Quantity)
VALUES(1,1,10)

INSERT INTO OrderDetails(OrderID,ProductID,Quantity)
VALUES(2,2,18)

INSERT INTO OrderDetails(OrderID,ProductID,Quantity)
VALUES(3,3,12)

INSERT INTO OrderDetails(OrderID,ProductID,Quantity)
VALUES(4,1,15)
GO

--顧客ID 訂單日期時間 訂單地址 訂單明細編號 訂單明細國家
INSERT INTO Orders(CustomerID,OrderDateTime,OrderShipAddress,OrderShipPostalCode,OrderShipCountry)
VALUES(1,'20180820 12:00','台中市南屯區公益路二段51號','100','TW')

INSERT INTO Orders(CustomerID,OrderDateTime,OrderShipAddress,OrderShipPostalCode,OrderShipCountry)
VALUES(2,'20180821 13:30','Blake Gerold 128 N 37th St.','101','US')

INSERT INTO Orders(CustomerID,OrderDateTime,OrderShipAddress,OrderShipPostalCode,OrderShipCountry)
VALUES(3,'20180823 15:20','神奈川県横浜市泉区白百合2-10-14','102','JP')

INSERT INTO Orders(CustomerID,OrderDateTime,OrderShipAddress,OrderShipPostalCode,OrderShipCountry)
VALUES(4,'20180824 11:20','台北市忠孝北路三段2號','103','TW')
GO







--測試刪除表中資料
--DELETE FROM Customers
--WHERE CustomerID = 1
--GO

--測試新增欄位 刪除欄位
--ALTER TABLE Customers ADD ABC varchar(10)
--GO

--alter table Customers drop column ABC
--GO
