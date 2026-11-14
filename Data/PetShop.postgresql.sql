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
