

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'corporativo_pos')
    CREATE DATABASE corporativo_pos
        COLLATE Latin1_General_CI_AI;
GO

USE corporativo_pos;
GO
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='usuarios' AND xtype='U')
CREATE TABLE usuarios (
    id       INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50)  NOT NULL UNIQUE,
    password VARCHAR(100) NOT NULL,
    rol      VARCHAR(20)  NOT NULL DEFAULT 'CAJERO' CHECK (rol IN ('ADMINISTRADOR','CAJERO')),
    nombre   VARCHAR(100) NOT NULL,
    edad     INT          NOT NULL DEFAULT 18,
    sexo     VARCHAR(20)  NOT NULL DEFAULT 'Otro'
);
GO
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='productos' AND xtype='U')
CREATE TABLE productos (
    id           VARCHAR(20)   PRIMARY KEY,
    nombre       VARCHAR(150)  NOT NULL,
    precio       DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    stock        DECIMAL(10,3) NOT NULL DEFAULT 0.000,
    stock_minimo FLOAT         NOT NULL DEFAULT 5,
    categoria    VARCHAR(100)  NOT NULL,
    unidad       VARCHAR(30)   NOT NULL DEFAULT 'Piezas',
    imagen_ruta  VARCHAR(500)           DEFAULT ''
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='proveedores' AND xtype='U')
CREATE TABLE proveedores (
    id        INT IDENTITY(1,1) PRIMARY KEY,
    nombre    VARCHAR(150) NOT NULL,
    contacto  VARCHAR(100),
    telefono  VARCHAR(20),
    email     VARCHAR(100),
    direccion VARCHAR(200),
    activo    BIT          NOT NULL DEFAULT 1
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ventas' AND xtype='U')
CREATE TABLE ventas (
    id          INT IDENTITY(1,1) PRIMARY KEY,
    total       DECIMAL(10,2) NOT NULL,
    metodo_pago VARCHAR(30)   NOT NULL DEFAULT 'Efectivo',
    fecha       DATETIME      NOT NULL DEFAULT GETDATE(),
    cajero      VARCHAR(100)  NOT NULL
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='venta_items' AND xtype='U')
CREATE TABLE venta_items (
    id          INT IDENTITY(1,1) PRIMARY KEY,
    venta_id    INT           NOT NULL,
    producto_id VARCHAR(20)   NOT NULL,
    cantidad    DECIMAL(10,3) NOT NULL,
    precio_unit DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (venta_id)    REFERENCES ventas(id)    ON DELETE CASCADE,
    FOREIGN KEY (producto_id) REFERENCES productos(id)
);
GO
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='movimientos' AND xtype='U')
CREATE TABLE movimientos (
    id          INT IDENTITY(1,1) PRIMARY KEY,
    tipo        VARCHAR(20)   NOT NULL CHECK (tipo IN ('VENTA','RETIRO','FONDO_INICIAL')),
    descripcion VARCHAR(200)  NOT NULL,
    monto       DECIMAL(10,2) NOT NULL,
    fecha       DATETIME      NOT NULL DEFAULT GETDATE(),
    usuario     VARCHAR(100)  NOT NULL
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ordenes_compra' AND xtype='U')
CREATE TABLE ordenes_compra (
    id           INT IDENTITY(1,1) PRIMARY KEY,
    proveedor_id INT           NOT NULL,
    total        DECIMAL(10,2) NOT NULL,
    tipo_pago    VARCHAR(10)   NOT NULL DEFAULT 'CONTADO' CHECK (tipo_pago IN ('CONTADO','CREDITO')),
    estado       VARCHAR(10)   NOT NULL DEFAULT 'PENDIENTE' CHECK (estado IN ('PENDIENTE','RECIBIDO')),
    fecha        DATETIME      NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (proveedor_id) REFERENCES proveedores(id)
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='orden_items' AND xtype='U')
CREATE TABLE orden_items (
    id                  INT IDENTITY(1,1) PRIMARY KEY,
    orden_id            INT           NOT NULL,
    producto_id         VARCHAR(20)   NOT NULL,
    cantidad_solicitada DECIMAL(10,3) NOT NULL,
    precio_costo        DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (orden_id)    REFERENCES ordenes_compra(id) ON DELETE CASCADE,
    FOREIGN KEY (producto_id) REFERENCES productos(id)
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='cuentas_por_pagar' AND xtype='U')
CREATE TABLE cuentas_por_pagar (
    id           INT IDENTITY(1,1) PRIMARY KEY,
    proveedor_id INT           NOT NULL,
    orden_id     INT           NOT NULL,
    monto_total  DECIMAL(10,2) NOT NULL,
    pagado       DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    saldo        DECIMAL(10,2) NOT NULL,
    vencimiento  VARCHAR(50)   NOT NULL DEFAULT '7 dias',
    estado       VARCHAR(10)   NOT NULL DEFAULT 'PENDIENTE' CHECK (estado IN ('PENDIENTE','PARCIAL','PAGADO')),
    FOREIGN KEY (proveedor_id) REFERENCES proveedores(id),
    FOREIGN KEY (orden_id)     REFERENCES ordenes_compra(id)
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='devoluciones' AND xtype='U')
CREATE TABLE devoluciones (
    id          INT IDENTITY(1,1) PRIMARY KEY,
    venta_id    INT          NOT NULL,
    producto_id VARCHAR(20)  NOT NULL,
    cantidad    FLOAT        NOT NULL,
    motivo      VARCHAR(255),
    fecha       DATETIME     DEFAULT GETDATE(),
    cajero      VARCHAR(100),
    FOREIGN KEY (venta_id)    REFERENCES ventas(id),
    FOREIGN KEY (producto_id) REFERENCES productos(id)
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='configuracion' AND xtype='U')
CREATE TABLE configuracion (
    id            INT          PRIMARY KEY,
    nombre_tienda VARCHAR(255),
    sucursal      VARCHAR(255),
    rfc           VARCHAR(20)
);
GO

IF NOT EXISTS (SELECT 1 FROM configuracion WHERE id = 1)
    INSERT INTO configuracion (id, nombre_tienda, sucursal, rfc)
    VALUES (1, 'CORPORATIVO POS', 'Sucursal Principal - Centro', 'XAXX010101000');
GO

IF NOT EXISTS (SELECT 1 FROM usuarios WHERE username = 'admin')
    INSERT INTO usuarios (username, password, rol, nombre, edad, sexo)
    VALUES ('admin', 'admin123', 'ADMINISTRADOR', 'Director General', 45, 'Masculino');
GO

IF NOT EXISTS (SELECT 1 FROM usuarios WHERE username = 'cajero1')
    INSERT INTO usuarios (username, password, rol, nombre, edad, sexo)
    VALUES ('cajero1', 'caja123', 'CAJERO', 'Juan Jose', 28, 'Masculino');
GO

IF NOT EXISTS (SELECT 1 FROM productos WHERE id = '001')
    INSERT INTO productos (id, nombre, precio, stock, categoria, unidad)
    VALUES ('001', 'Coca-Cola 600ml', 18.00, 50, 'Bebidas y Liquidos', 'Litros / mL');
GO
IF NOT EXISTS (SELECT 1 FROM productos WHERE id = '002')
    INSERT INTO productos (id, nombre, precio, stock, categoria, unidad)
    VALUES ('002', 'Sabritas Original 45g', 14.00, 40, 'Botanas y Dulces', 'Kilogramos / g');
GO
IF NOT EXISTS (SELECT 1 FROM productos WHERE id = '003')
    INSERT INTO productos (id, nombre, precio, stock, categoria, unidad)
    VALUES ('003', 'Arroz Verde Valle 1kg', 25.00, 60, 'Despensa Basica', 'Kilogramos / g');
GO
IF NOT EXISTS (SELECT 1 FROM productos WHERE id = '004')
    INSERT INTO productos (id, nombre, precio, stock, categoria, unidad)
    VALUES ('004', 'Leche Lala 1L', 22.00, 30, 'Lacteos y Huevo', 'Litros / mL');
GO
IF NOT EXISTS (SELECT 1 FROM productos WHERE id = '005')
    INSERT INTO productos (id, nombre, precio, stock, categoria, unidad)
    VALUES ('005', 'Atun Herdez 140g', 19.00, 30, 'Alimentos Preparados/Enlatados', 'Kilogramos / g');
GO

IF NOT EXISTS (SELECT 1 FROM proveedores WHERE nombre = 'GRUPO BIMBO')
    INSERT INTO proveedores (nombre, contacto, telefono, email, direccion, activo)
    VALUES ('GRUPO BIMBO', 'Juan Perez', '8112345678', 'bimbo@proveedor.mx', 'Monterrey, NL', 1);
GO
IF NOT EXISTS (SELECT 1 FROM proveedores WHERE nombre = 'COCA COLA FEMSA')
    INSERT INTO proveedores (nombre, contacto, telefono, email, direccion, activo)
    VALUES ('COCA COLA FEMSA', 'Ana Torres', '8187654321', 'femsa@proveedor.mx', 'Guadalupe, NL', 1);
GO
IF NOT EXISTS (SELECT 1 FROM proveedores WHERE nombre = 'BARCEL S.A.')
    INSERT INTO proveedores (nombre, contacto, telefono, email, direccion, activo)
    VALUES ('BARCEL S.A.', 'Carlos Ruiz', '8199991111', 'barcel@proveedor.mx', 'CDMX', 1);
GO
IF NOT EXISTS (SELECT 1 FROM proveedores WHERE nombre = 'LALA S.A. DE C.V.')
    INSERT INTO proveedores (nombre, contacto, telefono, email, direccion, activo)
    VALUES ('LALA S.A. DE C.V.', 'Maria Lopez', '8133334444', 'lala@proveedor.mx', 'Torreon, Coah', 1);
GO
