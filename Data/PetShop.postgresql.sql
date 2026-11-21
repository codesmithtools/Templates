-- PetShop for PostgreSQL: a subset of PetShop.sql (SQL Server's), for the samples. Load it into a database named
-- petshop: psql -d petshop -f PetShop.postgresql.sql
CREATE TABLE category (
  categoryid VARCHAR(10) NOT NULL,
  name VARCHAR(80) NULL,
  descn VARCHAR(255) NULL,
  CONSTRAINT pk_category PRIMARY KEY (categoryid)
)
;
CREATE TABLE supplier (
  suppid INTEGER NOT NULL,
  name VARCHAR(80) NULL,
  status VARCHAR(2) NOT NULL,
  city VARCHAR(80) NULL,
  zip VARCHAR(5) NULL,
  CONSTRAINT pk_supplier PRIMARY KEY (suppid)
)
;
CREATE TABLE product (
  productid VARCHAR(10) NOT NULL,
  categoryid VARCHAR(10) NOT NULL,
  name VARCHAR(80) NULL,
  descn VARCHAR(255) NULL,
  image VARCHAR(80) NULL,
  CONSTRAINT pk_product PRIMARY KEY (productid),
  CONSTRAINT fk_product_category FOREIGN KEY (categoryid) REFERENCES category (categoryid)
)
;
CREATE INDEX ix_product_category ON product (categoryid)
;
CREATE TABLE item (
  itemid VARCHAR(10) NOT NULL,
  productid VARCHAR(10) NOT NULL,
  listprice NUMERIC(10,2) NULL,
  unitcost NUMERIC(10,2) NULL,
  supplier INTEGER NULL,
  status VARCHAR(2) NULL,
  name VARCHAR(80) NULL,
  CONSTRAINT pk_item PRIMARY KEY (itemid),
  CONSTRAINT uq_item_name UNIQUE (name),
  CONSTRAINT fk_item_product FOREIGN KEY (productid) REFERENCES product (productid),
  CONSTRAINT fk_item_supplier FOREIGN KEY (supplier) REFERENCES supplier (suppid)
)
;
CREATE TABLE orders (
  orderid SERIAL NOT NULL,
  userid VARCHAR(20) NOT NULL,
  orderdate TIMESTAMP NOT NULL,
  shipcity VARCHAR(80) NOT NULL,
  totalprice NUMERIC(10,2) NOT NULL,
  locale VARCHAR(20) NOT NULL DEFAULT 'en-US',
  CONSTRAINT pk_orders PRIMARY KEY (orderid)
)
;
CREATE TABLE lineitem (
  orderid INTEGER NOT NULL,
  linenum INTEGER NOT NULL,
  itemid VARCHAR(10) NOT NULL,
  quantity INTEGER NOT NULL,
  unitprice NUMERIC(10,2) NOT NULL,
  CONSTRAINT pk_lineitem PRIMARY KEY (orderid, linenum),
  CONSTRAINT fk_lineitem_order FOREIGN KEY (orderid) REFERENCES orders (orderid),
  CONSTRAINT fk_lineitem_item FOREIGN KEY (itemid) REFERENCES item (itemid)
)
;
-- The rows of Category, Supplier, Product and Item, as PetShop.sql (SQL Server's) has them.
INSERT INTO category (categoryid, name, descn) VALUES
  ('BIRDS', 'Birds', 'Birds'),
  ('BUGS', 'Bugs', 'Bugs'),
  ('BYARD', 'Backyard', 'Backyard'),
  ('EDANGER', 'Endangered', 'Endangered'),
  ('FISH', 'Fish', 'Fish')
;
INSERT INTO supplier (suppid, name, status, city, zip) VALUES
  (1, 'XYZ Pets', 'AC', 'Los Angeles', '94024'),
  (2, 'ABC Pets', 'AC', 'San Francisco', '94024')
;
INSERT INTO product (productid, categoryid, name, descn, image) VALUES
  ('BD-01', 'BIRDS', 'Pelican', 'Will sit in your garden and admire nature', '~/Prod_Images/Birds/icon-pelican.gif'),
  ('BD-02', 'BIRDS', 'Penguin', 'Guaranteed to stay by your side', '~/Prod_Images/Birds/icon-penguin.gif'),
  ('BD-03', 'BIRDS', 'Pteranodon', 'Can''t let go of the past? This is your bird', '~/Prod_Images/Birds/icon-pteranodon.gif'),
  ('BD-04', 'BIRDS', 'Owl', 'Your personal dictionary ? night & day', '~/Prod_Images/Birds/icon-owl.gif'),
  ('BD-05', 'BIRDS', 'Duck', 'Lisps but otherwise sings well', '~/Prod_Images/Birds/icon-duck.gif'),
  ('BG-01', 'BUGS', 'Ant', 'Trash your vacuum cleaner', '~/Prod_Images/Bugs/icon-ant.gif'),
  ('BG-02', 'BUGS', 'Butterfly', 'Increased beauty with age', '~/Prod_Images/Bugs/icon-butterfly.gif'),
  ('BG-03', 'BUGS', 'Spider', 'Loves a good massage', '~/Prod_Images/Bugs/icon-spider.gif'),
  ('BG-04', 'BUGS', 'Slug', 'Your soft hugging buddy', '~/Prod_Images/Bugs/icon-slug.gif'),
  ('BG-05', 'BUGS', 'Frog', 'Want to get rid of an insect previously bought?', '~/Prod_Images/Bugs/icon-frog.gif'),
  ('BG-06', 'BUGS', 'Dragonfly', 'Beware of the meat lover', '~/Prod_Images/Bugs/icon-dragonfly.gif'),
  ('BY-01', 'BYARD', 'Sheep', 'Your soft hugging buddy', '~/Prod_Images/Backyard/icon-sheep.gif'),
  ('BY-02', 'BYARD', 'Cat', 'The friend you will never see', '~/Prod_Images/Backyard/icon-cat.gif'),
  ('BY-03', 'BYARD', 'Raccoon', 'Always keeps your dishes clean', '~/Prod_Images/Backyard/icon-raccoon.gif'),
  ('BY-04', 'BYARD', 'Goose', 'For your protection delivered in our special safety-bag', '~/Prod_Images/Backyard/icon-goose.gif'),
  ('BY-05', 'BYARD', 'Crab', 'The common house crab which lives in the refrigerator', '~/Prod_Images/Backyard/icon-crab.gif'),
  ('BY-06', 'BYARD', 'Skunk', 'You will love it - especially when you have your in-laws visiting', '~/Prod_Images/Backyard/icon-skunk.gif'),
  ('BY-07', 'BYARD', 'Zebra', 'The horse of the modern girl', '~/Prod_Images/Backyard/icon-zebra.gif'),
  ('DR-01', 'EDANGER', 'Skeleton', 'Dumb but hollow', '~/Prod_Images/Endangered/icon-skeleton.gif'),
  ('DR-02', 'EDANGER', 'Pet', 'The originals ? honestly!', '~/Prod_Images/Endangered/icon-pet.gif'),
  ('DR-03', 'EDANGER', 'Dino', 'Special offer: only for a limited time', '~/Prod_Images/Endangered/icon-dino.gif'),
  ('DR-04', 'EDANGER', 'Panda', 'Last one ? go for it!', '~/Prod_Images/Endangered/icon-panda.gif'),
  ('DR-05', 'EDANGER', 'Fish', 'They are waiting for your help', '~/Prod_Images/Endangered/icon-fish.gif'),
  ('FI-01', 'FISH', 'Meno', 'Your worried tiny friend warns you about life''s dangers', '~/Prod_Images/Fish/icon-meno.gif'),
  ('FI-02', 'FISH', 'Balloonfish', 'It''s your thermometer - the hotter it gets the bigger it gets', '~/Prod_Images/Fish/icon-ballonfish.gif'),
  ('FI-03', 'FISH', 'Blindfish', 'Likes pressure - ideal for divorcing couples', '~/Prod_Images/Fish/icon-blindfish.gif'),
  ('FI-04', 'FISH', 'Crabfish', 'Dances and sings every time you feed it!', '~/Prod_Images/Fish/icon-Crabfish.gif'),
  ('FI-05', 'FISH', 'Eucalyptus', 'For the tickle on your hands: you''ll love the massage', '~/Prod_Images/Fish/icon-eucalyptus.gif'),
  ('FI-06', 'FISH', 'Mister No', 'Need a companion for the dark times?', '~/Prod_Images/Fish/icon-misterno.gif'),
  ('FI-07', 'FISH', 'Nosyfish', 'Don''t underestimate this one - it bites!', '~/Prod_Images/Fish/icon-nosyfish.gif'),
  ('FI-08', 'FISH', 'Tooth Ferry', 'Very sensitive vegetarian, needs food every two months', '~/Prod_Images/Fish/icon-toothferry.gif')
;
INSERT INTO item (itemid, productid, listprice, unitcost, supplier, status, name) VALUES
  ('EST-1', 'FI-01', 16.50, 10.00, 1, 'P', 'Happy'),
  ('EST-10', 'FI-04', 18.50, 12.00, 1, 'P', 'Tap Dance'),
  ('EST-11', 'FI-04', 19.50, 12.00, 1, 'P', 'Ballet'),
  ('EST-12', 'FI-04', 18.90, 12.00, 1, 'P', 'Ballroom'),
  ('EST-13', 'FI-05', 16.50, 10.00, 1, 'P', 'Long Arms'),
  ('EST-14', 'FI-05', 16.90, 10.00, 1, 'P', 'Short Arms'),
  ('EST-15', 'FI-06', 13.90, 8.00, 1, 'P', 'Black'),
  ('EST-16', 'FI-06', 14.50, 8.00, 1, 'P', 'Sepia'),
  ('EST-17', 'FI-06', 15.50, 8.00, 1, 'P', 'Sable'),
  ('EST-18', 'FI-07', 16.50, 10.00, 1, 'P', 'Invidious'),
  ('EST-19', 'FI-07', 17.50, 10.00, 1, 'P', 'Beastly'),
  ('EST-2', 'FI-01', 17.50, 10.00, 1, 'P', 'Camouflage'),
  ('EST-20', 'FI-07', 18.50, 10.00, 1, 'P', 'Mean'),
  ('EST-21', 'FI-07', 19.50, 10.00, 1, 'P', 'Sneaky'),
  ('EST-22', 'FI-08', 28.50, 20.00, 1, 'P', 'Toothless'),
  ('EST-23', 'FI-08', 29.50, 20.00, 1, 'P', 'With Teeth'),
  ('EST-24', 'BY-01', 120.95, 99.00, 1, 'P', 'Fuzzy'),
  ('EST-25', 'BY-01', 130.95, 99.00, 1, 'P', 'Ironed'),
  ('EST-26', 'BY-02', 14.95, 2.00, 1, 'P', 'Transparent'),
  ('EST-27', 'BY-02', 15.95, 2.00, 1, 'P', 'Patterned'),
  ('EST-28', 'BY-02', 18.95, 2.00, 1, 'P', 'Uncolored'),
  ('EST-29', 'BY-03', 42.95, 30.00, 1, 'P', 'Long Tongue'),
  ('EST-3', 'FI-01', 15.90, 10.00, 1, 'P', 'Worried'),
  ('EST-30', 'BY-03', 45.95, 30.00, 1, 'P', 'Rough Tongue'),
  ('EST-31', 'BY-03', 48.95, 30.00, 1, 'P', 'Hairy Tongue'),
  ('EST-32', 'BY-04', 20.95, 12.00, 1, 'P', 'Feathered'),
  ('EST-33', 'BY-04', 22.95, 12.00, 1, 'P', 'Plucked'),
  ('EST-34', 'BY-05', 12.95, 10.00, 1, 'P', 'Red'),
  ('EST-35', 'BY-05', 13.95, 10.00, 1, 'P', 'Orange'),
  ('EST-36', 'BY-05', 14.95, 10.00, 1, 'P', 'Dotted'),
  ('EST-37', 'BY-06', 18.95, 12.00, 1, 'P', 'Bad Smell'),
  ('EST-38', 'BY-06', 20.95, 12.00, 1, 'P', 'Really Bad Smell'),
  ('EST-39', 'BY-06', 22.95, 12.00, 1, 'P', 'Worst Smell'),
  ('EST-4', 'FI-02', 17.50, 12.00, 1, 'P', 'Extra Stretch'),
  ('EST-40', 'BY-07', 859.95, 500.00, 1, 'P', 'Tiny'),
  ('EST-41', 'BY-07', 879.95, 500.00, 1, 'P', 'Small'),
  ('EST-42', 'BY-07', 899.95, 500.00, 1, 'P', 'Medium'),
  ('EST-43', 'BY-07', 949.95, 500.00, 1, 'P', 'Large'),
  ('EST-44', 'BD-01', 41.95, 30.00, 1, 'P', 'Flower Loving'),
  ('EST-45', 'BD-01', 45.95, 30.00, 1, 'P', 'Grass Loving'),
  ('EST-46', 'BD-02', 120.99, 99.00, 1, 'P', 'Adventurous'),
  ('EST-47', 'BD-02', 130.99, 99.00, 1, 'P', 'Homey'),
  ('EST-48', 'BD-03', 130.99, 99.00, 1, 'P', 'Old'),
  ('EST-49', 'BD-03', 130.99, 99.00, 1, 'P', 'Ancient'),
  ('EST-5', 'FI-02', 18.90, 12.00, 1, 'P', 'Natural'),
  ('EST-50', 'BD-04', 80.99, 50.00, 1, 'P', 'Day'),
  ('EST-51', 'BD-04', 85.99, 50.00, 1, 'P', 'Night'),
  ('EST-52', 'BD-05', 33.99, 20.00, 1, 'P', 'Domestic'),
  ('EST-53', 'BD-05', 38.99, 20.00, 1, 'P', 'Wild'),
  ('EST-54', 'BG-01', 0.25, 0.02, 1, 'P', 'Worker'),
  ('EST-55', 'BG-01', 0.30, 0.02, 1, 'P', 'Queen'),
  ('EST-56', 'BG-01', 0.50, 0.02, 1, 'P', 'Soldier'),
  ('EST-57', 'BG-02', 0.70, 0.10, 1, 'P', 'Larva'),
  ('EST-58', 'BG-02', 0.80, 0.10, 1, 'P', 'Pupa'),
  ('EST-59', 'BG-02', 0.90, 0.10, 1, 'P', 'Adult'),
  ('EST-6', 'FI-02', 19.50, 12.00, 1, 'P', 'Flammable'),
  ('EST-60', 'BG-03', 2.50, 1.00, 1, 'P', 'Arniladisplicata'),
  ('EST-61', 'BG-03', 2.60, 1.00, 1, 'P', 'Dysdera Crocata'),
  ('EST-62', 'BG-04', 0.89, 0.15, 1, 'P', 'Naked'),
  ('EST-63', 'BG-04', 0.99, 0.15, 1, 'P', 'Habitat'),
  ('EST-64', 'BG-05', 8.99, 2.50, 1, 'P', 'False'),
  ('EST-65', 'BG-05', 9.99, 2.50, 1, 'P', 'True'),
  ('EST-66', 'BG-06', 1.20, 0.80, 1, 'P', 'Omnivore'),
  ('EST-67', 'BG-06', 1.30, 0.80, 1, 'P', 'Vegetarian'),
  ('EST-68', 'BG-06', 1.40, 0.80, 1, 'P', 'Vegan'),
  ('EST-69', 'DR-01', 150.00, 100.00, 1, 'P', 'Male'),
  ('EST-7', 'FI-03', 21.50, 15.00, 1, 'P', 'Blind'),
  ('EST-70', 'DR-01', 160.00, 100.00, 1, 'P', 'Female'),
  ('EST-71', 'DR-01', 170.00, 100.00, 1, 'P', 'Aphrodite'),
  ('EST-72', 'DR-01', 180.00, 100.00, 1, 'P', 'Hermaphrodite'),
  ('EST-73', 'DR-02', 45.00, 22.00, 1, 'P', 'Rover'),
  ('EST-74', 'DR-02', 48.00, 22.00, 1, 'P', 'Trumpet'),
  ('EST-75', 'DR-02', 49.00, 22.00, 1, 'P', 'Kitty'),
  ('EST-76', 'DR-03', 349.00, 220.00, 1, 'P', 'Spiky'),
  ('EST-77', 'DR-03', 379.00, 220.00, 1, 'P', 'Shaved'),
  ('EST-78', 'DR-03', 399.00, 220.00, 1, 'P', 'Pointy'),
  ('EST-79', 'DR-04', 1999.00, 1500.00, 1, 'P', 'Exclusive'),
  ('EST-8', 'FI-03', 22.50, 15.00, 1, 'P', 'Short Sighted'),
  ('EST-80', 'DR-05', 22.95, 16.00, 1, 'P', 'Lost'),
  ('EST-81', 'DR-05', 22.95, 16.00, 1, 'P', 'Drunk'),
  ('EST-82', 'DR-05', 22.95, 16.00, 1, 'P', 'Caught'),
  ('EST-9', 'FI-03', 24.50, 15.00, 1, 'P', 'Far Sighted')
;
CREATE VIEW productcategory AS
SELECT p.productid, p.name AS productname, c.categoryid, c.name AS categoryname
FROM product p INNER JOIN category c ON c.categoryid = p.categoryid
;
CREATE FUNCTION get_products_by_category(category_id VARCHAR(10))
RETURNS TABLE (productid VARCHAR(10), name VARCHAR(80), descn VARCHAR(255))
LANGUAGE sql AS $$
  SELECT p.productid, p.name, p.descn FROM product p WHERE p.categoryid = category_id ORDER BY p.name
$$
;
