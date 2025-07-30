INSERT INTO [dbo].[Roles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
VALUES 
('1', 'Admin', 'ADMIN', NEWID()),
('2', 'Seller', 'SELLER', NEWID()),
('3', 'Customer', 'CUSTOMER', NEWID()),
('4', 'Support', 'SUPPORT', NEWID()),
('5', 'Moderator', 'MODERATOR', NEWID());
go 
INSERT INTO [dbo].[Users] ([Id], [profile_picture_url], [google_id], [date_of_birth], [created_at], [last_login], [is_active], [is_deleted], [deleted_at], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount])
VALUES
-- Admins
('a1', 'https://example.com/profiles/admin1.jpg', NULL, '1980-01-15', GETDATE(), GETDATE(), 1, 0, NULL, 'admin1', 'ADMIN1', 'admin1@example.com', 'ADMIN1@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567890', 1, 0, NULL, 1, 0),
('a2', 'https://example.com/profiles/admin2.jpg', NULL, '1985-05-20', GETDATE(), GETDATE(), 1, 0, NULL, 'admin2', 'ADMIN2', 'admin2@example.com', 'ADMIN2@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567891', 1, 0, NULL, 1, 0),

-- Sellers
('s1', 'https://example.com/profiles/seller1.jpg', NULL, '1990-03-10', GETDATE(), GETDATE(), 1, 0, NULL, 'seller1', 'SELLER1', 'seller1@example.com', 'SELLER1@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567892', 1, 0, NULL, 1, 0),
('s2', 'https://example.com/profiles/seller2.jpg', NULL, '1988-07-22', GETDATE(), GETDATE(), 1, 0, NULL, 'seller2', 'SELLER2', 'seller2@example.com', 'SELLER2@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567893', 1, 0, NULL, 1, 0),
('s3', 'https://example.com/profiles/seller3.jpg', NULL, '1992-11-05', GETDATE(), GETDATE(), 1, 0, NULL, 'seller3', 'SELLER3', 'seller3@example.com', 'SELLER3@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567894', 1, 0, NULL, 1, 0),
('s4', 'https://example.com/profiles/seller4.jpg', NULL, '1987-09-18', GETDATE(), GETDATE(), 1, 0, NULL, 'seller4', 'SELLER4', 'seller4@example.com', 'SELLER4@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567895', 1, 0, NULL, 1, 0),
('s5', 'https://example.com/profiles/seller5.jpg', NULL, '1991-04-30', GETDATE(), GETDATE(), 1, 0, NULL, 'seller5', 'SELLER5', 'seller5@example.com', 'SELLER5@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567896', 1, 0, NULL, 1, 0),

-- Customers
('c1', 'https://example.com/profiles/customer1.jpg', NULL, '1995-02-14', GETDATE(), GETDATE(), 1, 0, NULL, 'customer1', 'CUSTOMER1', 'customer1@example.com', 'CUSTOMER1@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567897', 1, 0, NULL, 1, 0),
('c2', 'https://example.com/profiles/customer2.jpg', NULL, '1993-06-25', GETDATE(), GETDATE(), 1, 0, NULL, 'customer2', 'CUSTOMER2', 'customer2@example.com', 'CUSTOMER2@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567898', 1, 0, NULL, 1, 0),
('c3', 'https://example.com/profiles/customer3.jpg', NULL, '1994-08-12', GETDATE(), GETDATE(), 1, 0, NULL, 'customer3', 'CUSTOMER3', 'customer3@example.com', 'CUSTOMER3@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567899', 1, 0, NULL, 1, 0),
('c4', 'https://example.com/profiles/customer4.jpg', NULL, '1996-12-03', GETDATE(), GETDATE(), 1, 0, NULL, 'customer4', 'CUSTOMER4', 'customer4@example.com', 'CUSTOMER4@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567800', 1, 0, NULL, 1, 0),
('c5', 'https://example.com/profiles/customer5.jpg', NULL, '1997-10-17', GETDATE(), GETDATE(), 1, 0, NULL, 'customer5', 'CUSTOMER5', 'customer5@example.com', 'CUSTOMER5@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567801', 1, 0, NULL, 1, 0),
('c6', 'https://example.com/profiles/customer6.jpg', NULL, '1998-01-28', GETDATE(), GETDATE(), 1, 0, NULL, 'customer6', 'CUSTOMER6', 'customer6@example.com', 'CUSTOMER6@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567802', 1, 0, NULL, 1, 0),
('c7', 'https://example.com/profiles/customer7.jpg', NULL, '1990-05-19', GETDATE(), GETDATE(), 1, 0, NULL, 'customer7', 'CUSTOMER7', 'customer7@example.com', 'CUSTOMER7@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567803', 1, 0, NULL, 1, 0),
('c8', 'https://example.com/profiles/customer8.jpg', NULL, '1991-07-22', GETDATE(), GETDATE(), 1, 0, NULL, 'customer8', 'CUSTOMER8', 'customer8@example.com', 'CUSTOMER8@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567804', 1, 0, NULL, 1, 0),
('c9', 'https://example.com/profiles/customer9.jpg', NULL, '1992-09-15', GETDATE(), GETDATE(), 1, 0, NULL, 'customer9', 'CUSTOMER9', 'customer9@example.com', 'CUSTOMER9@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567805', 1, 0, NULL, 1, 0),
('c10', 'https://example.com/profiles/customer10.jpg', NULL, '1993-11-08', GETDATE(), GETDATE(), 1, 0, NULL, 'customer10', 'CUSTOMER10', 'customer10@example.com', 'CUSTOMER10@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567806', 1, 0, NULL, 1, 0),

-- Support staff
('sp1', 'https://example.com/profiles/support1.jpg', NULL, '1986-04-05', GETDATE(), GETDATE(), 1, 0, NULL, 'support1', 'SUPPORT1', 'support1@example.com', 'SUPPORT1@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567807', 1, 0, NULL, 1, 0),
('sp2', 'https://example.com/profiles/support2.jpg', NULL, '1989-08-16', GETDATE(), GETDATE(), 1, 0, NULL, 'support2', 'SUPPORT2', 'support2@example.com', 'SUPPORT2@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAE...', NEWID(), NEWID(), '1234567808', 1, 0, NULL, 1, 0)


go



INSERT INTO [dbo].[UserRoles] ([UserId], [RoleId])
VALUES
-- Admins
('a1', '1'),
('a2', '1'),

-- Sellers
('s1', '2'),
('s2', '2'),
('s3', '2'),
('s4', '2'),
('s5', '2'),

-- Customers
('c1', '3'),
('c2', '3'),
('c3', '3'),
('c4', '3'),
('c5', '3'),
('c6', '3'),
('c7', '3'),
('c8', '3'),
('c9', '3'),
('c10', '3'),

-- Support staff
('sp1', '4'),
('sp2', '4')
 go


 INSERT INTO [dbo].[categories] ([id], [name], [description], [image_url], [parent_category_id], [created_by], [created_at], [last_modified_by], [last_modified_at], [is_active], [is_deleted], [deleted_by])
