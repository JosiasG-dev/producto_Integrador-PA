CREATE DATABASE IF NOT EXISTS corporativo_pos
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE corporativo_pos;

CREATE TABLE IF NOT EXISTS usuarios (
    id         INT AUTO_INCREMENT PRIMARY KEY,
    username   VARCHAR(50)  NOT NULL UNIQUE,
    password   VARCHAR(100) NOT NULL,
    rol        ENUM('ADMINISTRADOR','CAJERO') NOT NULL DEFAULT 'CAJERO',
    nombre     VARCHAR(100) NOT NULL,
    edad       INT          NOT NULL DEFAULT 18,
    sexo       VARCHAR(20)  NOT NULL DEFAULT 'Otro'
);

CREATE TABLE IF NOT EXISTS productos (
    id           VARCHAR(20)   PRIMARY KEY,
    nombre       VARCHAR(150)  NOT NULL,
    precio       DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    stock        DECIMAL(10,3) NOT NULL DEFAULT 0.000,
    stock_minimo DOUBLE        NOT NULL DEFAULT 5,
    categoria    VARCHAR(100)  NOT NULL,
    unidad       VARCHAR(30)   NOT NULL DEFAULT 'Piezas'
);

CREATE TABLE IF NOT EXISTS proveedores (
    id         INT AUTO_INCREMENT PRIMARY KEY,
    nombre     VARCHAR(150) NOT NULL,
    contacto   VARCHAR(100),
    telefono   VARCHAR(20),
    email      VARCHAR(100),
    direccion  VARCHAR(200),
    activo     TINYINT(1)   NOT NULL DEFAULT 1
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

CREATE TABLE configuracion (
    id INT PRIMARY KEY,
    nombre_tienda VARCHAR(255),
    sucursal VARCHAR(255),
    rfc VARCHAR(20)
);

ALTER TABLE productos 
ADD COLUMN imagen_ruta VARCHAR(500) DEFAULT '';

INSERT INTO configuracion (id, nombre_tienda, sucursal, rfc) 
VALUES (1, 'CORPORATIVO POS', 'Sucursal Principal - Centro', 'XAXX010101000');

INSERT IGNORE INTO usuarios (username, password, rol, nombre, edad, sexo) VALUES
('admin',   'admin123', 'ADMINISTRADOR', 'Director General', 45, 'Masculino'),
('cajero1', 'caja123',  'CAJERO',        'Juan Jose',     28, 'Masculino');

INSERT IGNORE INTO productos (id, nombre, precio, stock, categoria, unidad) VALUES
('001', 'Coca-Cola 600ml',       18.00, 50, 'Bebidas y Liquidos',            'Litros / mL'),
('002', 'Sabritas Original 45g', 14.00, 40, 'Botanas y Dulces',              'Kilogramos / g'),
('003', 'Arroz Verde Valle 1kg', 25.00, 60, 'Despensa Basica',               'Kilogramos / g'),
('004', 'Leche Lala 1L',         22.00, 30, 'Lacteos y Huevo',               'Litros / mL'),
('005', 'Atun Herdez 140g',      19.00, 30, 'Alimentos Preparados/Enlatados','Kilogramos / g');

INSERT IGNORE INTO proveedores (nombre, contacto, telefono, email, direccion, activo) VALUES
('GRUPO BIMBO',       'Juan Perez',   '8112345678', 'bimbo@proveedor.mx',  'Monterrey, NL', 1),
('COCA COLA FEMSA',   'Ana Torres',   '8187654321', 'femsa@proveedor.mx',  'Guadalupe, NL', 1),
('BARCEL S.A.',        'Carlos Ruiz',  '8199991111', 'barcel@proveedor.mx', 'CDMX',           1),
('LALA S.A. DE C.V.', 'Maria Lopez',  '8133334444', 'lala@proveedor.mx',   'Torreon, Coah', 1);