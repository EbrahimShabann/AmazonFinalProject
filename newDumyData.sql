use AmazonDBNew
GO

-- Insert Roles
INSERT INTO [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
VALUES 
('role_admin', 'Admin', 'ADMIN', NEWID()),
('role_seller', 'Seller', 'SELLER', NEWID()),
('role_customer', 'Customer', 'CUSTOMER', NEWID()),
('role_support', 'Support', 'SUPPORT', NEWID());
-- Add more if needed, but 4 roles are enough
GO
-- Insert Users
DECLARE @Users TABLE (Id NVARCHAR(450), Email NVARCHAR(256), UserName NVARCHAR(256))

INSERT INTO [dbo].[Users] 
([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], 
[PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumberConfirmed], 
[TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount], [created_at], [last_login], [is_active], [is_deleted])
OUTPUT inserted.Id, inserted.Email, inserted.UserName INTO @Users
VALUES
-- Admin
(NEWID(), 'admin@amazon.com', 'ADMIN@AMAZON.COM', 'admin@amazon.com', 'ADMIN@AMAZON.COM', 1, 'hash1', 'stamp1', 'concur1', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
-- Sellers (5)
(NEWID(), 'seller1@amazon.com', 'SELLER1@AMAZON.COM', 'seller1@amazon.com', 'SELLER1@AMAZON.COM', 1, 'hash2', 'stamp2', 'concur2', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
(NEWID(), 'seller2@amazon.com', 'SELLER2@AMAZON.COM', 'seller2@amazon.com', 'SELLER2@AMAZON.COM', 1, 'hash3', 'stamp3', 'concur3', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
(NEWID(), 'seller3@amazon.com', 'SELLER3@AMAZON.COM', 'seller3@amazon.com', 'SELLER3@AMAZON.COM', 1, 'hash4', 'stamp4', 'concur4', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
(NEWID(), 'seller4@amazon.com', 'SELLER4@AMAZON.COM', 'seller4@amazon.com', 'SELLER4@AMAZON.COM', 1, 'hash5', 'stamp5', 'concur5', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
(NEWID(), 'seller5@amazon.com', 'SELLER5@AMAZON.COM', 'seller5@amazon.com', 'SELLER5@AMAZON.COM', 1, 'hash6', 'stamp6', 'concur6', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
-- Customers (10)
(NEWID(), 'cust1@amazon.com', 'CUST1@AMAZON.COM', 'cust1@amazon.com', 'CUST1@AMAZON.COM', 1, 'hash7', 'stamp7', 'concur7', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
(NEWID(), 'cust2@amazon.com', 'CUST2@AMAZON.COM', 'cust2@amazon.com', 'CUST2@AMAZON.COM', 1, 'hash8', 'stamp8', 'concur8', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
(NEWID(), 'cust3@amazon.com', 'CUST3@AMAZON.COM', 'cust3@amazon.com', 'CUST3@AMAZON.COM', 1, 'hash9', 'stamp9', 'concur9', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
(NEWID(), 'cust4@amazon.com', 'CUST4@AMAZON.COM', 'cust4@amazon.com', 'CUST4@AMAZON.COM', 1, 'hash10', 'stamp10', 'concur10', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
(NEWID(), 'cust5@amazon.com', 'CUST5@AMAZON.COM', 'cust5@amazon.com', 'CUST5@AMAZON.COM', 1, 'hash11', 'stamp11', 'concur11', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
(NEWID(), 'cust6@amazon.com', 'CUST6@AMAZON.COM', 'cust6@amazon.com', 'CUST6@AMAZON.COM', 1, 'hash12', 'stamp12', 'concur12', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
(NEWID(), 'cust7@amazon.com', 'CUST7@AMAZON.COM', 'cust7@amazon.com', 'CUST7@AMAZON.COM', 1, 'hash13', 'stamp13', 'concur13', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
(NEWID(), 'cust8@amazon.com', 'CUST8@AMAZON.COM', 'cust8@amazon.com', 'CUST8@AMAZON.COM', 1, 'hash14', 'stamp14', 'concur14', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
(NEWID(), 'cust9@amazon.com', 'CUST9@AMAZON.COM', 'cust9@amazon.com', 'CUST9@AMAZON.COM', 1, 'hash15', 'stamp15', 'concur15', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
(NEWID(), 'cust10@amazon.com', 'CUST10@AMAZON.COM', 'cust10@amazon.com', 'CUST10@AMAZON.COM', 1, 'hash16', 'stamp16', 'concur16', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
-- Support Staff (2)
(NEWID(), 'support1@amazon.com', 'SUPPORT1@AMAZON.COM', 'support1@amazon.com', 'SUPPORT1@AMAZON.COM', 1, 'hash17', 'stamp17', 'concur17', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0),
(NEWID(), 'support2@amazon.com', 'SUPPORT2@AMAZON.COM', 'support2@amazon.com', 'SUPPORT2@AMAZON.COM', 1, 'hash18', 'stamp18', 'concur18', 1, 0, 1, 0, GETDATE(), GETDATE(), 1, 0);

-- Store inserted IDs
SELECT * INTO #Users FROM @Users;
go
--*********************************************************************
-- Assign roles
INSERT INTO [dbo].[UserRoles] ([UserId], [RoleId])
SELECT Id, 'role_customer' FROM #Users WHERE UserName LIKE 'cust%'
UNION ALL
SELECT Id, 'role_seller' FROM #Users WHERE UserName LIKE 'seller%'
UNION ALL
SELECT Id, 'role_admin' FROM #Users WHERE UserName = 'admin@amazon.com'
UNION ALL
SELECT Id, 'role_support' FROM #Users WHERE UserName LIKE 'support%';
go

-- Insert Categories
DECLARE @Categories TABLE (Id NVARCHAR(450), Name NVARCHAR(255))

INSERT INTO [dbo].[categories] 
([id], [name], [description], [image_url], [parent_category_id], [created_by], [created_at], [is_active], [is_deleted])
OUTPUT inserted.Id, inserted.Name INTO @Categories
VALUES
-- Parent categories
(NEWID(), 'Electronics', 'All electronic gadgets', 'https://pics/electronics.jpg', NULL, (SELECT Id FROM #Users WHERE UserName = 'admin@amazon.com'), GETDATE(), 1, 0),
(NEWID(), 'Clothing', 'Men and women apparel', 'https://pics/clothing.jpg', NULL, (SELECT Id FROM #Users WHERE UserName = 'admin@amazon.com'), GETDATE(), 1, 0),
(NEWID(), 'Books', 'Fiction, non-fiction, etc.', 'https://pics/books.jpg', NULL, (SELECT Id FROM #Users WHERE UserName = 'admin@amazon.com'), GETDATE(), 1, 0),
(NEWID(), 'Home & Kitchen', 'Home appliances and decor', 'https://pics/home.jpg', NULL, (SELECT Id FROM #Users WHERE UserName = 'admin@amazon.com'), GETDATE(), 1, 0),
(NEWID(), 'Toys & Games', 'Children and family games', 'https://pics/toys.jpg', NULL, (SELECT Id FROM #Users WHERE UserName = 'admin@amazon.com'), GETDATE(), 1, 0);

-- Subcategories
INSERT INTO [dbo].[categories] 
([id], [name], [description], [image_url], [parent_category_id], [created_by], [created_at], [is_active], [is_deleted])
SELECT NEWID(), 'Smartphones', 'Latest smartphones', 'https://pics/phones.jpg', Id, (SELECT Id FROM #Users WHERE UserName = 'admin@amazon.com'), GETDATE(), 1, 0 FROM @Categories WHERE Name = 'Electronics'
UNION ALL
SELECT NEWID(), 'Laptops', 'Laptops and notebooks', 'https://pics/laptops.jpg', Id, (SELECT Id FROM #Users WHERE UserName = 'admin@amazon.com'), GETDATE(), 1, 0 FROM @Categories WHERE Name = 'Electronics'
UNION ALL
SELECT NEWID(), 'T-Shirts', 'Casual wear', 'https://pics/tshirts.jpg', Id, (SELECT Id FROM #Users WHERE UserName = 'admin@amazon.com'), GETDATE(), 1, 0 FROM @Categories WHERE Name = 'Clothing'
UNION ALL
SELECT NEWID(), 'Fiction', 'Novels and stories', 'https://pics/fiction.jpg', Id, (SELECT Id FROM #Users WHERE UserName = 'admin@amazon.com'), GETDATE(), 1, 0 FROM @Categories WHERE Name = 'Books'
UNION ALL
SELECT NEWID(), 'Cookware', 'Pots, pans, etc.', 'https://pics/cookware.jpg', Id, (SELECT Id FROM #Users WHERE UserName = 'admin@amazon.com'), GETDATE(), 1, 0 FROM @Categories WHERE Name = 'Home & Kitchen'
UNION ALL
SELECT NEWID(), 'Action Figures', 'Superheroes and characters', 'https://pics/figures.jpg', Id, (SELECT Id FROM #Users WHERE UserName = 'admin@amazon.com'), GETDATE(), 1, 0 FROM @Categories WHERE Name = 'Toys & Games';

-- Add more to reach 20
;WITH MoreCats AS (
    SELECT TOP 11 ROW_NUMBER() OVER (ORDER BY Id) AS n, * FROM @Categories
)
INSERT INTO [dbo].[categories] 
([id], [name], [description], [image_url], [parent_category_id], [created_by], [created_at], [is_active], [is_deleted])
SELECT NEWID(), CONCAT('Category-', n), 'Dummy category', 'https://pics/dummy.jpg', NULL, (SELECT Id FROM #Users WHERE UserName = 'admin@amazon.com'), GETDATE(), 1, 0 FROM MoreCats;

go
-- Insert Products
INSERT INTO [dbo].[products] 
([id], [name], [description], [price], [Brand], [Colors], [Sizes], [stock_quantity], [sku], [category_id], [seller_id], [created_at], [is_active], [is_approved], [is_deleted])
SELECT 
NEWID(),
CONCAT('Product-', ROW_NUMBER() OVER (ORDER BY u.Id)),
CONCAT('Description for product ', ROW_NUMBER() OVER (ORDER BY u.Id)),
ROUND((RAND(CHECKSUM(NEWID())) * 1000), 2),
'Brand-' + CAST(ROW_NUMBER() OVER (ORDER BY u.Id) % 5 + 1 AS VARCHAR),
'Red,Blue,Green',
'S,M,L',
ABS(CHECKSUM(NEWID()) % 100),
'SKU-' + RIGHT('0000' + CAST(ROW_NUMBER() OVER (ORDER BY u.Id) AS VARCHAR), 4),
c.Id,
u.Id,
GETDATE(),
1, 1, 0
FROM (SELECT TOP 20 Id FROM #Users WHERE UserName LIKE 'seller%') u
CROSS JOIN (SELECT TOP 20 Id FROM [dbo].[categories]) c
ORDER BY NEWID()
OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;
go
INSERT INTO [dbo].[discounts] 
([id], [description], [discount_type], [value], [start_date], [end_date], [minimum_order_amount], [max_uses], [current_uses], [is_active], [is_deleted], [seller_id], [created_at])
SELECT 
NEWID(),
'10% off on electronics',
'Percentage',
10.00,
GETDATE(),
DATEADD(day, 30, GETDATE()),
50.00,
100,
0,
1,
0,
u.Id,
GETDATE()
FROM (SELECT TOP 20 Id FROM #Users WHERE UserName LIKE 'seller%') u
CROSS JOIN (SELECT TOP 20 id FROM [dbo].[products]) p
ORDER BY NEWID()
OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;
go

INSERT INTO [dbo].[product_discounts] ([id], [product_id], [discount_id])
SELECT NEWID(), p.id, d.id
FROM (SELECT TOP 20 id FROM [dbo].[products] ORDER BY NEWID()) p
CROSS JOIN (SELECT TOP 20 id FROM [dbo].[discounts] ORDER BY NEWID()) d
ORDER BY NEWID()
OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;
go

INSERT INTO [dbo].[shopping_carts] ([id], [user_id], [created_at], [last_updated_at])
SELECT NEWID(), Id, GETDATE(), GETDATE()
FROM #Users
WHERE UserName LIKE 'cust%' OR UserName LIKE 'seller%'
ORDER BY NEWID()
OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;
go

INSERT INTO [dbo].[cart_items] ([id], [cart_id], [product_id], [quantity], [added_at])
SELECT 
NEWID(),
c.id,
p.id,
ABS(CHECKSUM(NEWID()) % 5) + 1,
GETDATE()
FROM [dbo].[shopping_carts] c
CROSS JOIN (SELECT TOP 20 id FROM [dbo].[products]) p
ORDER BY NEWID()
OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;
go

-- Wishlists
INSERT INTO [dbo].[wishlists] ([id], [user_id], [created_at])
SELECT NEWID(), Id, GETDATE()
FROM #Users
WHERE UserName LIKE 'cust%'
ORDER BY NEWID()
OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;
go

-- Wishlist Items
INSERT INTO [dbo].[wishlist_items] ([id], [wishlist_id], [product_id], [added_at])
SELECT 
NEWID(),
w.id,
p.id,
GETDATE()
FROM [dbo].[wishlists] w
CROSS JOIN (SELECT TOP 20 id FROM [dbo].[products]) p
ORDER BY NEWID()
OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;
go

-- Orders
INSERT INTO [dbo].[orders] 
([id], [buyer_id], [order_date], [total_amount], [shipping_address], [billing_address], [status], [payment_method], [payment_status], [is_deleted])
SELECT 
NEWID(),
u.Id,
GETDATE(),
ROUND(RAND() * 500 + 10, 2),
'123 Main St, City',
'123 Main St, City',
'Shipped',
'Credit Card',
'Paid',
0
FROM #Users u
WHERE u.UserName LIKE 'cust%'
ORDER BY NEWID()
OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;

-- Order Items
INSERT INTO [dbo].[order_items] 
([id], [order_id], [seller_id], [product_id], [quantity], [unit_price], [status])
SELECT 
NEWID(),
o.id,
(SELECT TOP 1 Id FROM #Users WHERE UserName LIKE 'seller%'),
p.id,
ABS(CHECKSUM(NEWID()) % 3) + 1,
p.price,
'Delivered'
FROM [dbo].[orders] o
CROSS JOIN (SELECT TOP 20 id, price FROM [dbo].[products]) p
ORDER BY NEWID()
OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;
go

-- audit_logs
INSERT INTO [dbo].[audit_logs] ([id], [user_id], [action], [entity_type], [entity_id], [timestamp])
SELECT NEWID(), Id, 'Login', 'User', Id, GETDATE() FROM #Users ORDER BY NEWID() OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;

-- support_tickets
INSERT INTO [dbo].[support_tickets] ([id], [user_id], [subject], [description], [status], [priority], [created_at], [is_deleted])
SELECT NEWID(), Id, 'Help needed', 'I have an issue', 'Open', 'High', GETDATE(), 0 FROM #Users ORDER BY NEWID() OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;

-- ticket_messages
INSERT INTO [dbo].[ticket_messages] ([id], [ticket_id], [sender_id], [message], [sent_at], [is_read])
SELECT NEWID(), t.id, u.Id, 'Please help!', GETDATE(), 0 FROM [dbo].[support_tickets] t CROSS JOIN #Users u ORDER BY NEWID() OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;

-- chat_sessions
INSERT INTO [dbo].[chat_sessions] ([Id], [CustomerId], [SellerId], [CreatedAt], [Status], [IsDeleted])
SELECT NEWID(), c.Id, s.Id, GETDATE(), 'Active', 0 FROM (SELECT TOP 10 Id FROM #Users WHERE UserName LIKE 'cust%') c CROSS JOIN (SELECT TOP 10 Id FROM #Users WHERE UserName LIKE 'seller%') s ORDER BY NEWID() OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;

-- chat_messages
INSERT INTO [dbo].[chat_messages] ([id], [chat_sessionId], [sender_id], [message], [sent_at], [is_read])
SELECT NEWID(), cs.Id, u.Id, 'Hello!', GETDATE(), 1 FROM [dbo].[chat_sessions] cs CROSS JOIN #Users u ORDER BY NEWID() OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;

-- product_reviews
INSERT INTO [dbo].[product_reviews] ([id], [product_id], [user_id], [rating], [title], [comment], [created_at], [is_verified_purchase])
SELECT NEWID(), p.id, u.Id, ABS(CHECKSUM(NEWID()) % 5) + 1, 'Great!', 'Nice product', GETDATE(), 1 FROM [dbo].[products] p CROSS JOIN #Users u ORDER BY NEWID() OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;

-- review_reply
INSERT INTO [dbo].[review_reply] ([id], [review_id], [replier_id], [reply_text], [created_at], [is_seller_reply])
SELECT NEWID(), r.id, u.Id, 'Thank you!', GETDATE(), 1 FROM [dbo].[product_reviews] r CROSS JOIN #Users u ORDER BY NEWID() OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;

-- order_history
INSERT INTO [dbo].[order_history] ([id], [order_id], [status], [changed_by], [changed_at])
SELECT NEWID(), o.id, 'Shipped', u.Id, GETDATE() FROM [dbo].[orders] o CROSS JOIN #Users u ORDER BY NEWID() OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;

-- ticket_history
INSERT INTO [dbo].[ticket_history] ([id], [ticket_id], [changed_by], [changed_at], [field_changed], [old_value], [new_value])
SELECT NEWID(), t.id, u.Id, GETDATE(), 'Status', 'Open', 'In Progress' FROM [dbo].[support_tickets] t CROSS JOIN #Users u ORDER BY NEWID() OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;

-- AccountLog
INSERT INTO [dbo].[AccountLog] ([UserID], [ActionType], [AdditionalInfo])
SELECT Id, 'Login', 'User logged in' FROM #Users ORDER BY NEWID() OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;

-- Orders_Reverted
INSERT INTO [dbo].[Orders_Reverted] ([id], [orderId], [order_itemId], [RevertDate], [Reason], [Notes])
SELECT NEWID(), o.id, oi.id, GETDATE(), 'Wrong item', 'Reverted' FROM [dbo].[orders] o JOIN [dbo].[order_items] oi ON o.id = oi.order_id ORDER BY NEWID() OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;

-- product_images
INSERT INTO [dbo].[product_images] ([id], [product_id], [image_url], [is_primary], [uploaded_at])
SELECT NEWID(), p.id, 'https://pics/product.jpg', 1, GETDATE() FROM [dbo].[products] p ORDER BY NEWID() OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;
go


















