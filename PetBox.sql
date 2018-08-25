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








--測試刪除表中資料
--DELETE FROM Customers
--WHERE CustomerID = 1
--GO

--測試新增欄位 刪除欄位
--ALTER TABLE Customers ADD ABC varchar(10)
--GO

--alter table Customers drop column ABC
--GO