VALUES
-- Top-level categories
('cat1', 'Electronics', 'All electronic devices and accessories', 'https://example.com/images/electronics.jpg', NULL, 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat2', 'Clothing', 'Clothing for men, women and children', 'https://example.com/images/clothing.jpg', NULL, 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat3', 'Home & Kitchen', 'Home appliances and kitchenware', 'https://example.com/images/homekitchen.jpg', NULL, 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat4', 'Books', 'Books of all genres', 'https://example.com/images/books.jpg', NULL, 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat5', 'Toys & Games', 'Toys and games for all ages', 'https://example.com/images/toys.jpg', NULL, 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),

-- Subcategories for Electronics
('cat6', 'Smartphones', 'Latest smartphones and accessories', 'https://example.com/images/smartphones.jpg', 'cat1', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat7', 'Laptops', 'Laptops for work and gaming', 'https://example.com/images/laptops.jpg', 'cat1', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat8', 'TVs', 'Televisions of all sizes', 'https://example.com/images/tvs.jpg', 'cat1', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat9', 'Headphones', 'Wired and wireless headphones', 'https://example.com/images/headphones.jpg', 'cat1', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat10', 'Cameras', 'DSLRs, mirrorless and point-and-shoot cameras', 'https://example.com/images/cameras.jpg', 'cat1', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),

-- Subcategories for Clothing
('cat11', 'Men''s Clothing', 'Clothing for men', 'https://example.com/images/mensclothing.jpg', 'cat2', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat12', 'Women''s Clothing', 'Clothing for women', 'https://example.com/images/womensclothing.jpg', 'cat2', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat13', 'Kids'' Clothing', 'Clothing for children', 'https://example.com/images/kidsclothing.jpg', 'cat2', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat14', 'Shoes', 'Footwear for all', 'https://example.com/images/shoes.jpg', 'cat2', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat15', 'Accessories', 'Jewelry, watches and more', 'https://example.com/images/accessories.jpg', 'cat2', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),

-- Subcategories for Home & Kitchen
('cat16', 'Furniture', 'Home and office furniture', 'https://example.com/images/furniture.jpg', 'cat3', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat17', 'Kitchen Appliances', 'Microwaves, blenders and more', 'https://example.com/images/kitchenappliances.jpg', 'cat3', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat18', 'Home Decor', 'Decorative items for your home', 'https://example.com/images/homedecor.jpg', 'cat3', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat19', 'Bedding', 'Bed sheets, pillows and more', 'https://example.com/images/bedding.jpg', 'cat3', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat20', 'Lighting', 'Indoor and outdoor lighting', 'https://example.com/images/lighting.jpg', 'cat3', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),

-- Subcategories for Books
('cat21', 'Fiction', 'Novels and fiction books', 'https://example.com/images/fiction.jpg', 'cat4', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat22', 'Non-Fiction', 'Biographies, history and more', 'https://example.com/images/nonfiction.jpg', 'cat4', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat23', 'Children''s Books', 'Books for kids', 'https://example.com/images/childrensbooks.jpg', 'cat4', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat24', 'Textbooks', 'Educational books', 'https://example.com/images/textbooks.jpg', 'cat4', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat25', 'Cookbooks', 'Books with recipes', 'https://example.com/images/cookbooks.jpg', 'cat4', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),

-- Subcategories for Toys & Games
('cat26', 'Action Figures', 'Collectible action figures', 'https://example.com/images/actionfigures.jpg', 'cat5', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat27', 'Board Games', 'Classic and new board games', 'https://example.com/images/boardgames.jpg', 'cat5', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat28', 'Puzzles', 'Jigsaw puzzles for all ages', 'https://example.com/images/puzzles.jpg', 'cat5', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat29', 'Outdoor Toys', 'Toys for outdoor play', 'https://example.com/images/outdoortoys.jpg', 'cat5', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL),
('cat30', 'Video Games', 'Console and PC games', 'https://example.com/images/videogames.jpg', 'cat5', 'a1', GETDATE(), 'a1', GETDATE(), 1, 0, NULL);
go
INSERT INTO [dbo].[products] ([id], [name], [description], [price], [Brand], [Colors], [Sizes], [discount_price], [stock_quantity], [sku], [category_id], [seller_id], [created_at], [last_modified_at], [is_active], [is_approved], [approved_by], [approved_at], [is_deleted])
VALUES
-- Electronics - Smartphones
('p1', 'Premium Smartphone X', 'Latest flagship smartphone with 6.7" AMOLED display, 108MP camera', 999.99, 'TechMaster', 'Black,Blue,White', NULL, 899.99, 100, 'SMX-001', 'cat6', 's1', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p2', 'Budget Smartphone Y', 'Affordable smartphone with 6.5" LCD display, 48MP camera', 299.99, 'TechMaster', 'Black,Red', NULL, NULL, 200, 'SMY-001', 'cat6', 's1', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p3', 'Gaming Phone Z', 'High-performance gaming phone with 144Hz refresh rate', 799.99, 'GameTech', 'Black,Green', NULL, 749.99, 50, 'GPZ-001', 'cat6', 's2', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),

-- Electronics - Laptops
('p4', 'Ultrabook Pro', 'Thin and light laptop with 14" 4K display, 16GB RAM', 1299.99, 'UltraTech', 'Silver,Space Gray', NULL, 1199.99, 75, 'UBP-001', 'cat7', 's1', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p5', 'Gaming Laptop Extreme', 'Powerful gaming laptop with RTX 3080, 32GB RAM', 2499.99, 'GameTech', 'Black', NULL, NULL, 30, 'GLE-001', 'cat7', 's2', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p6', 'Budget Laptop Basic', 'Affordable laptop for everyday tasks', 499.99, 'BasicTech', 'Black,White', NULL, 449.99, 150, 'BLB-001', 'cat7', 's3', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),

-- Clothing - Men's
('p7', 'Men''s Casual T-Shirt', 'Comfortable cotton t-shirt for everyday wear', 19.99, 'FashionHub', 'Black,White,Blue,Red', 'S,M,L,XL,XXL', 15.99, 300, 'MCT-001', 'cat11', 's4', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p8', 'Men''s Formal Shirt', 'Premium dress shirt for office wear', 49.99, 'FashionHub', 'White,Blue,Black', 'S,M,L,XL', 39.99, 200, 'MFS-001', 'cat11', 's4', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p9', 'Men''s Jeans', 'Classic fit jeans with stretch fabric', 59.99, 'DenimCo', 'Blue,Black', '28,30,32,34,36', 49.99, 250, 'MJ-001', 'cat11', 's5', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),

-- Clothing - Women's
('p10', 'Women''s Summer Dress', 'Lightweight floral summer dress', 39.99, 'StyleQueen', 'Red,Blue,Yellow', 'XS,S,M,L,XL', 34.99, 180, 'WSD-001', 'cat12', 's4', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p11', 'Women''s Blouse', 'Elegant blouse for work or evening', 45.99, 'StyleQueen', 'White,Black,Pink', 'XS,S,M,L', NULL, 150, 'WB-001', 'cat12', 's4', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p12', 'Women''s Leggings', 'Comfortable yoga leggings with pocket', 29.99, 'ActiveWear', 'Black,Blue,Gray', 'XS,S,M,L,XL', 24.99, 220, 'WL-001', 'cat12', 's5', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),

-- Home & Kitchen - Furniture
('p13', 'Modern Sofa', '3-seater sofa with velvet upholstery', 899.99, 'HomeStyle', 'Gray,Blue,Green', NULL, 799.99, 20, 'MS-001', 'cat16', 's3', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p14', 'Dining Table Set', '6-seater dining table with chairs', 1299.99, 'HomeStyle', 'Walnut,White', NULL, NULL, 15, 'DTS-001', 'cat16', 's3', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p15', 'Bookshelf', '5-tier wooden bookshelf', 149.99, 'FurniCo', 'Oak,Black', NULL, 129.99, 40, 'BS-001', 'cat16', 's2', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),

-- Home & Kitchen - Appliances
('p16', 'Blender Pro', 'High-speed blender with 1200W motor', 129.99, 'KitchenMaster', 'Black,Red', NULL, 99.99, 80, 'BP-001', 'cat17', 's1', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p17', 'Air Fryer', '5.8QT digital air fryer with 8 presets', 89.99, 'KitchenMaster', 'Black,Silver', NULL, NULL, 60, 'AF-001', 'cat17', 's1', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p18', 'Coffee Maker', 'Programmable 12-cup coffee maker', 59.99, 'BrewTech', 'Black,White', NULL, 49.99, 100, 'CM-001', 'cat17', 's2', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),

-- Books
('p19', 'Best-Selling Novel', 'The latest best-selling fiction novel', 24.99, 'BookHouse', NULL, NULL, 19.99, 200, 'BN-001', 'cat21', 's5', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p20', 'Cookbook Collection', '500 recipes from around the world', 29.99, 'CulinaryPress', NULL, NULL, NULL, 150, 'CC-001', 'cat25', 's5', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p21', 'Children''s Picture Book', 'Colorful picture book for ages 3-6', 12.99, 'KidReads', NULL, NULL, 9.99, 300, 'CPB-001', 'cat23', 's5', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),

-- Toys & Games
('p22', 'Building Blocks Set', '200-piece educational building blocks', 39.99, 'ToyFun', 'Multicolor', NULL, 34.99, 120, 'BBS-001', 'cat26', 's3', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p23', 'Board Game Classic', 'Family board game for 2-6 players', 29.99, 'GameTime', NULL, NULL, NULL, 90, 'BGC-001', 'cat27', 's3', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p24', 'Action Figure Set', 'Set of 5 popular action figures', 49.99, 'HeroToys', NULL, NULL, 44.99, 70, 'AFS-001', 'cat26', 's2', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),

-- More products to reach 30+
('p25', 'Wireless Earbuds', 'True wireless earbuds with 20hr battery', 79.99, 'SoundTech', 'Black,White', NULL, 69.99, 180, 'WEB-001', 'cat9', 's1', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p26', 'Smart Watch', 'Fitness tracker with heart rate monitor', 129.99, 'TechMaster', 'Black,Silver', NULL, 119.99, 95, 'SW-001', 'cat9', 's1', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p27', 'Men''s Running Shoes', 'Lightweight running shoes with cushioning', 89.99, 'RunFast', 'Black,Blue,Red', '7,8,9,10,11,12', 79.99, 140, 'MRS-001', 'cat14', 's4', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p28', 'Women''s Sneakers', 'Casual sneakers for everyday wear', 69.99, 'WalkEasy', 'White,Pink,Black', '5,6,7,8,9', 59.99, 160, 'WSN-001', 'cat14', 's4', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p29', 'Throw Pillow Set', 'Set of 2 decorative throw pillows', 39.99, 'HomeDecor', 'Gray,Blue,Yellow', NULL, 34.99, 85, 'TPS-001', 'cat18', 's3', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0),
('p30', 'Desk Lamp', 'Adjustable LED desk lamp', 29.99, 'LightWorks', 'Black,White', NULL, 24.99, 110, 'DL-001', 'cat20', 's2', GETDATE(), GETDATE(), 1, 1, 'a1', GETDATE(), 0);

go
INSERT INTO [dbo].[product_images] ([id], [product_id], [image_url], [is_primary], [uploaded_at])
VALUES
-- Product 1 images
('pi1', 'p1', 'https://example.com/products/p1-1.jpg', 1, GETDATE()),
('pi2', 'p1', 'https://example.com/products/p1-2.jpg', 0, GETDATE()),
('pi3', 'p1', 'https://example.com/products/p1-3.jpg', 0, GETDATE()),

-- Product 2 images
('pi4', 'p2', 'https://example.com/products/p2-1.jpg', 1, GETDATE()),
('pi5', 'p2', 'https://example.com/products/p2-2.jpg', 0, GETDATE()),

-- Product 3 images
('pi6', 'p3', 'https://example.com/products/p3-1.jpg', 1, GETDATE()),
('pi7', 'p3', 'https://example.com/products/p3-2.jpg', 0, GETDATE()),
('pi8', 'p3', 'https://example.com/products/p3-3.jpg', 0, GETDATE()),

-- Product 4 images
('pi9', 'p4', 'https://example.com/products/p4-1.jpg', 1, GETDATE()),
('pi10', 'p4', 'https://example.com/products/p4-2.jpg', 0, GETDATE()),

-- Product 5 images
('pi11', 'p5', 'https://example.com/products/p5-1.jpg', 1, GETDATE()),
('pi12', 'p5', 'https://example.com/products/p5-2.jpg', 0, GETDATE()),

-- Product 6 images
('pi13', 'p6', 'https://example.com/products/p6-1.jpg', 1, GETDATE()),
('pi14', 'p6', 'https://example.com/products/p6-2.jpg', 0, GETDATE()),

-- Product 7 images
('pi15', 'p7', 'https://example.com/products/p7-1.jpg', 1, GETDATE()),
('pi16', 'p7', 'https://example.com/products/p7-2.jpg', 0, GETDATE()),

-- Product 8 images
('pi17', 'p8', 'https://example.com/products/p8-1.jpg', 1, GETDATE()),
('pi18', 'p8', 'https://example.com/products/p8-2.jpg', 0, GETDATE()),

-- Product 9 images
('pi19', 'p9', 'https://example.com/products/p9-1.jpg', 1, GETDATE()),
('pi20', 'p9', 'https://example.com/products/p9-2.jpg', 0, GETDATE()),

-- Product 10 images
('pi21', 'p10', 'https://example.com/products/p10-1.jpg', 1, GETDATE()),
('pi22', 'p10', 'https://example.com/products/p10-2.jpg', 0, GETDATE()),
('pi23', 'p10', 'https://example.com/products/p10-3.jpg', 0, GETDATE()),

-- Product 11 images
('pi24', 'p11', 'https://example.com/products/p11-1.jpg', 1, GETDATE()),
('pi25', 'p11', 'https://example.com/products/p11-2.jpg', 0, GETDATE()),

-- Product 12 images
('pi26', 'p12', 'https://example.com/products/p12-1.jpg', 1, GETDATE()),
('pi27', 'p12', 'https://example.com/products/p12-2.jpg', 0, GETDATE()),

-- Product 13 images
('pi28', 'p13', 'https://example.com/products/p13-1.jpg', 1, GETDATE()),
('pi29', 'p13', 'https://example.com/products/p13-2.jpg', 0, GETDATE()),

-- Product 14 images
('pi30', 'p14', 'https://example.com/products/p14-1.jpg', 1, GETDATE()),
('pi31', 'p14', 'https://example.com/products/p14-2.jpg', 0, GETDATE()),

-- Product 15 images
('pi32', 'p15', 'https://example.com/products/p15-1.jpg', 1, GETDATE()),
('pi33', 'p15', 'https://example.com/products/p15-2.jpg', 0, GETDATE()),

-- Product 16 images
('pi34', 'p16', 'https://example.com/products/p16-1.jpg', 1, GETDATE()),
('pi35', 'p16', 'https://example.com/products/p16-2.jpg', 0, GETDATE()),

-- Product 17 images
('pi36', 'p17', 'https://example.com/products/p17-1.jpg', 1, GETDATE()),
('pi37', 'p17', 'https://example.com/products/p17-2.jpg', 0, GETDATE()),

-- Product 18 images
('pi38', 'p18', 'https://example.com/products/p18-1.jpg', 1, GETDATE()),
('pi39', 'p18', 'https://example.com/products/p18-2.jpg', 0, GETDATE()),

-- Product 19 images
('pi40', 'p19', 'https://example.com/products/p19-1.jpg', 1, GETDATE()),
('pi41', 'p19', 'https://example.com/products/p19-2.jpg', 0, GETDATE()),

-- Product 20 images
('pi42', 'p20', 'https://example.com/products/p20-1.jpg', 1, GETDATE()),
('pi43', 'p20', 'https://example.com/products/p20-2.jpg', 0, GETDATE()),

-- Product 21 images
('pi44', 'p21', 'https://example.com/products/p21-1.jpg', 1, GETDATE()),
('pi45', 'p21', 'https://example.com/products/p21-2.jpg', 0, GETDATE()),

-- Product 22 images
('pi46', 'p22', 'https://example.com/products/p22-1.jpg', 1, GETDATE()),
('pi47', 'p22', 'https://example.com/products/p22-2.jpg', 0, GETDATE()),

-- Product 23 images
('pi48', 'p23', 'https://example.com/products/p23-1.jpg', 1, GETDATE()),
('pi49', 'p23', 'https://example.com/products/p23-2.jpg', 0, GETDATE()),

-- Product 24 images
('pi50', 'p24', 'https://example.com/products/p24-1.jpg', 1, GETDATE()),
('pi51', 'p24', 'https://example.com/products/p24-2.jpg', 0, GETDATE()),

-- Product 25 images
('pi52', 'p25', 'https://example.com/products/p25-1.jpg', 1, GETDATE()),
('pi53', 'p25', 'https://example.com/products/p25-2.jpg', 0, GETDATE()),

-- Product 26 images
('pi54', 'p26', 'https://example.com/products/p26-1.jpg', 1, GETDATE()),
('pi55', 'p26', 'https://example.com/products/p26-2.jpg', 0, GETDATE()),

-- Product 27 images
('pi56', 'p27', 'https://example.com/products/p27-1.jpg', 1, GETDATE()),
('pi57', 'p27', 'https://example.com/products/p27-2.jpg', 0, GETDATE()),

-- Product 28 images
('pi58', 'p28', 'https://example.com/products/p28-1.jpg', 1, GETDATE()),
('pi59', 'p28', 'https://example.com/products/p28-2.jpg', 0, GETDATE()),

-- Product 29 images
('pi60', 'p29', 'https://example.com/products/p29-1.jpg', 1, GETDATE()),
('pi61', 'p29', 'https://example.com/products/p29-2.jpg', 0, GETDATE()),

-- Product 30 images
('pi62', 'p30', 'https://example.com/products/p30-1.jpg', 1, GETDATE()),
('pi63', 'p30', 'https://example.com/products/p30-2.jpg', 0, GETDATE());
go

INSERT INTO [dbo].[discounts] ([id], [description], [discount_type], [value], [start_date], [end_date], [minimum_order_amount], [max_uses], [current_uses], [is_active], [seller_id], [created_at])
VALUES
-- Percentage discounts
('d1', 'Summer Sale - 20% off', 'Percentage', 20.00, '2025-06-01', '2025-06-30', 50.00, 1000, 342, 1, 's1', GETDATE()),
('d2', 'Clearance - 30% off', 'Percentage', 30.00, '2025-07-01', '2025-07-31', NULL, 500, 127, 1, 's1', GETDATE()),
('d3', 'New Customer - 15% off', 'Percentage', 15.00, '2025-01-01', '2025-12-31', NULL, 2000, 856, 1, 's2', GETDATE()),
('d4', 'Holiday Special - 25% off', 'Percentage', 25.00, '2025-12-01', '2025-12-31', 100.00, 800, 0, 0, 's2', GETDATE()),
('d5', 'Flash Sale - 40% off', 'Percentage', 40.00, '2025-08-15', '2025-08-16', NULL, 300, 0, 0, 's3', GETDATE()),

-- Fixed amount discounts
('d6', '$10 off Electronics', 'Fixed', 10.00, '2025-05-01', '2025-05-31', 100.00, 500, 289, 1, 's1', GETDATE()),
('d7', '$5 off Clothing', 'Fixed', 5.00, '2025-07-01', '2025-07-31', 30.00, 1000, 412, 1, 's4', GETDATE()),
('d8', '$20 off Furniture', 'Fixed', 20.00, '2025-06-15', '2025-07-15', 200.00, 200, 78, 1, 's3', GETDATE()),
('d9', '$15 off Kitchen Appliances', 'Fixed', 15.00, '2025-07-01', '2025-07-31', 75.00, 300, 145, 1, 's1', GETDATE()),
('d10', '$50 off Orders over $500', 'Fixed', 50.00, '2025-06-01', '2025-08-31', 500.00, 100, 23, 1, 's2', GETDATE()),

-- More discounts
('d11', 'Back to School - 15% off', 'Percentage', 15.00, '2025-08-01', '2025-08-31', NULL, 1000, 0, 0, 's5', GETDATE()),
('d12', 'Winter Clearance - 50% off', 'Percentage', 50.00, '2025-01-01', '2025-01-31', NULL, 500, 500, 0, 's4', GETDATE()),
('d13', '$25 off Smartphones', 'Fixed', 25.00, '2025-07-01', '2025-07-15', NULL, 200, 187, 1, 's1', GETDATE()),
('d14', '10% off First Purchase', 'Percentage', 10.00, '2025-01-01', '2025-12-31', NULL, 5000, 3245, 1, 's3', GETDATE()),
('d15', '$30 off Laptops', 'Fixed', 30.00, '2025-06-01', '2025-06-30', NULL, 150, 112, 0, 's1', GETDATE()),

-- More discounts to reach 30+
('d16', 'Spring Sale - 20% off', 'Percentage', 20.00, '2025-03-01', '2025-03-31', NULL, 1000, 723, 0, 's2', GETDATE()),
('d17', '$8 off Books', 'Fixed', 8.00, '2025-07-01', '2025-07-31', 20.00, 400, 156, 1, 's5', GETDATE()),
('d18', 'Member Exclusive - 15% off', 'Percentage', 15.00, '2025-01-01', '2025-12-31', NULL, NULL, 892, 1, 's4', GETDATE()),
('d19', '$12 off Toys', 'Fixed', 12.00, '2025-07-01', '2025-07-31', 50.00, 300, 89, 1, 's3', GETDATE()),
('d20', 'Black Friday - 30% off', 'Percentage', 30.00, '2025-11-25', '2025-11-27', NULL, 2000, 0, 0, 's1', GETDATE()),
('d21', '$5 off Small Items', 'Fixed', 5.00, '2025-07-01', '2025-07-15', NULL, 500, 342, 1, 's5', GETDATE()),
('d22', 'End of Season - 25% off', 'Percentage', 25.00, '2025-08-01', '2025-08-31', NULL, 800, 0, 0, 's4', GETDATE()),
('d23', '$100 off Large Orders', 'Fixed', 100.00, '2025-07-01', '2025-09-30', 1000.00, 50, 12, 1, 's2', GETDATE()),
('d24', '15% off Weekend Sale', 'Percentage', 15.00, '2025-07-10', '2025-07-11', NULL, 300, 298, 0, 's1', GETDATE()),
('d25', '$7 off Home Decor', 'Fixed', 7.00, '2025-07-01', '2025-07-31', 35.00, 400, 167, 1, 's3', GETDATE()),
('d26', '20% off Clearance', 'Percentage', 20.00, '2025-07-01', '2025-07-31', NULL, NULL, 432, 1, 's4', GETDATE()),
('d27', '$40 off Electronics Bundle', 'Fixed', 40.00, '2025-06-01', '2025-08-31', 200.00, 200, 87, 1, 's1', GETDATE()),
('d28', '10% off for Students', 'Percentage', 10.00, '2025-01-01', '2025-12-31', NULL, NULL, 654, 1, 's2', GETDATE()),
('d29', '$15 off Shoes', 'Fixed', 15.00, '2025-07-01', '2025-07-31', 75.00, 300, 134, 1, 's4', GETDATE()),
('d30', 'Summer Special - 20% off', 'Percentage', 20.00, '2025-06-15', '2025-08-15', NULL, 1500, 987, 1, 's5', GETDATE());
go

INSERT INTO [dbo].[product_discounts] ([id], [product_id], [discount_id])
VALUES
-- Discounts for electronics
('pd1', 'p1', 'd1'),
('pd2', 'p1', 'd13'),
('pd3', 'p2', 'd1'),
('pd4', 'p3', 'd3'),
('pd5', 'p4', 'd15'),
('pd6', 'p5', 'd10'),
('pd7', 'p6', 'd1'),
('pd8', 'p25', 'd1'),
('pd9', 'p25', 'd6'),
('pd10', 'p26', 'd1'),

-- Discounts for clothing
('pd11', 'p7', 'd2'),
('pd12', 'p7', 'd7'),
('pd13', 'p8', 'd2'),
('pd14', 'p9', 'd2'),
('pd15', 'p10', 'd2'),
('pd16', 'p11', 'd2'),
('pd17', 'p12', 'd2'),
('pd18', 'p27', 'd29'),
('pd19', 'p28', 'd29'),

-- Discounts for home & kitchen
('pd20', 'p13', 'd8'),
('pd21', 'p14', 'd8'),
('pd22', 'p15', 'd8'),
('pd23', 'p16', 'd9'),
('pd24', 'p17', 'd9'),
('pd25', 'p18', 'd9'),
('pd26', 'p29', 'd25'),
('pd27', 'p30', 'd25'),

-- Discounts for books
('pd28', 'p19', 'd17'),
('pd29', 'p20', 'd17'),
('pd30', 'p21', 'd17'),

-- Discounts for toys
('pd31', 'p22', 'd19'),
('pd32', 'p23', 'd19'),
('pd33', 'p24', 'd19'),

-- More product discounts
('pd34', 'p1', 'd27'),
('pd35', 'p3', 'd27'),
('pd36', 'p4', 'd27'),
('pd37', 'p7', 'd18'),
('pd38', 'p10', 'd18'),
('pd39', 'p13', 'd23'),
('pd40', 'p16', 'd23');
go
INSERT INTO [dbo].[shopping_carts] ([id], [user_id], [created_at], [last_updated_at])
VALUES
-- Active carts
('cart1', 'c1', GETDATE(), GETDATE()),
('cart2', 'c2', GETDATE(), GETDATE()),
('cart3', 'c3', GETDATE(), GETDATE()),
('cart4', 'c4', GETDATE(), GETDATE()),
('cart5', 'c5', GETDATE(), GETDATE()),
('cart6', 'c6', GETDATE(), GETDATE()),
('cart7', 'c7', GETDATE(), GETDATE()),
('cart8', 'c8', GETDATE(), GETDATE()),
('cart9', 'c9', GETDATE(), GETDATE()),
('cart10', 'c10', GETDATE(), GETDATE()),

-- More carts
('cart11', 'c1', DATEADD(day, -30, GETDATE()), DATEADD(day, -25, GETDATE())), -- abandoned cart
('cart12', 'c2', DATEADD(day, -15, GETDATE()), DATEADD(day, -10, GETDATE())), -- abandoned cart
('cart13', 'c3', DATEADD(day, -7, GETDATE()), DATEADD(day, -5, GETDATE())), -- abandoned cart
('cart14', 'c4', DATEADD(day, -3, GETDATE()), DATEADD(day, -1, GETDATE())), -- recently abandoned
('cart15', 'c5', DATEADD(day, -1, GETDATE()), GETDATE()), -- active but not updated recently
('cart16', 'c6', DATEADD(day, -60, GETDATE()), DATEADD(day, -55, GETDATE())), -- old abandoned
('cart17', 'c7', DATEADD(day, -45, GETDATE()), DATEADD(day, -40, GETDATE())), -- old abandoned
('cart18', 'c8', DATEADD(day, -20, GETDATE()), DATEADD(day, -18, GETDATE())), -- abandoned
('cart19', 'c9', DATEADD(day, -10, GETDATE()), DATEADD(day, -8, GETDATE())), -- abandoned
('cart20', 'c10', DATEADD(day, -5, GETDATE()), DATEADD(day, -2, GETDATE())), -- abandoned

-- More carts to reach 30+
('cart21', 'c1', DATEADD(day, -90, GETDATE()), DATEADD(day, -85, GETDATE())), -- very old
('cart22', 'c2', DATEADD(day, -75, GETDATE()), DATEADD(day, -70, GETDATE())), -- very old
('cart23', 'c3', DATEADD(day, -50, GETDATE()), DATEADD(day, -45, GETDATE())), -- old
('cart24', 'c4', DATEADD(day, -35, GETDATE()), DATEADD(day, -30, GETDATE())), -- old
('cart25', 'c5', DATEADD(day, -25, GETDATE()), DATEADD(day, -20, GETDATE())), -- old
('cart26', 'c6', DATEADD(day, -18, GETDATE()), DATEADD(day, -15, GETDATE())), -- abandoned
('cart27', 'c7', DATEADD(day, -12, GETDATE()), DATEADD(day, -10, GETDATE())), -- abandoned
('cart28', 'c8', DATEADD(day, -8, GETDATE()), DATEADD(day, -6, GETDATE())), -- abandoned
('cart29', 'c9', DATEADD(day, -4, GETDATE()), DATEADD(day, -2, GETDATE())), -- abandoned
('cart30', 'c10', DATEADD(day, -2, GETDATE()), DATEADD(day, -1, GETDATE())); -- recently abandoned

go
INSERT INTO [dbo].[cart_items] ([id], [cart_id], [product_id], [quantity], [added_at])
VALUES
-- Cart 1 items (current active cart)
('ci1', 'cart1', 'p1', 1, GETDATE()),
('ci2', 'cart1', 'p7', 2, GETDATE()),
('ci3', 'cart1', 'p19', 1, GETDATE()),

-- Cart 2 items (current active cart)
('ci4', 'cart2', 'p3', 1, GETDATE()),
('ci5', 'cart2', 'p12', 1, GETDATE()),

-- Cart 3 items (current active cart)
('ci6', 'cart3', 'p16', 1, GETDATE()),
('ci7', 'cart3', 'p17', 1, GETDATE()),
('ci8', 'cart3', 'p18', 1, GETDATE()),

-- Cart 4 items (current active cart)
('ci9', 'cart4', 'p22', 1, GETDATE()),
('ci10', 'cart4', 'p23', 2, GETDATE()),

-- Cart 5 items (current active cart)
('ci11', 'cart5', 'p27', 1, GETDATE()),
('ci12', 'cart5', 'p28', 1, GETDATE()),

-- Cart 6 items (current active cart)
('ci13', 'cart6', 'p4', 1, GETDATE()),
('ci14', 'cart6', 'p25', 1, GETDATE()),

-- Cart 7 items (current active cart)
('ci15', 'cart7', 'p10', 1, GETDATE()),
('ci16', 'cart7', 'p11', 1, GETDATE()),

-- Cart 8 items (current active cart)
('ci17', 'cart8', 'p13', 1, GETDATE()),
('ci18', 'cart8', 'p15', 1, GETDATE()),

-- Cart 9 items (current active cart)
('ci19', 'cart9', 'p20', 1, GETDATE()),
('ci20', 'cart9', 'p21', 3, GETDATE()),

-- Cart 10 items (current active cart)
('ci21', 'cart10', 'p5', 1, GETDATE()),
('ci22', 'cart10', 'p26', 1, GETDATE()),

-- Abandoned cart items (cart11-20)
('ci23', 'cart11', 'p2', 1, DATEADD(day, -30, GETDATE())),
('ci24', 'cart11', 'p8', 1, DATEADD(day, -30, GETDATE())),
('ci25', 'cart12', 'p9', 2, DATEADD(day, -15, GETDATE())),
('ci26', 'cart13', 'p14', 1, DATEADD(day, -7, GETDATE())),
('ci27', 'cart14', 'p24', 1, DATEADD(day, -3, GETDATE())),
('ci28', 'cart15', 'p29', 2, DATEADD(day, -1, GETDATE())),
('ci29', 'cart16', 'p6', 1, DATEADD(day, -60, GETDATE())),
('ci30', 'cart17', 'p30', 1, DATEADD(day, -45, GETDATE())),
('ci31', 'cart18', 'p1', 1, DATEADD(day, -20, GETDATE())),
('ci32', 'cart19', 'p7', 3, DATEADD(day, -10, GETDATE())),
('ci33', 'cart20', 'p19', 1, DATEADD(day, -5, GETDATE())),

-- More cart items to reach 30+
('ci34', 'cart21', 'p3', 1, DATEADD(day, -90, GETDATE())),
('ci35', 'cart22', 'p12', 1, DATEADD(day, -75, GETDATE())),
('ci36', 'cart23', 'p16', 1, DATEADD(day, -50, GETDATE())),
('ci37', 'cart24', 'p22', 1, DATEADD(day, -35, GETDATE())),
('ci38', 'cart25', 'p27', 1, DATEADD(day, -25, GETDATE())),
('ci39', 'cart26', 'p4', 1, DATEADD(day, -18, GETDATE())),
('ci40', 'cart27', 'p10', 1, DATEADD(day, -12, GETDATE())),
('ci41', 'cart28', 'p13', 1, DATEADD(day, -8, GETDATE())),
('ci42', 'cart29', 'p20', 1, DATEADD(day, -4, GETDATE())),
('ci43', 'cart30', 'p5', 1, DATEADD(day, -2, GETDATE()));
go

INSERT INTO [dbo].[orders] ([id], [buyer_id], [seller_id], [order_date], [total_amount], [shipping_address], [billing_address], [status], [payment_method], [payment_status], [tracking_number], [estimated_delivery_date], [delivered_at])
VALUES
-- Completed orders
('o1', 'c1', 's1', DATEADD(day, -60, GETDATE()), 899.99, '123 Main St, Anytown, USA', '123 Main St, Anytown, USA', 'Delivered', 'Credit Card', 'Paid', 'TRK123456789', DATEADD(day, -55, GETDATE()), DATEADD(day, -55, GETDATE())),
('o2', 'c2', 's1', DATEADD(day, -55, GETDATE()), 299.99, '456 Oak Ave, Somewhere, USA', '456 Oak Ave, Somewhere, USA', 'Delivered', 'PayPal', 'Paid', 'TRK987654321', DATEADD(day, -50, GETDATE()), DATEADD(day, -50, GETDATE())),
('o3', 'c3', 's2', DATEADD(day, -50, GETDATE()), 749.99, '789 Pine Rd, Nowhere, USA', '789 Pine Rd, Nowhere, USA', 'Delivered', 'Credit Card', 'Paid', 'TRK456789123', DATEADD(day, -45, GETDATE()), DATEADD(day, -45, GETDATE())),
('o4', 'c4', 's1', DATEADD(day, -45, GETDATE()), 1199.99, '321 Elm St, Anywhere, USA', '321 Elm St, Anywhere, USA', 'Delivered', 'Credit Card', 'Paid', 'TRK321654987', DATEADD(day, -40, GETDATE()), DATEADD(day, -40, GETDATE())),
('o5', 'c5', 's2', DATEADD(day, -40, GETDATE()), 2499.99, '654 Maple Dr, Everywhere, USA', '654 Maple Dr, Everywhere, USA', 'Delivered', 'PayPal', 'Paid', 'TRK789123456', DATEADD(day, -35, GETDATE()), DATEADD(day, -35, GETDATE())),
('o6', 'c6', 's3', DATEADD(day, -35, GETDATE()), 449.99, '987 Cedar Ln, Somewhere, USA', '987 Cedar Ln, Somewhere, USA', 'Delivered', 'Credit Card', 'Paid', 'TRK159357486', DATEADD(day, -30, GETDATE()), DATEADD(day, -30, GETDATE())),
('o7', 'c7', 's4', DATEADD(day, -30, GETDATE()), 15.99, '147 Birch Blvd, Nowhere, USA', '147 Birch Blvd, Nowhere, USA', 'Delivered', 'Credit Card', 'Paid', 'TRK357159486', DATEADD(day, -25, GETDATE()), DATEADD(day, -25, GETDATE())),
('o8', 'c8', 's4', DATEADD(day, -25, GETDATE()), 39.99, '258 Willow Way, Anywhere, USA', '258 Willow Way, Anywhere, USA', 'Delivered', 'PayPal', 'Paid', 'TRK486357159', DATEADD(day, -20, GETDATE()), DATEADD(day, -20, GETDATE())),
('o9', 'c9', 's5', DATEADD(day, -20, GETDATE()), 49.99, '369 Spruce Ct, Everywhere, USA', '369 Spruce Ct, Everywhere, USA', 'Delivered', 'Credit Card', 'Paid', 'TRK753951846', DATEADD(day, -15, GETDATE()), DATEADD(day, -15, GETDATE())),
('o10', 'c10', 's4', DATEADD(day, -15, GETDATE()), 34.99, '159 Aspen Trl, Somewhere, USA', '159 Aspen Trl, Somewhere, USA', 'Delivered', 'Credit Card', 'Paid', 'TRK846753951', DATEADD(day, -10, GETDATE()), DATEADD(day, -10, GETDATE())),

-- Shipped orders
('o11', 'c1', 's1', DATEADD(day, -10, GETDATE()), 69.99, '123 Main St, Anytown, USA', '123 Main St, Anytown, USA', 'Shipped', 'Credit Card', 'Paid', 'TRK951753846', DATEADD(day, -5, GETDATE()), NULL),
('o12', 'c2', 's2', DATEADD(day, -9, GETDATE()), 119.99, '456 Oak Ave, Somewhere, USA', '456 Oak Ave, Somewhere, USA', 'Shipped', 'PayPal', 'Paid', 'TRK159753486', DATEADD(day, -4, GETDATE()), NULL),
('o13', 'c3', 's3', DATEADD(day, -8, GETDATE()), 24.99, '789 Pine Rd, Nowhere, USA', '789 Pine Rd, Nowhere, USA', 'Shipped', 'Credit Card', 'Paid', 'TRK357951486', DATEADD(day, -3, GETDATE()), NULL),
('o14', 'c4', 's4', DATEADD(day, -7, GETDATE()), 79.99, '321 Elm St, Anywhere, USA', '321 Elm St, Anywhere, USA', 'Shipped', 'Credit Card', 'Paid', 'TRK486159753', DATEADD(day, -2, GETDATE()), NULL),
('o15', 'c5', 's5', DATEADD(day, -6, GETDATE()), 44.99, '654 Maple Dr, Everywhere, USA', '654 Maple Dr, Everywhere, USA', 'Shipped', 'PayPal', 'Paid', 'TRK753159486', DATEADD(day, -1, GETDATE()), NULL),

-- Processing orders
('o16', 'c6', 's1', DATEADD(day, -5, GETDATE()), 799.99, '987 Cedar Ln, Somewhere, USA', '987 Cedar Ln, Somewhere, USA', 'Processing', 'Credit Card', 'Paid', NULL, DATEADD(day, 5, GETDATE()), NULL),
('o17', 'c7', 's2', DATEADD(day, -4, GETDATE()), 129.99, '147 Birch Blvd, Nowhere, USA', '147 Birch Blvd, Nowhere, USA', 'Processing', 'Credit Card', 'Paid', NULL, DATEADD(day, 6, GETDATE()), NULL),
('o18', 'c8', 's3', DATEADD(day, -3, GETDATE()), 34.99, '258 Willow Way, Anywhere, USA', '258 Willow Way, Anywhere, USA', 'Processing', 'PayPal', 'Paid', NULL, DATEADD(day, 7, GETDATE()), NULL),
('o19', 'c9', 's4', DATEADD(day, -2, GETDATE()), 59.99, '369 Spruce Ct, Everywhere, USA', '369 Spruce Ct, Everywhere, USA', 'Processing', 'Credit Card', 'Paid', NULL, DATEADD(day, 8, GETDATE()), NULL),
('o20', 'c10', 's5', DATEADD(day, -1, GETDATE()), 19.99, '159 Aspen Trl, Somewhere, USA', '159 Aspen Trl, Somewhere, USA', 'Processing', 'Credit Card', 'Paid', NULL, DATEADD(day, 9, GETDATE()), NULL),

-- More orders to reach 30+
('o21', 'c1', 's1', DATEADD(day, -60, GETDATE()), 99.99, '123 Main St, Anytown, USA', '123 Main St, Anytown, USA', 'Delivered', 'Credit Card', 'Paid', 'TRK123456780', DATEADD(day, -55, GETDATE()), DATEADD(day, -55, GETDATE())),
('o22', 'c2', 's1', DATEADD(day, -55, GETDATE()), 199.99, '456 Oak Ave, Somewhere, USA', '456 Oak Ave, Somewhere, USA', 'Delivered', 'PayPal', 'Paid', 'TRK987654322', DATEADD(day, -50, GETDATE()), DATEADD(day, -50, GETDATE())),
('o23', 'c3', 's2', DATEADD(day, -50, GETDATE()), 349.99, '789 Pine Rd, Nowhere, USA', '789 Pine Rd, Nowhere, USA', 'Delivered', 'Credit Card', 'Paid', 'TRK456789124', DATEADD(day, -45, GETDATE()), DATEADD(day, -45, GETDATE())),
('o24', 'c4', 's1', DATEADD(day, -45, GETDATE()), 499.99, '321 Elm St, Anywhere, USA', '321 Elm St, Anywhere, USA', 'Delivered', 'Credit Card', 'Paid', 'TRK321654988', DATEADD(day, -40, GETDATE()), DATEADD(day, -40, GETDATE())),
('o25', 'c5', 's2', DATEADD(day, -40, GETDATE()), 599.99, '654 Maple Dr, Everywhere, USA', '654 Maple Dr, Everywhere, USA', 'Delivered', 'PayPal', 'Paid', 'TRK789123457', DATEADD(day, -35, GETDATE()), DATEADD(day, -35, GETDATE())),
('o26', 'c6', 's3', DATEADD(day, -35, GETDATE()), 699.99, '987 Cedar Ln, Somewhere, USA', '987 Cedar Ln, Somewhere, USA', 'Delivered', 'Credit Card', 'Paid', 'TRK159357487', DATEADD(day, -30, GETDATE()), DATEADD(day, -30, GETDATE())),
('o27', 'c7', 's4', DATEADD(day, -30, GETDATE()), 799.99, '147 Birch Blvd, Nowhere, USA', '147 Birch Blvd, Nowhere, USA', 'Delivered', 'Credit Card', 'Paid', 'TRK357159487', DATEADD(day, -25, GETDATE()), DATEADD(day, -25, GETDATE())),
('o28', 'c8', 's4', DATEADD(day, -25, GETDATE()), 899.99, '258 Willow Way, Anywhere, USA', '258 Willow Way, Anywhere, USA', 'Delivered', 'PayPal', 'Paid', 'TRK486357160', DATEADD(day, -20, GETDATE()), DATEADD(day, -20, GETDATE())),
('o29', 'c9', 's5', DATEADD(day, -20, GETDATE()), 999.99, '369 Spruce Ct, Everywhere, USA', '369 Spruce Ct, Everywhere, USA', 'Delivered', 'Credit Card', 'Paid', 'TRK753951847', DATEADD(day, -15, GETDATE()), DATEADD(day, -15, GETDATE())),
('o30', 'c10', 's4', DATEADD(day, -15, GETDATE()), 1099.99, '159 Aspen Trl, Somewhere, USA', '159 Aspen Trl, Somewhere, USA', 'Delivered', 'Credit Card', 'Paid', 'TRK846753952', DATEADD(day, -10, GETDATE()), DATEADD(day, -10, GETDATE()));
go
INSERT INTO [dbo].[order_items] ([id], [order_id], [product_id], [quantity], [unit_price], [discount_applied])
VALUES
-- Order 1 items
('oi1', 'o1', 'p1', 1, 899.99, 100.00),

-- Order 2 items
('oi2', 'o2', 'p2', 1, 299.99, 0.00),

-- Order 3 items
('oi3', 'o3', 'p3', 1, 749.99, 50.00),

-- Order 4 items
('oi4', 'o4', 'p4', 1, 1199.99, 100.00),

-- Order 5 items
('oi5', 'o5', 'p5', 1, 2499.99, 0.00),

-- Order 6 items
('oi6', 'o6', 'p6', 1, 449.99, 50.00),

-- Order 7 items
('oi7', 'o7', 'p7', 1, 15.99, 4.00),

-- Order 8 items
('oi8', 'o8', 'p8', 1, 39.99, 10.00),

-- Order 9 items
('oi9', 'o9', 'p9', 1, 49.99, 10.00),

-- Order 10 items
('oi10', 'o10', 'p10', 1, 34.99, 5.00),

-- Order 11 items
('oi11', 'o11', 'p25', 1, 69.99, 10.00),

-- Order 12 items
('oi12', 'o12', 'p26', 1, 119.99, 10.00),

-- Order 13 items
('oi13', 'o13', 'p19', 1, 19.99, 5.00),

-- Order 14 items
('oi14', 'o14', 'p27', 1, 79.99, 10.00),

-- Order 15 items
('oi15', 'o15', 'p24', 1, 44.99, 5.00),

-- Order 16 items
('oi16', 'o16', 'p13', 1, 799.99, 100.00),

-- Order 17 items
('oi17', 'o17', 'p16', 1, 119.99, 10.00),

-- Order 18 items
('oi18', 'o18', 'p21', 3, 9.99, 3.00),

-- Order 19 items
('oi19', 'o19', 'p28', 1, 59.99, 10.00),

-- Order 20 items
('oi20', 'o20', 'p20', 1, 19.99, 10.00),

-- More order items to reach 30+
('oi21', 'o21', 'p25', 1, 79.99, 0.00),
('oi22', 'o22', 'p26', 1, 129.99, 0.00),
('oi23', 'o23', 'p13', 1, 799.99, 0.00),
('oi24', 'o24', 'p14', 1, 1299.99, 0.00),
('oi25', 'o25', 'p15', 1, 149.99, 0.00),
('oi26', 'o26', 'p16', 1, 129.99, 0.00),
('oi27', 'o27', 'p17', 1, 89.99, 0.00),
('oi28', 'o28', 'p18', 1, 59.99, 0.00),
('oi29', 'o29', 'p19', 1, 24.99, 0.00),
('oi30', 'o30', 'p20', 1, 29.99, 0.00);
go


INSERT INTO [dbo].[order_history] ([id], [order_id], [status], [changed_by], [changed_at], [notes])
VALUES
-- Order 1 history
('oh1', 'o1', 'Processing', 'system', DATEADD(day, -60, GETDATE()), 'Order created'),
('oh2', 'o1', 'Shipped', 's1', DATEADD(day, -59, GETDATE()), 'Package handed to carrier'),
('oh3', 'o1', 'Delivered', 'system', DATEADD(day, -55, GETDATE()), 'Package delivered to customer'),

-- Order 2 history
('oh4', 'o2', 'Processing', 'system', DATEADD(day, -55, GETDATE()), 'Order created'),
('oh5', 'o2', 'Shipped', 's1', DATEADD(day, -54, GETDATE()), 'Package shipped via standard delivery'),
('oh6', 'o2', 'Delivered', 'system', DATEADD(day, -50, GETDATE()), 'Package delivered to customer'),

-- Order 3 history
('oh7', 'o3', 'Processing', 'system', DATEADD(day, -50, GETDATE()), 'Order created'),
('oh8', 'o3', 'Shipped', 's2', DATEADD(day, -49, GETDATE()), 'Package shipped with tracking'),
('oh9', 'o3', 'Delivered', 'system', DATEADD(day, -45, GETDATE()), 'Package delivered to customer'),

-- Order 4 history
('oh10', 'o4', 'Processing', 'system', DATEADD(day, -45, GETDATE()), 'Order created'),
('oh11', 'o4', 'Shipped', 's1', DATEADD(day, -44, GETDATE()), 'Package shipped with express delivery'),
('oh12', 'o4', 'Delivered', 'system', DATEADD(day, -40, GETDATE()), 'Package delivered to customer'),

-- Order 5 history
('oh13', 'o5', 'Processing', 'system', DATEADD(day, -40, GETDATE()), 'Order created'),
('oh14', 'o5', 'Shipped', 's2', DATEADD(day, -39, GETDATE()), 'Package shipped with insurance'),
('oh15', 'o5', 'Delivered', 'system', DATEADD(day, -35, GETDATE()), 'Package delivered to customer'),

-- Order 6 history
('oh16', 'o6', 'Processing', 'system', DATEADD(day, -35, GETDATE()), 'Order created'),
('oh17', 'o6', 'Shipped', 's3', DATEADD(day, -34, GETDATE()), 'Package shipped with standard delivery'),
('oh18', 'o6', 'Delivered', 'system', DATEADD(day, -30, GETDATE()), 'Package delivered to customer'),

-- Order 7 history
('oh19', 'o7', 'Processing', 'system', DATEADD(day, -30, GETDATE()), 'Order created'),
('oh20', 'o7', 'Shipped', 's4', DATEADD(day, -29, GETDATE()), 'Package shipped'),
('oh21', 'o7', 'Delivered', 'system', DATEADD(day, -25, GETDATE()), 'Package delivered to customer'),

-- Order 8 history
('oh22', 'o8', 'Processing', 'system', DATEADD(day, -25, GETDATE()), 'Order created'),
('oh23', 'o8', 'Shipped', 's4', DATEADD(day, -24, GETDATE()), 'Package shipped'),
('oh24', 'o8', 'Delivered', 'system', DATEADD(day, -20, GETDATE()), 'Package delivered to customer'),

-- Order 9 history
('oh25', 'o9', 'Processing', 'system', DATEADD(day, -20, GETDATE()), 'Order created'),
('oh26', 'o9', 'Shipped', 's5', DATEADD(day, -19, GETDATE()), 'Package shipped'),
('oh27', 'o9', 'Delivered', 'system', DATEADD(day, -15, GETDATE()), 'Package delivered to customer'),

-- Order 10 history
('oh28', 'o10', 'Processing', 'system', DATEADD(day, -15, GETDATE()), 'Order created'),
('oh29', 'o10', 'Shipped', 's4', DATEADD(day, -14, GETDATE()), 'Package shipped'),
('oh30', 'o10', 'Delivered', 'system', DATEADD(day, -10, GETDATE()), 'Package delivered to customer');
go

INSERT INTO [dbo].[product_reviews] ([id], [product_id], [user_id], [rating], [title], [comment], [created_at], [is_verified_purchase])
VALUES
-- Reviews for product 1
('pr1', 'p1', 'c1', 5, 'Excellent phone!', 'This phone exceeded all my expectations. The camera quality is amazing and battery life lasts all day.', DATEADD(day, -55, GETDATE()), 1),
('pr2', 'p1', 'c2', 4, 'Great device', 'Very happy with this purchase. The only downside is it''s a bit heavy.', DATEADD(day, -50, GETDATE()), 1),
('pr3', 'p1', 'c3', 3, 'Good but not perfect', 'Decent phone but the software has some bugs that need fixing.', DATEADD(day, -45, GETDATE()), 0),

-- Reviews for product 2
('pr4', 'p2', 'c4', 5, 'Amazing value', 'For this price, you can''t get a better phone. Very satisfied!', DATEADD(day, -40, GETDATE()), 1),
('pr5', 'p2', 'c5', 2, 'Disappointed', 'Battery life is terrible and the camera quality is poor.', DATEADD(day, -35, GETDATE()), 1),

-- Reviews for product 3
('pr6', 'p3', 'c6', 5, 'Gaming beast', 'Perfect for mobile gaming. No lag even at highest settings.', DATEADD(day, -30, GETDATE()), 1),
('pr7', 'p3', 'c7', 4, 'Great performance', 'Handles all my games smoothly. Gets a bit warm during long sessions.', DATEADD(day, -25, GETDATE()), 1),

-- Reviews for product 4
('pr8', 'p4', 'c8', 5, 'Perfect laptop', 'Lightweight yet powerful. Exactly what I needed for work.', DATEADD(day, -20, GETDATE()), 1),
('pr9', 'p4', 'c9', 4, 'Very good', 'Excellent build quality and performance. Battery could last longer.', DATEADD(day, -15, GETDATE()), 1),

-- Reviews for product 5
('pr10', 'p5', 'c10', 5, 'Powerhouse', 'Handles all my games at max settings. Worth every penny.', DATEADD(day, -10, GETDATE()), 1),
('pr11', 'p5', 'c1', 3, 'Good but loud', 'Performance is great but fans are very loud under load.', DATEADD(day, -5, GETDATE()), 1),

-- Reviews for product 6
('pr12', 'p6', 'c2', 4, 'Great budget option', 'Perfect for basic tasks. Don''t expect gaming performance.', GETDATE(), 1),
('pr13', 'p6', 'c3', 2, 'Slow', 'Gets sluggish with multiple tabs open. Expected better.', DATEADD(day, -2, GETDATE()), 1),

-- Reviews for product 7
('pr14', 'p7', 'c4', 5, 'Comfortable fit', 'Very soft material and true to size. Will buy more colors.', DATEADD(day, -55, GETDATE()), 1),
('pr15', 'p7', 'c5', 4, 'Good quality', 'Nice shirt but color faded slightly after first wash.', DATEADD(day, -50, GETDATE()), 1),

-- Reviews for product 8
('pr16', 'p8', 'c6', 5, 'Perfect for work', 'Looks professional and fits well. Highly recommend.', DATEADD(day, -45, GETDATE()), 1),
('pr17', 'p8', 'c7', 3, 'Average', 'Fabric is a bit stiff. Not as comfortable as expected.', DATEADD(day, -40, GETDATE()), 1),

-- Reviews for product 9
('pr18', 'p9', 'c8', 4, 'Great jeans', 'Comfortable and good quality. True to size.', DATEADD(day, -35, GETDATE()), 1),
('pr19', 'p9', 'c9', 2, 'Poor fit', 'Ran large despite ordering my usual size.', DATEADD(day, -30, GETDATE()), 1),

-- Reviews for product 10
('pr20', 'p10', 'c10', 5, 'Beautiful dress', 'Love the pattern and fit. Received many compliments.', DATEADD(day, -25, GETDATE()), 1),
('pr21', 'p10', 'c1', 4, 'Nice but thin', 'Pretty dress but material is thinner than expected.', DATEADD(day, -20, GETDATE()), 1),

-- More reviews to reach 30+
('pr22', 'p11', 'c2', 5, 'Elegant blouse', 'Perfect for both work and evenings out.', DATEADD(day, -15, GETDATE()), 1),
('pr23', 'p12', 'c3', 4, 'Comfy leggings', 'Great for yoga and everyday wear. Pocket is useful.', DATEADD(day, -10, GETDATE()), 1),
('pr24', 'p13', 'c4', 5, 'Stylish sofa', 'Comfortable and looks great in my living room.', DATEADD(day, -5, GETDATE()), 1),
('pr25', 'p14', 'c5', 3, 'Good table set', 'Nice but chairs could be more comfortable.', GETDATE(), 1),
('pr26', 'p15', 'c6', 4, 'Solid bookshelf', 'Easy to assemble and holds all my books.', DATEADD(day, -2, GETDATE()), 1),
('pr27', 'p16', 'c7', 5, 'Powerful blender', 'Makes smooth smoothies and even crushes ice.', DATEADD(day, -55, GETDATE()), 1),
('pr28', 'p17', 'c8', 4, 'Great air fryer', 'Cooked perfect fries and chicken wings.', DATEADD(day, -50, GETDATE()), 1),
('pr29', 'p18', 'c9', 5, 'Reliable coffee maker', 'Brews great coffee every morning.', DATEADD(day, -45, GETDATE()), 1),
('pr30', 'p19', 'c10', 5, 'Couldn''t put it down', 'Amazing story with unexpected twists.', DATEADD(day, -40, GETDATE()), 1);
go
INSERT INTO [dbo].[support_tickets] ([id], [user_id], [subject], [description], [status], [priority], [created_at], [resolved_at], [resolved_by], [is_deleted])
VALUES
-- Open tickets
('t1', 'c1', 'Order not received', 'I placed order o1 but haven''t received it yet. Tracking shows delivered but it''s not here.', 'Open', 'High', DATEADD(day, -54, GETDATE()), NULL, NULL, 0),
('t2', 'c2', 'Defective product', 'The smartphone I received (order o2) has a cracked screen out of the box.', 'Open', 'High', DATEADD(day, -49, GETDATE()), NULL, NULL, 0),
('t3', 'c3', 'Wrong item shipped', 'I ordered product p3 but received a completely different item.', 'Open', 'Medium', DATEADD(day, -44, GETDATE()), NULL, NULL, 0),

-- In Progress tickets
('t4', 'c4', 'Refund request', 'I returned my item 2 weeks ago but haven''t received my refund.', 'In Progress', 'Medium', DATEADD(day, -39, GETDATE()), NULL, NULL, 0),
('t5', 'c5', 'Account issue', 'I can''t log in to my account. Getting "invalid credentials" error.', 'In Progress', 'High', DATEADD(day, -34, GETDATE()), NULL, NULL, 0),
('t6', 'c6', 'Billing question', 'I was charged twice for order o6. Need one charge reversed.', 'In Progress', 'High', DATEADD(day, -29, GETDATE()), NULL, NULL, 0),

-- Resolved tickets
('t7', 'c7', 'Late delivery', 'My order o7 was supposed to arrive 3 days ago.', 'Resolved', 'Medium', DATEADD(day, -24, GETDATE()), DATEADD(day, -20, GETDATE()), 'sp1', 0),
('t8', 'c8', 'Product question', 'Does product p8 come in other colors?', 'Resolved', 'Low', DATEADD(day, -19, GETDATE()), DATEADD(day, -18, GETDATE()), 'sp2', 0),
('t9', 'c9', 'Return instructions', 'How do I return an item I''m not satisfied with?', 'Resolved', 'Low', DATEADD(day, -14, GETDATE()), DATEADD(day, -13, GETDATE()), 'sp1', 0),
('t10', 'c10', 'Coupon not working', 'Tried to apply coupon code SUMMER20 but it says invalid.', 'Resolved', 'Medium', DATEADD(day, -9, GETDATE()), DATEADD(day, -7, GETDATE()), 'sp2', 0),

-- More tickets to reach 30+
('t11', 'c1', 'Order cancellation', 'Need to cancel order o11 as I found a better deal elsewhere.', 'Open', 'Medium', DATEADD(day, -8, GETDATE()), NULL, NULL, 0),
('t12', 'c2', 'Shipping upgrade', 'Can I upgrade my shipping for order o12 to expedited?', 'Open', 'Low', DATEADD(day, -7, GETDATE()), NULL, NULL, 0),
('t13', 'c3', 'Product suggestion', 'Please consider stocking more colors for product p13.', 'Open', 'Low', DATEADD(day, -6, GETDATE()), NULL, NULL, 0),
('t14', 'c4', 'Missing item', 'My order o14 was missing one of the items.', 'In Progress', 'High', DATEADD(day, -5, GETDATE()), NULL, NULL, 0),
('t15', 'c5', 'Price match', 'Found product p15 cheaper elsewhere. Will you price match?', 'In Progress', 'Medium', DATEADD(day, -4, GETDATE()), NULL, NULL, 0),
('t16', 'c6', 'Warranty claim', 'My product p16 stopped working after 2 months.', 'In Progress', 'High', DATEADD(day, -3, GETDATE()), NULL, NULL, 0),
('t17', 'c7', 'Return status', 'When will my return be processed? Shipped it back 10 days ago.', 'Resolved', 'Medium', DATEADD(day, -24, GETDATE()), DATEADD(day, -20, GETDATE()), 'sp1', 0),
('t18', 'c8', 'Product availability', 'When will product p18 be back in stock?', 'Resolved', 'Low', DATEADD(day, -19, GETDATE()), DATEADD(day, -18, GETDATE()), 'sp2', 0),
('t19', 'c9', 'Account merge', 'I have two accounts and want to merge them.', 'Resolved', 'Medium', DATEADD(day, -14, GETDATE()), DATEADD(day, -13, GETDATE()), 'sp1', 0),
('t20', 'c10', 'Payment method', 'Can I change the payment method for order o20?', 'Resolved', 'Low', DATEADD(day, -9, GETDATE()), DATEADD(day, -7, GETDATE()), 'sp2', 0),
('t21', 'c1', 'Order modification', 'Need to add an item to order o21 before it ships.', 'Open', 'Medium', DATEADD(day, -8, GETDATE()), NULL, NULL, 0),
('t22', 'c2', 'Gift wrapping', 'Can you add gift wrapping to order o22?', 'Open', 'Low', DATEADD(day, -7, GETDATE()), NULL, NULL, 0),
('t23', 'c3', 'Product details', 'What are the dimensions of product p23?', 'Open', 'Low', DATEADD(day, -6, GETDATE()), NULL, NULL, 0),
('t24', 'c4', 'Damaged item', 'Received item p24 with damaged packaging.', 'In Progress', 'High', DATEADD(day, -5, GETDATE()), NULL, NULL, 0),
('t25', 'c5', 'Subscription cancel', 'How do I cancel my premium membership?', 'In Progress', 'Medium', DATEADD(day, -4, GETDATE()), NULL, NULL, 0),
('t26', 'c6', 'Order tracking', 'No tracking update for order o26 in 5 days.', 'In Progress', 'High', DATEADD(day, -3, GETDATE()), NULL, NULL, 0),
('t27', 'c7', 'Refund status', 'When will my refund be processed?', 'Resolved', 'Medium', DATEADD(day, -24, GETDATE()), DATEADD(day, -20, GETDATE()), 'sp1', 0),
('t28', 'c8', 'Product comparison', 'What''s the difference between p28 and p29?', 'Resolved', 'Low', DATEADD(day, -19, GETDATE()), DATEADD(day, -18, GETDATE()), 'sp2', 0),
('t29', 'c9', 'Bulk order discount', 'Do you offer discounts for bulk orders?', 'Resolved', 'Medium', DATEADD(day, -14, GETDATE()), DATEADD(day, -13, GETDATE()), 'sp1', 0),
('t30', 'c10', 'International shipping', 'Do you ship to Canada?', 'Resolved', 'Low', DATEADD(day, -9, GETDATE()), DATEADD(day, -7, GETDATE()), 'sp2', 0);
go
INSERT INTO [dbo].[ticket_messages] ([id], [ticket_id], [sender_id], [message], [sent_at], [is_read])
VALUES
-- Ticket 1 messages
('tm1', 't1', 'c1', 'Hello, I placed order o1 but haven''t received it yet. Tracking shows delivered but it''s not here. Can you help?', DATEADD(day, -54, GETDATE()), 1),
('tm2', 't1', 'sp1', 'I''m sorry to hear that. Let me check with the carrier and get back to you within 24 hours.', DATEADD(day, -53, GETDATE()), 1),
('tm3', 't1', 'c1', 'Thank you, I appreciate your help.', DATEADD(day, -53, GETDATE()), 1),

-- Ticket 2 messages
('tm4', 't2', 'c2', 'The smartphone I received has a cracked screen right out of the box. Very disappointed!', DATEADD(day, -49, GETDATE()), 1),
('tm5', 't2', 'sp2', 'I apologize for the inconvenience. We''ll send you a replacement immediately. Please provide photos of the damage.', DATEADD(day, -48, GETDATE()), 1),

-- Ticket 3 messages
('tm6', 't3', 'c3', 'I ordered product p3 but received something completely different. What should I do?', DATEADD(day, -44, GETDATE()), 1),
('tm7', 't3', 'sp1', 'That shouldn''t have happened. We''ll arrange for return shipping and send the correct item right away.', DATEADD(day, -43, GETDATE()), 1),

-- Ticket 4 messages
('tm8', 't4', 'c4', 'I returned my item 2 weeks ago but haven''t received my refund. The tracking shows it was delivered.', DATEADD(day, -39, GETDATE()), 1),
('tm9', 't4', 'sp2', 'Let me check with our returns department and update you shortly.', DATEADD(day, -38, GETDATE()), 1),

-- Ticket 5 messages
('tm10', 't5', 'c5', 'I can''t log in to my account. Getting "invalid credentials" error but I''m sure my password is correct.', DATEADD(day, -34, GETDATE()), 1),
('tm11', 't5', 'sp1', 'Let me reset your password and send you a temporary one. You can change it after logging in.', DATEADD(day, -33, GETDATE()), 1),

-- Ticket 6 messages
('tm12', 't6', 'c6', 'I was charged twice for order o6. Need one charge reversed.', DATEADD(day, -29, GETDATE()), 1),
('tm13', 't6', 'sp2', 'I see the duplicate charge. We''ll process the refund within 3-5 business days.', DATEADD(day, -28, GETDATE()), 1),

-- Ticket 7 messages
('tm14', 't7', 'c7', 'My order o7 was supposed to arrive 3 days ago but tracking hasn''t updated.', DATEADD(day, -24, GETDATE()), 1),
('tm15', 't7', 'sp1', 'The carrier reports delays in your area due to weather. Should arrive tomorrow.', DATEADD(day, -23, GETDATE()), 1),
('tm16', 't7', 'c7', 'It arrived today, thanks for your help!', DATEADD(day, -22, GETDATE()), 1),

-- Ticket 8 messages
('tm17', 't8', 'c8', 'Does product p8 come in other colors besides what''s shown?', DATEADD(day, -19, GETDATE()), 1),
('tm18', 't8', 'sp2', 'Currently we only stock the colors shown, but more may be available next season.', DATEADD(day, -18, GETDATE()), 1),

-- Ticket 9 messages
('tm19', 't9', 'c9', 'How do I return an item I''m not satisfied with?', DATEADD(day, -14, GETDATE()), 1),
('tm20', 't9', 'sp1', 'You can initiate a return from your order history. Here''s a direct link: [link]', DATEADD(day, -13, GETDATE()), 1),

-- Ticket 10 messages
('tm21', 't10', 'c10', 'Tried to apply coupon code SUMMER20 but it says invalid.', DATEADD(day, -9, GETDATE()), 1),
('tm22', 't10', 'sp2', 'That coupon expired yesterday. Here''s a new one for 15% off: FALL15', DATEADD(day, -8, GETDATE()), 1),

-- More ticket messages to reach 30+
('tm23', 't11', 'c1', 'Need to cancel order o11 as I found a better deal elsewhere.', DATEADD(day, -8, GETDATE()), 1),
('tm24', 't12', 'c2', 'Can I upgrade my shipping for order o12 to expedited?', DATEADD(day, -7, GETDATE()), 1),
('tm25', 't13', 'c3', 'Please consider stocking more colors for product p13.', DATEADD(day, -6, GETDATE()), 1),
('tm26', 't14', 'c4', 'My order o14 was missing one of the items.', DATEADD(day, -5, GETDATE()), 1),
('tm27', 't15', 'c5', 'Found product p15 cheaper elsewhere. Will you price match?', DATEADD(day, -4, GETDATE()), 1),
('tm28', 't16', 'c6', 'My product p16 stopped working after 2 months.', DATEADD(day, -3, GETDATE()), 1),
('tm29', 't17', 'c7', 'When will my return be processed? Shipped it back 10 days ago.', DATEADD(day, -24, GETDATE()), 1),
('tm30', 't18', 'c8', 'When will product p18 be back in stock?', DATEADD(day, -19, GETDATE()), 1);
go
INSERT INTO [dbo].[ticket_history] ([id], [ticket_id], [changed_by], [changed_at], [field_changed], [old_value], [new_value])
VALUES
-- Ticket 1 history
('th1', 't1', 'system', DATEADD(day, -54, GETDATE()), 'Status', NULL, 'Open'),
('th2', 't1', 'sp1', DATEADD(day, -53, GETDATE()), 'Priority', 'Medium', 'High'),

-- Ticket 2 history
('th3', 't2', 'system', DATEADD(day, -49, GETDATE()), 'Status', NULL, 'Open'),
('th4', 't2', 'sp2', DATEADD(day, -48, GETDATE()), 'Priority', 'Medium', 'High'),

-- Ticket 3 history
('th5', 't3', 'system', DATEADD(day, -44, GETDATE()), 'Status', NULL, 'Open'),

-- Ticket 4 history
('th6', 't4', 'system', DATEADD(day, -39, GETDATE()), 'Status', NULL, 'Open'),
('th7', 't4', 'sp2', DATEADD(day, -38, GETDATE()), 'Status', 'Open', 'In Progress'),

-- Ticket 5 history
('th8', 't5', 'system', DATEADD(day, -34, GETDATE()), 'Status', NULL, 'Open'),
('th9', 't5', 'sp1', DATEADD(day, -33, GETDATE()), 'Status', 'Open', 'In Progress'),
('th10', 't5', 'sp1', DATEADD(day, -33, GETDATE()), 'Priority', 'Medium', 'High'),

-- Ticket 6 history
('th11', 't6', 'system', DATEADD(day, -29, GETDATE()), 'Status', NULL, 'Open'),
('th12', 't6', 'sp2', DATEADD(day, -28, GETDATE()), 'Status', 'Open', 'In Progress'),
('th13', 't6', 'sp2', DATEADD(day, -28, GETDATE()), 'Priority', 'Medium', 'High'),

-- Ticket 7 history
('th14', 't7', 'system', DATEADD(day, -24, GETDATE()), 'Status', NULL, 'Open'),
('th15', 't7', 'sp1', DATEADD(day, -23, GETDATE()), 'Status', 'Open', 'In Progress'),
('th16', 't7', 'sp1', DATEADD(day, -20, GETDATE()), 'Status', 'In Progress', 'Resolved'),

-- Ticket 8 history
('th17', 't8', 'system', DATEADD(day, -19, GETDATE()), 'Status', NULL, 'Open'),
('th18', 't8', 'sp2', DATEADD(day, -18, GETDATE()), 'Status', 'Open', 'Resolved'),

-- Ticket 9 history
('th19', 't9', 'system', DATEADD(day, -14, GETDATE()), 'Status', NULL, 'Open'),
('th20', 't9', 'sp1', DATEADD(day, -13, GETDATE()), 'Status', 'Open', 'Resolved'),

-- Ticket 10 history
('th21', 't10', 'system', DATEADD(day, -9, GETDATE()), 'Status', NULL, 'Open'),
('th22', 't10', 'sp2', DATEADD(day, -8, GETDATE()), 'Status', 'Open', 'In Progress'),
('th23', 't10', 'sp2', DATEADD(day, -7, GETDATE()), 'Status', 'In Progress', 'Resolved'),

-- More ticket history to reach 30+
('th24', 't11', 'system', DATEADD(day, -8, GETDATE()), 'Status', NULL, 'Open'),
('th25', 't12', 'system', DATEADD(day, -7, GETDATE()), 'Status', NULL, 'Open'),
('th26', 't13', 'system', DATEADD(day, -6, GETDATE()), 'Status', NULL, 'Open'),
('th27', 't14', 'system', DATEADD(day, -5, GETDATE()), 'Status', NULL, 'Open'),
('th28', 't14', 'sp1', DATEADD(day, -4, GETDATE()), 'Status', 'Open', 'In Progress'),
('th29', 't15', 'system', DATEADD(day, -4, GETDATE()), 'Status', NULL, 'Open'),
('th30', 't15', 'sp2', DATEADD(day, -3, GETDATE()), 'Status', 'Open', 'In Progress');
go

INSERT INTO [dbo].[chat_sessions] ([Id], [CustomerId], [SellerId], [CreatedAt], [LastMessageAt], [Status], [ClosedAt], [IsDeleted])
VALUES
-- Active chats
('cs1', 'c1', 's1', DATEADD(hour, -2, GETDATE()), DATEADD(minute, -15, GETDATE()), 'Active', NULL, 0),
('cs2', 'c2', 's2', DATEADD(hour, -1, GETDATE()), DATEADD(minute, -10, GETDATE()), 'Active', NULL, 0),
('cs3', 'c3', 's3', DATEADD(minute, -45, GETDATE()), DATEADD(minute, -5, GETDATE()), 'Active', NULL, 0),

-- Recently closed chats
('cs4', 'c4', 's4', DATEADD(day, -1, GETDATE()), DATEADD(day, -1, GETDATE()), 'Closed', DATEADD(day, -1, GETDATE()), 0),
('cs5', 'c5', 's5', DATEADD(day, -2, GETDATE()), DATEADD(day, -2, GETDATE()), 'Closed', DATEADD(day, -2, GETDATE()), 0),
('cs6', 'c6', 's1', DATEADD(day, -3, GETDATE()), DATEADD(day, -3, GETDATE()), 'Closed', DATEADD(day, -3, GETDATE()), 0),

-- Older chats
('cs7', 'c7', 's2', DATEADD(day, -5, GETDATE()), DATEADD(day, -5, GETDATE()), 'Closed', DATEADD(day, -5, GETDATE()), 0),
('cs8', 'c8', 's3', DATEADD(day, -7, GETDATE()), DATEADD(day, -7, GETDATE()), 'Closed', DATEADD(day, -7, GETDATE()), 0),
('cs9', 'c9', 's4', DATEADD(day, -10, GETDATE()), DATEADD(day, -10, GETDATE()), 'Closed', DATEADD(day, -10, GETDATE()), 0),
('cs10', 'c10', 's5', DATEADD(day, -14, GETDATE()), DATEADD(day, -14, GETDATE()), 'Closed', DATEADD(day, -14, GETDATE()), 0),

-- More chat sessions to reach 30+
('cs11', 'c1', 's2', DATEADD(day, -15, GETDATE()), DATEADD(day, -15, GETDATE()), 'Closed', DATEADD(day, -15, GETDATE()), 0),
('cs12', 'c2', 's3', DATEADD(day, -16, GETDATE()), DATEADD(day, -16, GETDATE()), 'Closed', DATEADD(day, -16, GETDATE()), 0),
('cs13', 'c3', 's4', DATEADD(day, -17, GETDATE()), DATEADD(day, -17, GETDATE()), 'Closed', DATEADD(day, -17, GETDATE()), 0),
('cs14', 'c4', 's5', DATEADD(day, -18, GETDATE()), DATEADD(day, -18, GETDATE()), 'Closed', DATEADD(day, -18, GETDATE()), 0),
('cs15', 'c5', 's1', DATEADD(day, -19, GETDATE()), DATEADD(day, -19, GETDATE()), 'Closed', DATEADD(day, -19, GETDATE()), 0),
('cs16', 'c6', 's2', DATEADD(day, -20, GETDATE()), DATEADD(day, -20, GETDATE()), 'Closed', DATEADD(day, -20, GETDATE()), 0),
('cs17', 'c7', 's3', DATEADD(day, -21, GETDATE()), DATEADD(day, -21, GETDATE()), 'Closed', DATEADD(day, -21, GETDATE()), 0),
('cs18', 'c8', 's4', DATEADD(day, -22, GETDATE()), DATEADD(day, -22, GETDATE()), 'Closed', DATEADD(day, -22, GETDATE()), 0),
('cs19', 'c9', 's5', DATEADD(day, -23, GETDATE()), DATEADD(day, -23, GETDATE()), 'Closed', DATEADD(day, -23, GETDATE()), 0),
('cs20', 'c10', 's1', DATEADD(day, -24, GETDATE()), DATEADD(day, -24, GETDATE()), 'Closed', DATEADD(day, -24, GETDATE()), 0),
('cs21', 'c1', 's3', DATEADD(day, -25, GETDATE()), DATEADD(day, -25, GETDATE()), 'Closed', DATEADD(day, -25, GETDATE()), 0),
('cs22', 'c2', 's4', DATEADD(day, -26, GETDATE()), DATEADD(day, -26, GETDATE()), 'Closed', DATEADD(day, -26, GETDATE()), 0),
('cs23', 'c3', 's5', DATEADD(day, -27, GETDATE()), DATEADD(day, -27, GETDATE()), 'Closed', DATEADD(day, -27, GETDATE()), 0),
('cs24', 'c4', 's1', DATEADD(day, -28, GETDATE()), DATEADD(day, -28, GETDATE()), 'Closed', DATEADD(day, -28, GETDATE()), 0),
('cs25', 'c5', 's2', DATEADD(day, -29, GETDATE()), DATEADD(day, -29, GETDATE()), 'Closed', DATEADD(day, -29, GETDATE()), 0),
('cs26', 'c6', 's3', DATEADD(day, -30, GETDATE()), DATEADD(day, -30, GETDATE()), 'Closed', DATEADD(day, -30, GETDATE()), 0),
('cs27', 'c7', 's4', DATEADD(day, -31, GETDATE()), DATEADD(day, -31, GETDATE()), 'Closed', DATEADD(day, -31, GETDATE()), 0),
('cs28', 'c8', 's5', DATEADD(day, -32, GETDATE()), DATEADD(day, -32, GETDATE()), 'Closed', DATEADD(day, -32, GETDATE()), 0),
('cs29', 'c9', 's1', DATEADD(day, -33, GETDATE()), DATEADD(day, -33, GETDATE()), 'Closed', DATEADD(day, -33, GETDATE()), 0),
('cs30', 'c10', 's2', DATEADD(day, -34, GETDATE()), DATEADD(day, -34, GETDATE()), 'Closed', DATEADD(day, -34, GETDATE()), 0);
go

INSERT INTO [dbo].[chat_messages] ([id], [session_id], [sender_id], [message], [sent_at], [is_read])
VALUES
-- Chat session 1 messages
('cm1', 'cs1', 'c1', 'Hi, I have a question about product p1', DATEADD(hour, -2, GETDATE()), 1),
('cm2', 'cs1', 's1', 'Hello! What would you like to know?', DATEADD(hour, -2, GETDATE()), 1),
('cm3', 'cs1', 'c1', 'Does it come with a warranty?', DATEADD(hour, -1, GETDATE()), 1),
('cm4', 'cs1', 's1', 'Yes, it has a 1-year manufacturer warranty', DATEADD(hour, -1, GETDATE()), 1),
('cm5', 'cs1', 'c1', 'Great, thanks!', DATEADD(minute, -15, GETDATE()), 1),

-- Chat session 2 messages
('cm6', 'cs2', 'c2', 'Is product p2 in stock?', DATEADD(hour, -1, GETDATE()), 1),
('cm7', 'cs2', 's2', 'Yes, we have 200 units available', DATEADD(hour, -1, GETDATE()), 1),
('cm8', 'cs2', 'c2', 'Perfect, ordering now', DATEADD(minute, -10, GETDATE()), 1),

-- Chat session 3 messages
('cm9', 'cs3', 'c3', 'Do you offer bulk discounts for product p3?', DATEADD(minute, -45, GETDATE()), 1),
('cm10', 'cs3', 's3', 'We can offer 10% off for orders of 5+ units', DATEADD(minute, -30, GETDATE()), 1),
('cm11', 'cs3', 'c3', 'That works for me', DATEADD(minute, -5, GETDATE()), 1),

-- Chat session 4 messages
('cm12', 'cs4', 'c4', 'When will my order ship?', DATEADD(day, -1, GETDATE()), 1),
('cm13', 'cs4', 's4', 'It will ship tomorrow morning', DATEADD(day, -1, GETDATE()), 1),
('cm14', 'cs4', 'c4', 'Thanks for the update', DATEADD(day, -1, GETDATE()), 1),

-- Chat session 5 messages
('cm15', 'cs5', 'c5', 'Can I change the shipping address for my order?', DATEADD(day, -2, GETDATE()), 1),
('cm16', 'cs5', 's5', 'Yes, please provide the new address', DATEADD(day, -2, GETDATE()), 1),
('cm17', 'cs5', 'c5', 'Sent it via private message', DATEADD(day, -2, GETDATE()), 1),

-- More chat messages to reach 30+
('cm18', 'cs6', 'c6', 'Do you have product p6 in other colors?', DATEADD(day, -3, GETDATE()), 1),
('cm19', 'cs6', 's1', 'Only in black and white currently', DATEADD(day, -3, GETDATE()), 1),
('cm20', 'cs7', 'c7', 'What''s the return policy?', DATEADD(day, -5, GETDATE()), 1),
('cm21', 'cs7', 's2', '30-day return policy for unused items', DATEADD(day, -5, GETDATE()), 1),
('cm22', 'cs8', 'c8', 'Is product p8 machine washable?', DATEADD(day, -7, GETDATE()), 1),
('cm23', 'cs8', 's3', 'Yes, cold wash recommended', DATEADD(day, -7, GETDATE()), 1),
('cm24', 'cs9', 'c9', 'Do you ship internationally?', DATEADD(day, -10, GETDATE()), 1),
('cm25', 'cs9', 's4', 'Yes, to select countries', DATEADD(day, -10, GETDATE()), 1),
('cm26', 'cs10', 'c10', 'Can I get a discount code?', DATEADD(day, -14, GETDATE()), 1),
('cm27', 'cs10', 's5', 'Here''s 10% off: WELCOME10', DATEADD(day, -14, GETDATE()), 1),
('cm28', 'cs11', 'c1', 'Is p11 true to size?', DATEADD(day, -15, GETDATE()), 1),
('cm29', 'cs11', 's2', 'Runs slightly small, recommend sizing up', DATEADD(day, -15, GETDATE()), 1),
('cm30', 'cs12', 'c2', 'What material is p12 made of?', DATEADD(day, -16, GETDATE()), 1);
go





INSERT INTO [dbo].[audit_logs] ([id], [user_id], [action], [entity_type], [entity_id], [old_values], [new_values], [ip_address], [user_agent], [timestamp])
VALUES
-- User-related logs
('al1', 'a1', 'Create', 'User', 'c1', NULL, '{"UserName":"customer1","Email":"customer1@example.com"}', '192.168.1.1', 'Mozilla/5.0', DATEADD(day, -90, GETDATE())),
('al2', 'a1', 'Update', 'User', 'c1', '{"is_active":false}', '{"is_active":true}', '192.168.1.1', 'Mozilla/5.0', DATEADD(day, -85, GETDATE())),
('al3', 'a2', 'Create', 'User', 's1', NULL, '{"UserName":"seller1","Email":"seller1@example.com"}', '192.168.1.2', 'Mozilla/5.0', DATEADD(day, -80, GETDATE())),

-- Product-related logs
('al4', 's1', 'Create', 'Product', 'p1', NULL, '{"name":"Premium Smartphone X","price":999.99}', '192.168.2.1', 'Mozilla/5.0', DATEADD(day, -75, GETDATE())),
('al5', 'a1', 'Approve', 'Product', 'p1', '{"is_approved":false}', '{"is_approved":true}', '192.168.1.1', 'Mozilla/5.0', DATEADD(day, -74, GETDATE())),
('al6', 's1', 'Update', 'Product', 'p1', '{"price":999.99}', '{"price":899.99}', '192.168.2.1', 'Mozilla/5.0', DATEADD(day, -70, GETDATE())),

-- Order-related logs
('al7', 'a1', 'Create', 'Order', 'o1', NULL, '{"total_amount":899.99,"status":"Processing"}', '192.168.3.1', 'System', DATEADD(day, -60, GETDATE())),
('al8', 's1', 'Update', 'Order', 'o1', '{"status":"Processing"}', '{"status":"Shipped"}', '192.168.2.1', 'Mozilla/5.0', DATEADD(day, -59, GETDATE())),
('al9', 'a1', 'Update', 'Order', 'o1', '{"status":"Shipped"}', '{"status":"Delivered"}', '192.168.3.1', 'System', DATEADD(day, -55, GETDATE())),

-- Category-related logs
('al10', 'a1', 'Create', 'Category', 'cat1', NULL, '{"name":"Electronics","is_active":true}', '192.168.1.1', 'Mozilla/5.0', DATEADD(day, -50, GETDATE())),
('al11', 'a1', 'Create', 'Category', 'cat6', NULL, '{"name":"Smartphones","parent_category_id":"cat1"}', '192.168.1.1', 'Mozilla/5.0', DATEADD(day, -49, GETDATE())),
('al12', 'a1', 'Update', 'Category', 'cat6', '{"description":null}', '{"description":"Latest smartphones and accessories"}', '192.168.1.1', 'Mozilla/5.0', DATEADD(day, -48, GETDATE())),

-- Discount-related logs
('al13', 's1', 'Create', 'Discount', 'd1', NULL, '{"description":"Summer Sale - 20% off","value":20}', '192.168.2.1', 'Mozilla/5.0', DATEADD(day, -40, GETDATE())),
('al14', 's1', 'Update', 'Discount', 'd1', '{"end_date":"2025-06-15"}', '{"end_date":"2025-06-30"}', '192.168.2.1', 'Mozilla/5.0', DATEADD(day, -35, GETDATE())),
('al15', 's1', 'Deactivate', 'Discount', 'd1', '{"is_active":true}', '{"is_active":false}', '192.168.2.1', 'Mozilla/5.0', DATEADD(day, -30, GETDATE())),

-- More audit logs to reach 30+
('al16', 'a1', 'Delete', 'User', 'temp1', '{"UserName":"temp1","Email":"temp1@example.com"}', NULL, '192.168.1.1', 'Mozilla/5.0', DATEADD(day, -25, GETDATE())),
('al17', 's2', 'Create', 'Product', 'p3', NULL, '{"name":"Gaming Phone Z","price":799.99}', '192.168.2.2', 'Mozilla/5.0', DATEADD(day, -20, GETDATE())),
('al18', 'a1', 'Approve', 'Product', 'p3', '{"is_approved":false}', '{"is_approved":true}', '192.168.1.1', 'Mozilla/5.0', DATEADD(day, -19, GETDATE())),
('al19', 'a1', 'Create', 'Order', 'o2', NULL, '{"total_amount":299.99,"status":"Processing"}', '192.168.3.1', 'System', DATEADD(day, -18, GETDATE())),
('al20', 's1', 'Update', 'Order', 'o2', '{"status":"Processing"}', '{"status":"Shipped"}', '192.168.2.1', 'Mozilla/5.0', DATEADD(day, -17, GETDATE())),
('al21', 'a1', 'Update', 'Order', 'o2', '{"status":"Shipped"}', '{"status":"Delivered"}', '192.168.3.1', 'System', DATEADD(day, -15, GETDATE())),
('al22', 'a1', 'Create', 'Category', 'cat2', NULL, '{"name":"Clothing","is_active":true}', '192.168.1.1', 'Mozilla/5.0', DATEADD(day, -14, GETDATE())),
('al23', 'a1', 'Create', 'Category', 'cat11', NULL, '{"name":"Men''s Clothing","parent_category_id":"cat2"}', '192.168.1.1', 'Mozilla/5.0', DATEADD(day, -13, GETDATE())),
('al24', 's4', 'Create', 'Product', 'p7', NULL, '{"name":"Men''s Casual T-Shirt","price":19.99}', '192.168.2.4', 'Mozilla/5.0', DATEADD(day, -12, GETDATE())),
('al25', 'a1', 'Approve', 'Product', 'p7', '{"is_approved":false}', '{"is_approved":true}', '192.168.1.1', 'Mozilla/5.0', DATEADD(day, -11, GETDATE())),
('al26', 's4', 'Update', 'Product', 'p7', '{"price":19.99}', '{"price":15.99}', '192.168.2.4', 'Mozilla/5.0', DATEADD(day, -10, GETDATE())),
('al27', 'a1', 'Create', 'Order', 'o3', NULL, '{"total_amount":749.99,"status":"Processing"}', '192.168.3.1', 'System', DATEADD(day, -9, GETDATE())),
('al28', 's2', 'Update', 'Order', 'o3', '{"status":"Processing"}', '{"status":"Shipped"}', '192.168.2.2', 'Mozilla/5.0', DATEADD(day, -8, GETDATE())),
('al29', 'a1', 'Update', 'Order', 'o3', '{"status":"Shipped"}', '{"status":"Delivered"}', '192.168.3.1', 'System', DATEADD(day, -7, GETDATE())),
('al30', 'a1', 'Update', 'User', 'c1', '{"profile_picture_url":null}', '{"profile_picture_url":"https://example.com/profiles/customer1.jpg"}', '192.168.1.1', 'Mozilla/5.0', DATEADD(day, -5, GETDATE()));
go





