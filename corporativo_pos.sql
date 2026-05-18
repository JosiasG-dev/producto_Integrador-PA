CREATE DATABASE IF NOT EXISTS corporativo_pos
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE corporativo_pos;

CREATE TABLE IF NOT EXISTS usuarios (
    id       INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50)  NOT NULL UNIQUE,
    password VARCHAR(100) NOT NULL,
    rol      ENUM('ADMINISTRADOR','CAJERO') NOT NULL DEFAULT 'CAJERO',
    nombre   VARCHAR(100) NOT NULL,
    edad     INT          NOT NULL DEFAULT 18,
    sexo     VARCHAR(20)  NOT NULL DEFAULT 'Otro'
);

CREATE TABLE IF NOT EXISTS productos (
    id           VARCHAR(20)   PRIMARY KEY,
    nombre       VARCHAR(150)  NOT NULL,
    precio       DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    stock        DECIMAL(10,3) NOT NULL DEFAULT 0.000,
    stock_minimo DOUBLE        NOT NULL DEFAULT 5,
    categoria    VARCHAR(100)  NOT NULL,
    unidad       VARCHAR(30)   NOT NULL DEFAULT 'Piezas',
    imagen_ruta  VARCHAR(500)           DEFAULT ''
);

CREATE TABLE IF NOT EXISTS proveedores (
    id        INT AUTO_INCREMENT PRIMARY KEY,
    nombre    VARCHAR(150) NOT NULL,
    contacto  VARCHAR(100),
    telefono  VARCHAR(20),
    email     VARCHAR(100),
    direccion VARCHAR(200),
    activo    TINYINT(1)   NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS ventas (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    total       DECIMAL(10,2) NOT NULL,
    metodo_pago VARCHAR(30)   NOT NULL DEFAULT 'Efectivo',
    fecha       DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    cajero      VARCHAR(100)  NOT NULL
);

CREATE TABLE IF NOT EXISTS venta_items (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    venta_id    INT           NOT NULL,
    producto_id VARCHAR(20)   NOT NULL,
    cantidad    DECIMAL(10,3) NOT NULL,
    precio_unit DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (venta_id)    REFERENCES ventas(id)    ON DELETE CASCADE,
    FOREIGN KEY (producto_id) REFERENCES productos(id)
);

CREATE TABLE IF NOT EXISTS movimientos (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    tipo        ENUM('VENTA','RETIRO','FONDO_INICIAL') NOT NULL,
    descripcion VARCHAR(200)  NOT NULL,
    monto       DECIMAL(10,2) NOT NULL,
    fecha       DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    usuario     VARCHAR(100)  NOT NULL
);

CREATE TABLE IF NOT EXISTS ordenes_compra (
    id           INT AUTO_INCREMENT PRIMARY KEY,
    proveedor_id INT           NOT NULL,
    total        DECIMAL(10,2) NOT NULL,
    tipo_pago    ENUM('CONTADO','CREDITO')    NOT NULL DEFAULT 'CONTADO',
    estado       ENUM('PENDIENTE','RECIBIDO') NOT NULL DEFAULT 'PENDIENTE',
    fecha        DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (proveedor_id) REFERENCES proveedores(id)
);

CREATE TABLE IF NOT EXISTS orden_items (
    id                  INT AUTO_INCREMENT PRIMARY KEY,
    orden_id            INT           NOT NULL,
    producto_id         VARCHAR(20)   NOT NULL,
    cantidad_solicitada DECIMAL(10,3) NOT NULL,
    precio_costo        DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (orden_id)    REFERENCES ordenes_compra(id) ON DELETE CASCADE,
    FOREIGN KEY (producto_id) REFERENCES productos(id)
);

CREATE TABLE IF NOT EXISTS cuentas_por_pagar (
    id           INT AUTO_INCREMENT PRIMARY KEY,
    proveedor_id INT           NOT NULL,
    orden_id     INT           NOT NULL,
    monto_total  DECIMAL(10,2) NOT NULL,
    pagado       DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    saldo        DECIMAL(10,2) NOT NULL,
    vencimiento  VARCHAR(50)   NOT NULL DEFAULT '7 dias',
    estado       ENUM('PENDIENTE','PARCIAL','PAGADO') NOT NULL DEFAULT 'PENDIENTE',
    FOREIGN KEY (proveedor_id) REFERENCES proveedores(id),
    FOREIGN KEY (orden_id)     REFERENCES ordenes_compra(id)
);

CREATE TABLE IF NOT EXISTS devoluciones (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    venta_id    INT          NOT NULL,
    producto_id VARCHAR(20)  NOT NULL,
    cantidad    DOUBLE       NOT NULL,
    motivo      VARCHAR(255),
    fecha       TIMESTAMP    DEFAULT CURRENT_TIMESTAMP,
    cajero      VARCHAR(100),
    FOREIGN KEY (venta_id)    REFERENCES ventas(id),
    FOREIGN KEY (producto_id) REFERENCES productos(id)
);

CREATE TABLE IF NOT EXISTS configuracion (
    id            INT          PRIMARY KEY,
    nombre_tienda VARCHAR(255),
    sucursal      VARCHAR(255),
    rfc           VARCHAR(20)
);

-- ============================================================
-- DATOS INICIALES
-- ============================================================
INSERT IGNORE INTO configuracion (id, nombre_tienda, sucursal, rfc)
VALUES (1, 'CORPORATIVO POS', 'Sucursal Principal - Centro', 'XAXX010101000');

INSERT IGNORE INTO usuarios (username, password, rol, nombre, edad, sexo) VALUES
('admin',   'admin123', 'ADMINISTRADOR', 'Director General', 45, 'Masculino'),
('cajero1', 'caja123',  'CAJERO',        'Juan Jose',        28, 'Masculino');

INSERT IGNORE INTO proveedores (nombre, contacto, telefono, email, direccion, activo) VALUES
('GRUPO BIMBO',       'Juan Perez',  '8112345678', 'bimbo@proveedor.mx',    'Monterrey, NL',  1),
('COCA COLA FEMSA',   'Ana Torres',  '8187654321', 'femsa@proveedor.mx',    'Guadalupe, NL',  1),
('BARCEL S.A.',       'Carlos Ruiz', '8199991111', 'barcel@proveedor.mx',   'CDMX',           1),
('LALA S.A. DE C.V.','Maria Lopez',  '8133334444', 'lala@proveedor.mx',     'Torreon, Coah',  1),
('NESTLE MEXICO',     'Pedro Hdz',   '5512345678', 'nestle@proveedor.mx',   'CDMX',           1),
('UNILEVER MEXICO',   'Sofia Ruiz',  '5598765432', 'unilever@prov.mx',      'Monterrey, NL',  1);

-- ============================================================
-- 150 PRODUCTOS (9 categorias)
-- Si ya tienes productos viejos, borralos primero para evitar conflictos
-- ============================================================

-- Borrar los 5 productos originales (si existen) para reemplazarlos con los 150 nuevos
DELETE FROM orden_items  WHERE producto_id IN ('001','002','003','004','005');
DELETE FROM venta_items  WHERE producto_id IN ('001','002','003','004','005');
DELETE FROM devoluciones WHERE producto_id IN ('001','002','003','004','005');
DELETE FROM productos    WHERE id IN ('001','002','003','004','005');

-- DESPENSA BASICA (20)
INSERT IGNORE INTO productos (id,nombre,precio,stock,stock_minimo,categoria,unidad,imagen_ruta) VALUES
('001','Arroz Verde Valle 1kg',25.00,60,10,'Despensa Basica','Kilogramos / g','Images/1.jpg'),
('002','Frijol Negro Isadora 500g',18.50,50,10,'Despensa Basica','Kilogramos / g','Images/2.jpg'),
('003','Azucar Zulka 1kg',32.00,45,10,'Despensa Basica','Kilogramos / g','Images/3.jpg'),
('004','Aceite Nutrioli 1L',48.00,40,8,'Despensa Basica','Litros / mL','Images/4.jpg'),
('005','Sal La Fina 1kg',12.00,55,10,'Despensa Basica','Kilogramos / g','Images/5.jpg'),
('006','Harina Maseca 1kg',28.00,50,10,'Despensa Basica','Kilogramos / g','Images/6.jpg'),
('007','Pasta Barilla Spaghetti 500g',22.00,45,8,'Despensa Basica','Kilogramos / g','Images/7.jpg'),
('008','Sopa Maruchan Camaron',8.50,80,15,'Despensa Basica','Piezas','Images/8.jpg'),
('009','Lentejas La Merced 500g',19.00,35,8,'Despensa Basica','Kilogramos / g','Images/9.jpg'),
('010','Avena Quaker 500g',38.00,40,8,'Despensa Basica','Kilogramos / g','Images/10.jpg'),
('011','Vinagre Clemente Jacques 500mL',18.00,30,5,'Despensa Basica','Litros / mL','Images/11.jpg'),
('012','Canela Entera McCormick 30g',15.00,25,5,'Despensa Basica','Kilogramos / g','Images/12.jpg'),
('013','Consome de Pollo Knorr 180g',24.00,50,10,'Despensa Basica','Kilogramos / g','Images/13.jpg'),
('014','Chile Guajillo 100g',12.00,35,8,'Despensa Basica','Kilogramos / g','Images/14.jpg'),
('015','Maicena 250g',16.00,30,5,'Despensa Basica','Kilogramos / g','Images/15.jpg'),
('016','Cafe Nescafe Clasico 135g',75.00,30,5,'Despensa Basica','Kilogramos / g','Images/16.jpg'),
('017','Te Lipton 25 sobres',32.00,25,5,'Despensa Basica','Piezas','Images/17.jpg'),
('018','Fideo Corto La Moderna 200g',10.00,60,10,'Despensa Basica','Kilogramos / g','Images/18.jpg'),
('019','Semola Tres Generaciones 400g',17.00,20,5,'Despensa Basica','Kilogramos / g','Images/19.jpg'),
('020','Polvo para Hornear Royal 80g',14.00,25,5,'Despensa Basica','Kilogramos / g','Images/20.jpg');

-- LACTEOS Y HUEVO (15)
INSERT IGNORE INTO productos (id,nombre,precio,stock,stock_minimo,categoria,unidad,imagen_ruta) VALUES
('021','Leche Lala Entera 1L',22.00,50,10,'Lacteos y Huevo','Litros / mL','Images/21.jpg'),
('022','Leche Alpura 1L',23.00,40,10,'Lacteos y Huevo','Litros / mL','Images/22.jpg'),
('023','Huevo San Juan 1kg',52.00,45,8,'Lacteos y Huevo','Kilogramos / g','Images/23.jpg'),
('024','Crema Lala 200mL',22.00,30,6,'Lacteos y Huevo','Litros / mL','Images/24.jpg'),
('025','Queso Manchego Lala 400g',85.00,20,5,'Lacteos y Huevo','Kilogramos / g','Images/25.jpg'),
('026','Yogurt Danone Natural 1kg',48.00,25,5,'Lacteos y Huevo','Kilogramos / g','Images/26.jpg'),
('027','Mantequilla Lala 90g',28.00,30,6,'Lacteos y Huevo','Kilogramos / g','Images/27.jpg'),
('028','Leche Condensada Nestle 397g',42.00,30,6,'Lacteos y Huevo','Kilogramos / g','Images/28.jpg'),
('029','Queso Panela Covadonga 400g',72.00,15,5,'Lacteos y Huevo','Kilogramos / g','Images/29.jpg'),
('030','Leche Evaporada Carnation 360mL',28.00,25,5,'Lacteos y Huevo','Litros / mL','Images/30.jpg'),
('031','Yogurt Yoplait Fresa 150g',18.00,35,8,'Lacteos y Huevo','Kilogramos / g','Images/31.jpg'),
('032','Crema Acida Sello Rojo 250mL',19.00,20,5,'Lacteos y Huevo','Litros / mL','Images/32.jpg'),
('033','Jocoque Seco Lala 190g',22.00,15,4,'Lacteos y Huevo','Kilogramos / g','Images/33.jpg'),
('034','Requeson Dos Pinos 250g',30.00,12,4,'Lacteos y Huevo','Kilogramos / g','Images/34.jpg'),
('035','Leche de Coco Goya 400mL',28.00,18,5,'Lacteos y Huevo','Litros / mL','Images/35.jpg');

-- BEBIDAS Y LIQUIDOS (20)
INSERT IGNORE INTO productos (id,nombre,precio,stock,stock_minimo,categoria,unidad,imagen_ruta) VALUES
('036','Coca-Cola 600mL',18.00,80,15,'Bebidas y Liquidos','Litros / mL','Images/36.jpg'),
('037','Pepsi 600mL',17.00,60,15,'Bebidas y Liquidos','Litros / mL','Images/37.jpg'),
('038','Agua Ciel 1L',12.00,100,20,'Bebidas y Liquidos','Litros / mL','Images/38.jpg'),
('039','Agua Epura 1.5L',15.00,80,15,'Bebidas y Liquidos','Litros / mL','Images/39.jpg'),
('040','Jugo Jumex Mango 473mL',18.00,50,10,'Bebidas y Liquidos','Litros / mL','Images/40.jpg'),
('041','Jugo Del Valle Naranja 450mL',18.00,45,10,'Bebidas y Liquidos','Litros / mL','Images/41.jpg'),
('042','Sprite 600mL',17.00,50,10,'Bebidas y Liquidos','Litros / mL','Images/42.jpg'),
('043','Fanta Naranja 600mL',17.00,45,10,'Bebidas y Liquidos','Litros / mL','Images/43.jpg'),
('044','Powerade Mora Azul 500mL',20.00,35,8,'Bebidas y Liquidos','Litros / mL','Images/44.jpg'),
('045','Gatorade Limon 600mL',20.00,35,8,'Bebidas y Liquidos','Litros / mL','Images/45.jpg'),
('046','Red Bull 250mL',38.00,25,5,'Bebidas y Liquidos','Litros / mL','Images/46.jpg'),
('047','Monster Energy 473mL',42.00,20,5,'Bebidas y Liquidos','Litros / mL','Images/47.jpg'),
('048','Sidral Mundet 600mL',16.00,40,8,'Bebidas y Liquidos','Litros / mL','Images/48.jpg'),
('049','Boing Guayaba 250mL',10.00,60,12,'Bebidas y Liquidos','Litros / mL','Images/49.jpg'),
('050','Lipton Te Limon 600mL',17.00,40,8,'Bebidas y Liquidos','Litros / mL','Images/50.jpg'),
('051','Nestea Durazno 500mL',17.00,35,8,'Bebidas y Liquidos','Litros / mL','Images/51.jpg'),
('052','Squirt 600mL',16.00,30,8,'Bebidas y Liquidos','Litros / mL','Images/52.jpg'),
('053','Sangria Senorial 600mL',14.00,30,8,'Bebidas y Liquidos','Litros / mL','Images/53.jpg'),
('054','Agua Mineral Penafiel 600mL',13.00,40,8,'Bebidas y Liquidos','Litros / mL','Images/54.jpg'),
('055','Horchata Pascual 500mL',15.00,25,5,'Bebidas y Liquidos','Litros / mL','Images/55.jpg');

-- BOTANAS Y DULCES (15)
INSERT IGNORE INTO productos (id,nombre,precio,stock,stock_minimo,categoria,unidad,imagen_ruta) VALUES
('056','Sabritas Original 45g',14.00,60,12,'Botanas y Dulces','Kilogramos / g','Images/56.jpg'),
('057','Ruffles 45g',14.00,55,10,'Botanas y Dulces','Kilogramos / g','Images/57.jpg'),
('058','Doritos Nacho 65g',16.00,50,10,'Botanas y Dulces','Kilogramos / g','Images/58.jpg'),
('059','Cheetos Flamin Hot 55g',14.00,45,10,'Botanas y Dulces','Kilogramos / g','Images/59.jpg'),
('060','Palomitas Popcorners 28g',15.00,30,8,'Botanas y Dulces','Kilogramos / g','Images/60.jpg'),
('061','Cacahuates Barcel 70g',15.00,40,8,'Botanas y Dulces','Kilogramos / g','Images/61.jpg'),
('062','Duvalin Vainilla-Fresa',8.00,60,12,'Botanas y Dulces','Piezas','Images/62.jpg'),
('063','Gansito Marinela',16.00,50,10,'Botanas y Dulces','Piezas','Images/63.jpg'),
('064','Pinguino Chocolate',14.00,45,10,'Botanas y Dulces','Piezas','Images/64.jpg'),
('065','Bubulubu',8.00,70,15,'Botanas y Dulces','Piezas','Images/65.jpg'),
('066','Paleta Payaso',6.00,80,15,'Botanas y Dulces','Piezas','Images/66.jpg'),
('067','Chicle Trident Menta 14 pzas',14.00,40,8,'Botanas y Dulces','Piezas','Images/67.jpg'),
('068','Chocolate Carlos V 18g',7.00,60,12,'Botanas y Dulces','Piezas','Images/68.jpg'),
('069','Mazapan De La Rosa',6.00,70,15,'Botanas y Dulces','Piezas','Images/69.jpg'),
('070','Takis Fuego 56g',16.00,55,10,'Botanas y Dulces','Kilogramos / g','Images/70.jpg');

-- FRUTAS Y VERDURAS (10)
INSERT IGNORE INTO productos (id,nombre,precio,stock,stock_minimo,categoria,unidad,imagen_ruta) VALUES
('071','Jitomate Saladet 1kg',28.00,30,8,'Frutas y Verduras','Kilogramos / g','Images/71.jpg'),
('072','Cebolla Blanca 1kg',22.00,35,8,'Frutas y Verduras','Kilogramos / g','Images/72.jpg'),
('073','Papa Blanca 1kg',18.00,40,10,'Frutas y Verduras','Kilogramos / g','Images/73.jpg'),
('074','Zanahoria 1kg',15.00,35,8,'Frutas y Verduras','Kilogramos / g','Images/74.jpg'),
('075','Chile Serrano 500g',20.00,25,5,'Frutas y Verduras','Kilogramos / g','Images/75.jpg'),
('076','Limon 1kg',25.00,30,8,'Frutas y Verduras','Kilogramos / g','Images/76.jpg'),
('077','Platano Tabasco 1kg',20.00,25,5,'Frutas y Verduras','Kilogramos / g','Images/77.jpg'),
('078','Manzana Roja 1kg',45.00,20,5,'Frutas y Verduras','Kilogramos / g','Images/78.jpg'),
('079','Ajo 100g',18.00,25,5,'Frutas y Verduras','Kilogramos / g','Images/79.jpg'),
('080','Epazote 100g',8.00,20,5,'Frutas y Verduras','Kilogramos / g','Images/80.jpg');

-- CARNES Y SALCHICHONERIA (10)
INSERT IGNORE INTO productos (id,nombre,precio,stock,stock_minimo,categoria,unidad,imagen_ruta) VALUES
('081','Jamon Salchicha FUD 500g',65.00,20,5,'Carnes y Salchichoneria','Kilogramos / g','Images/81.jpg'),
('082','Salchicha Viena FUD 500g',55.00,20,5,'Carnes y Salchichoneria','Kilogramos / g','Images/82.jpg'),
('083','Chorizo San Rafael 500g',70.00,15,4,'Carnes y Salchichoneria','Kilogramos / g','Images/83.jpg'),
('084','Mortadela Campestre 200g',38.00,15,4,'Carnes y Salchichoneria','Kilogramos / g','Images/84.jpg'),
('085','Tocino Rebanado Kir 200g',55.00,12,4,'Carnes y Salchichoneria','Kilogramos / g','Images/85.jpg'),
('086','Pechuga de Pollo 1kg',105.00,15,4,'Carnes y Salchichoneria','Kilogramos / g','Images/86.jpg'),
('087','Carne Molida Res 500g',85.00,12,4,'Carnes y Salchichoneria','Kilogramos / g','Images/87.jpg'),
('088','Milanesa de Res 500g',95.00,10,3,'Carnes y Salchichoneria','Kilogramos / g','Images/88.jpg'),
('089','Queso de Puerco 200g',32.00,15,4,'Carnes y Salchichoneria','Kilogramos / g','Images/89.jpg'),
('090','Spam 340g',55.00,18,5,'Carnes y Salchichoneria','Kilogramos / g','Images/90.jpg');

-- ALIMENTOS PREPARADOS/ENLATADOS (15)
INSERT IGNORE INTO productos (id,nombre,precio,stock,stock_minimo,categoria,unidad,imagen_ruta) VALUES
('091','Atun Herdez 140g',19.00,50,10,'Alimentos Preparados/Enlatados','Kilogramos / g','Images/91.jpg'),
('092','Sardina Clasica Caru 215g',22.00,40,8,'Alimentos Preparados/Enlatados','Kilogramos / g','Images/92.jpg'),
('093','Chile Chipotles La Costena 210g',22.00,35,8,'Alimentos Preparados/Enlatados','Kilogramos / g','Images/93.jpg'),
('094','Frijoles Refritos La Sierra 430g',28.00,40,8,'Alimentos Preparados/Enlatados','Kilogramos / g','Images/94.jpg'),
('095','Sopa de Tomate Campbells 305g',28.00,30,6,'Alimentos Preparados/Enlatados','Kilogramos / g','Images/95.jpg'),
('096','Maiz Dulce Caru 400g',22.00,35,8,'Alimentos Preparados/Enlatados','Kilogramos / g','Images/96.jpg'),
('097','Durazno en Almibar Del Monte 820g',42.00,25,5,'Alimentos Preparados/Enlatados','Kilogramos / g','Images/97.jpg'),
('098','Pasta de Tomate Del Fuerte 70g',10.00,50,10,'Alimentos Preparados/Enlatados','Kilogramos / g','Images/98.jpg'),
('099','Sopa Instantanea Nissin 85g',8.50,60,12,'Alimentos Preparados/Enlatados','Piezas','Images/99.jpg'),
('100','Chiles Jalapenos La Costena 220g',19.00,35,8,'Alimentos Preparados/Enlatados','Kilogramos / g','Images/100.jpg'),
('101','Mole Dona Maria 235g',42.00,20,5,'Alimentos Preparados/Enlatados','Kilogramos / g','Images/101.jpg'),
('102','Pozole Maseca 200g',25.00,25,5,'Alimentos Preparados/Enlatados','Kilogramos / g','Images/102.jpg'),
('103','Aceitunas La Fe 200g',35.00,18,4,'Alimentos Preparados/Enlatados','Kilogramos / g','Images/103.jpg'),
('104','Mermelada Smuckers Fresa 340g',45.00,20,5,'Alimentos Preparados/Enlatados','Kilogramos / g','Images/104.jpg'),
('105','Mantequilla de Cacahuate Jif 340g',65.00,15,4,'Alimentos Preparados/Enlatados','Kilogramos / g','Images/105.jpg');

-- CUIDADO DEL HOGAR (25)
INSERT IGNORE INTO productos (id,nombre,precio,stock,stock_minimo,categoria,unidad,imagen_ruta) VALUES
('106','Detergente Ariel 1kg',85.00,30,6,'Cuidado del Hogar','Kilogramos / g','Images/106.jpg'),
('107','Suavizante Downy 800mL',65.00,25,5,'Cuidado del Hogar','Litros / mL','Images/107.jpg'),
('108','Jabon Roma 250g',14.00,50,10,'Cuidado del Hogar','Kilogramos / g','Images/108.jpg'),
('109','Jabon Zote Rosa 400g',18.00,40,8,'Cuidado del Hogar','Kilogramos / g','Images/109.jpg'),
('110','Cloro Cloralex 960mL',22.00,35,8,'Cuidado del Hogar','Litros / mL','Images/110.jpg'),
('111','Pinol Limon 828mL',38.00,25,5,'Cuidado del Hogar','Litros / mL','Images/111.jpg'),
('112','Fabuloso Lavanda 1L',35.00,30,6,'Cuidado del Hogar','Litros / mL','Images/112.jpg'),
('113','Esponja Scotch Brite Verde',9.00,50,10,'Cuidado del Hogar','Piezas','Images/113.jpg'),
('114','Bolsas Basura 10 pzas',22.00,40,8,'Cuidado del Hogar','Piezas','Images/114.jpg'),
('115','Trapo Microfibra',18.00,30,6,'Cuidado del Hogar','Piezas','Images/115.jpg'),
('116','Insecticida Raid 400mL',85.00,15,4,'Cuidado del Hogar','Litros / mL','Images/116.jpg'),
('117','Papel de Bano Petalo 4 rollos',38.00,40,8,'Cuidado del Hogar','Piezas','Images/117.jpg'),
('118','Servilletas Estrella 150 pzas',22.00,35,8,'Cuidado del Hogar','Piezas','Images/118.jpg'),
('119','Papel Aluminio Wyandotte 10m',22.00,20,5,'Cuidado del Hogar','Piezas','Images/119.jpg'),
('120','Papel Film Stretch 30cm x 20m',25.00,20,5,'Cuidado del Hogar','Piezas','Images/120.jpg'),
('121','Jabon Palmolive Trastes 750mL',42.00,25,5,'Cuidado del Hogar','Litros / mL','Images/121.jpg'),
('122','Quita Sarro Harpic 500mL',45.00,15,4,'Cuidado del Hogar','Litros / mL','Images/122.jpg'),
('123','Limpia Vidrios Windex 500mL',42.00,15,4,'Cuidado del Hogar','Litros / mL','Images/123.jpg'),
('124','Guantes de Latex Talla M',18.00,30,6,'Cuidado del Hogar','Piezas','Images/124.jpg'),
('125','Detergente Salvo Liquido 1L',52.00,20,5,'Cuidado del Hogar','Litros / mL','Images/125.jpg'),
('126','Suavitel Primavera 800mL',55.00,20,5,'Cuidado del Hogar','Litros / mL','Images/126.jpg'),
('127','Blanqueador ACE 800mL',28.00,18,5,'Cuidado del Hogar','Litros / mL','Images/127.jpg'),
('128','Cera Johnson 250mL',48.00,10,3,'Cuidado del Hogar','Litros / mL','Images/128.jpg'),
('129','Focos LED 9W paquete 2',55.00,20,5,'Cuidado del Hogar','Piezas','Images/129.jpg'),
('130','Pilas Duracell AA 4 pzas',48.00,25,5,'Cuidado del Hogar','Piezas','Images/130.jpg');

-- HIGIENE Y CUIDADO PERSONAL (20)
INSERT IGNORE INTO productos (id,nombre,precio,stock,stock_minimo,categoria,unidad,imagen_ruta) VALUES
('131','Shampoo Head and Shoulders 375mL',85.00,20,5,'Higiene y Cuidado Personal','Litros / mL','Images/131.jpg'),
('132','Acondicionador Pantene 375mL',75.00,18,4,'Higiene y Cuidado Personal','Litros / mL','Images/132.jpg'),
('133','Jabon Dove Original 135g',28.00,40,8,'Higiene y Cuidado Personal','Kilogramos / g','Images/133.jpg'),
('134','Desodorante Rexona Men 150mL',52.00,25,5,'Higiene y Cuidado Personal','Litros / mL','Images/134.jpg'),
('135','Pasta Dental Colgate 100mL',38.00,35,8,'Higiene y Cuidado Personal','Litros / mL','Images/135.jpg'),
('136','Cepillo Dental Oral-B Medio',28.00,30,6,'Higiene y Cuidado Personal','Piezas','Images/136.jpg'),
('137','Papel Higienico Scott 4 rollos',42.00,30,6,'Higiene y Cuidado Personal','Piezas','Images/137.jpg'),
('138','Crema Nivea 150mL',55.00,20,5,'Higiene y Cuidado Personal','Litros / mL','Images/138.jpg'),
('139','Rastrillo Gillette 2 pzas',35.00,20,5,'Higiene y Cuidado Personal','Piezas','Images/139.jpg'),
('140','Toallas Femeninas Kotex 10 pzas',35.00,25,5,'Higiene y Cuidado Personal','Piezas','Images/140.jpg'),
('141','Panuelos Kleenex 100 pzas',25.00,30,6,'Higiene y Cuidado Personal','Piezas','Images/141.jpg'),
('142','Gel Antibacterial Dettol 250mL',45.00,30,6,'Higiene y Cuidado Personal','Litros / mL','Images/142.jpg'),
('143','Hilo Dental Oral-B Menta 50m',22.00,20,5,'Higiene y Cuidado Personal','Piezas','Images/143.jpg'),
('144','Shampoo Sedal Ceramidas 350mL',72.00,18,4,'Higiene y Cuidado Personal','Litros / mL','Images/144.jpg'),
('145','Desodorante Lady Speed Stick 45g',38.00,20,5,'Higiene y Cuidado Personal','Kilogramos / g','Images/145.jpg'),
('146','Crema Ponds Clarant B3 200g',75.00,15,4,'Higiene y Cuidado Personal','Kilogramos / g','Images/146.jpg'),
('147','Protector Solar Banana Boat SPF50',98.00,12,3,'Higiene y Cuidado Personal','Kilogramos / g','Images/147.jpg'),
('148','Cotonetes Jumbo 100 pzas',18.00,25,5,'Higiene y Cuidado Personal','Piezas','Images/148.jpg'),
('149','Isodine Bucofaringeo 240mL',65.00,15,4,'Higiene y Cuidado Personal','Litros / mL','Images/149.jpg'),
('150','Crema Barba Gillette 175mL',52.00,18,4,'Higiene y Cuidado Personal','Litros / mL','Images/150.jpg');


-- Desactivar la revisión de llaves foráneas temporalmente
SET FOREIGN_KEY_CHECKS = 0;

-- Vaciar las tablas de historial para evitar conflictos
TRUNCATE TABLE movimientos;
TRUNCATE TABLE devoluciones;
TRUNCATE TABLE cuentas_por_pagar;
TRUNCATE TABLE ordenes_compra_detalles;
TRUNCATE TABLE ordenes_compra;
TRUNCATE TABLE ventas_detalles;
TRUNCATE TABLE ventas;

-- Vaciar la tabla de productos (esto obligará al sistema a recargarlos desde DatosIniciales.java)
TRUNCATE TABLE productos;

-- Volver a activar la revisión de llaves foráneas
SET FOREIGN_KEY_CHECKS = 1;


