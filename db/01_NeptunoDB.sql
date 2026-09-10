/* ============================================================================
   NeptunoDB  -  Base de datos para el desafio "ADO .NET - Semana 04"
   Curso: Desarrollo de Aplicaciones Empresariales Avanzado (Tecsup)
   Docente: Arevalo Sermeno, Edwin William

   Version reducida de la base "Neptuno" (traduccion al espanol de Northwind).
   El script es RE-EJECUTABLE: elimina y vuelve a crear la base completa.

   Ejecutar desde la laptop Ubuntu donde corre SQL Server 2022:
     sqlcmd -S localhost -U sa -P 'TU_PASSWORD' -C -i db/01_NeptunoDB.sql
   ============================================================================ */

USE master;
GO

IF DB_ID('NeptunoDB') IS NOT NULL
BEGIN
    ALTER DATABASE NeptunoDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE NeptunoDB;
END
GO

CREATE DATABASE NeptunoDB;
GO

USE NeptunoDB;
GO

/* ---------------------------------------------------------------------------
   1. TABLAS
   --------------------------------------------------------------------------- */

CREATE TABLE dbo.Categorias
(
    IdCategoria     INT IDENTITY(1,1) NOT NULL,
    NombreCategoria NVARCHAR(50)      NOT NULL,
    Descripcion     NVARCHAR(400)     NULL,
    CONSTRAINT PK_Categorias PRIMARY KEY (IdCategoria)
);
GO

CREATE TABLE dbo.Proveedores
(
    IdProveedor     INT IDENTITY(1,1) NOT NULL,
    NombreCompania  NVARCHAR(60)      NOT NULL,
    NombreContacto  NVARCHAR(40)      NULL,
    CargoContacto   NVARCHAR(40)      NULL,
    Direccion       NVARCHAR(80)      NULL,
    Ciudad          NVARCHAR(30)      NULL,
    Region          NVARCHAR(30)      NULL,
    CodPostal       NVARCHAR(15)      NULL,
    Pais            NVARCHAR(30)      NULL,
    Telefono        NVARCHAR(30)      NULL,
    Fax             NVARCHAR(30)      NULL,
    CONSTRAINT PK_Proveedores PRIMARY KEY (IdProveedor)
);
GO

CREATE TABLE dbo.Productos
(
    IdProducto           INT IDENTITY(1,1) NOT NULL,
    NombreProducto       NVARCHAR(80)      NOT NULL,
    IdProveedor          INT               NULL,
    IdCategoria          INT               NULL,
    CantidadPorUnidad    NVARCHAR(40)      NULL,
    PrecioUnidad         MONEY             NOT NULL CONSTRAINT DF_Productos_Precio    DEFAULT (0),
    UnidadesEnExistencia SMALLINT          NOT NULL CONSTRAINT DF_Productos_Exist     DEFAULT (0),
    UnidadesEnPedido     SMALLINT          NOT NULL CONSTRAINT DF_Productos_Pedido    DEFAULT (0),
    NivelNuevoPedido     SMALLINT          NOT NULL CONSTRAINT DF_Productos_Nivel     DEFAULT (0),
    Suspendido           BIT               NOT NULL CONSTRAINT DF_Productos_Suspend   DEFAULT (0),
    CONSTRAINT PK_Productos PRIMARY KEY (IdProducto),
    CONSTRAINT FK_Productos_Proveedores FOREIGN KEY (IdProveedor) REFERENCES dbo.Proveedores (IdProveedor),
    CONSTRAINT FK_Productos_Categorias  FOREIGN KEY (IdCategoria) REFERENCES dbo.Categorias  (IdCategoria)
);
GO

CREATE TABLE dbo.Clientes
(
    IdCliente      INT IDENTITY(1,1) NOT NULL,
    NombreCompania NVARCHAR(60)      NOT NULL,
    NombreContacto NVARCHAR(40)      NULL,
    Ciudad         NVARCHAR(30)      NULL,
    Pais           NVARCHAR(30)      NULL,
    Telefono       NVARCHAR(30)      NULL,
    CONSTRAINT PK_Clientes PRIMARY KEY (IdCliente)
);
GO

CREATE TABLE dbo.Empleados
(
    IdEmpleado INT IDENTITY(1,1) NOT NULL,
    Apellidos  NVARCHAR(30)      NOT NULL,
    Nombre     NVARCHAR(20)      NOT NULL,
    Cargo      NVARCHAR(40)      NULL,
    CONSTRAINT PK_Empleados PRIMARY KEY (IdEmpleado)
);
GO

CREATE TABLE dbo.Pedidos
(
    IdPedido      INT IDENTITY(1,1) NOT NULL,
    IdCliente     INT               NULL,
    IdEmpleado    INT               NULL,
    FechaPedido   DATETIME          NULL,
    FechaEntrega  DATETIME          NULL,
    FechaEnvio    DATETIME          NULL,
    Flete         MONEY             NOT NULL CONSTRAINT DF_Pedidos_Flete DEFAULT (0),
    Destinatario  NVARCHAR(60)      NULL,
    CiudadDestino NVARCHAR(30)      NULL,
    PaisDestino   NVARCHAR(30)      NULL,
    CONSTRAINT PK_Pedidos PRIMARY KEY (IdPedido),
    CONSTRAINT FK_Pedidos_Clientes  FOREIGN KEY (IdCliente)  REFERENCES dbo.Clientes  (IdCliente),
    CONSTRAINT FK_Pedidos_Empleados FOREIGN KEY (IdEmpleado) REFERENCES dbo.Empleados (IdEmpleado)
);
GO

CREATE TABLE dbo.DetallesPedidos
(
    IdPedido     INT      NOT NULL,
    IdProducto   INT      NOT NULL,
    PrecioUnidad MONEY    NOT NULL CONSTRAINT DF_Detalles_Precio   DEFAULT (0),
    Cantidad     SMALLINT NOT NULL CONSTRAINT DF_Detalles_Cantidad DEFAULT (1),
    Descuento    REAL     NOT NULL CONSTRAINT DF_Detalles_Descuento DEFAULT (0),
    CONSTRAINT PK_DetallesPedidos PRIMARY KEY (IdPedido, IdProducto),
    CONSTRAINT FK_Detalles_Pedidos   FOREIGN KEY (IdPedido)   REFERENCES dbo.Pedidos   (IdPedido),
    CONSTRAINT FK_Detalles_Productos FOREIGN KEY (IdProducto) REFERENCES dbo.Productos (IdProducto)
);
GO

/* ---------------------------------------------------------------------------
   2. DATOS DE PRUEBA
   --------------------------------------------------------------------------- */

SET IDENTITY_INSERT dbo.Categorias ON;
INSERT INTO dbo.Categorias (IdCategoria, NombreCategoria, Descripcion) VALUES
 (1, N'Bebidas',          N'Gaseosas, cafes, cervezas y tes'),
 (2, N'Condimentos',      N'Salsas, aderezos y especias'),
 (3, N'Reposteria',       N'Postres, dulces y panes'),
 (4, N'Lacteos',          N'Quesos, leches y derivados'),
 (5, N'Granos y Cereales',N'Harinas, arroz y pastas'),
 (6, N'Carnes',           N'Carnes rojas y aves preparadas'),
 (7, N'Frutas y Verduras',N'Productos frescos y desecados'),
 (8, N'Pescados/Mariscos',N'Pescados y mariscos');
SET IDENTITY_INSERT dbo.Categorias OFF;
GO

SET IDENTITY_INSERT dbo.Proveedores ON;
INSERT INTO dbo.Proveedores (IdProveedor, NombreCompania, NombreContacto, CargoContacto, Direccion, Ciudad, Region, CodPostal, Pais, Telefono, Fax) VALUES
 (1,  N'Alimentos del Sur SAC',      N'Carlos Ramirez',   N'Gerente de Ventas',      N'Av. Los Incas 123',    N'Lima',      N'Lima',      N'15001', N'Peru',      N'01-4567890', N'01-4567891'),
 (2,  N'Distribuidora Andina EIRL',  N'Maria Quispe',     N'Representante Comercial', N'Jr. Cusco 456',        N'Arequipa',  N'Arequipa',  N'04001', N'Peru',      N'054-223344', NULL),
 (3,  N'Lacteos La Campina SA',      N'Jorge Mendoza',    N'Jefe de Logistica',      N'Carretera Central 789',N'Huancayo',  N'Junin',     N'12001', N'Peru',      N'064-556677', N'064-556678'),
 (4,  N'Bebidas Tropicales SAC',     N'Ana Torres',       N'Ejecutiva de Cuentas',   N'Av. Grau 321',         N'Piura',     N'Piura',     N'20001', N'Peru',      N'073-334455', NULL),
 (5,  N'Molinos del Peru SA',        N'Luis Fernandez',   N'Gerente Comercial',      N'Av. Argentina 1000',   N'Lima',      N'Lima',      N'15082', N'Peru',      N'01-3345566', N'01-3345567'),
 (6,  N'Import Foods Global',        N'Patricia Sanchez', N'Gerente de Ventas',      N'Calle 50 #22',         N'Ciudad de Panama', NULL,  N'0801',  N'Panama',    N'507-2334455', NULL),
 (7,  N'Comercial El Pacifico Ltda', N'Roberto Diaz',     N'Vendedor Senior',        N'Av. Providencia 234',  N'Santiago',  N'RM',        N'7500000',N'Chile',     N'56-2-9988776', NULL),
 (8,  N'Frutas Selectas SAC',        N'Carmen Rojas',     N'Representante Comercial', N'Mercado Mayorista S/N',N'Lima',      N'Lima',      N'15021', N'Peru',      N'01-2211334', NULL),
 (9,  N'Pesquera Costa Azul SA',     N'Miguel Castro',    N'Jefe de Ventas',         N'Muelle 5, Zona Sur',   N'Callao',    N'Callao',    N'07001', N'Peru',      N'01-4998877', N'01-4998878'),
 (10, N'Especias del Mundo EIRL',    N'Lucia Herrera',    N'Ejecutiva Comercial',    N'Jr. Union 555',        N'Trujillo',  N'La Libertad',N'13001',N'Peru',      N'044-667788', NULL);
SET IDENTITY_INSERT dbo.Proveedores OFF;
GO

SET IDENTITY_INSERT dbo.Productos ON;
INSERT INTO dbo.Productos (IdProducto, NombreProducto, IdProveedor, IdCategoria, CantidadPorUnidad, PrecioUnidad, UnidadesEnExistencia, UnidadesEnPedido, NivelNuevoPedido, Suspendido) VALUES
 (1,  N'Cafe tostado molido 500g',      1,  1, N'24 bolsas x caja',   28.50, 120, 0,  25, 0),
 (2,  N'Te verde en filtros',           6,  1, N'50 sobres x caja',    9.90, 300, 0,  50, 0),
 (3,  N'Gaseosa cola 1.5L',             4,  1, N'12 botellas x paq',   6.50, 480, 60, 100,0),
 (4,  N'Cerveza rubia 355ml',           4,  1, N'24 latas x caja',     4.20, 200, 0,  40, 0),
 (5,  N'Salsa de aji picante 200ml',    2,  2, N'12 frascos x caja',   5.75, 90,  0,  20, 0),
 (6,  N'Aderezo cesar 250ml',           2,  2, N'12 frascos x caja',   8.30, 60,  0,  15, 0),
 (7,  N'Pimienta negra molida 100g',    10, 2, N'20 frascos x caja',   7.10, 75,  0,  20, 0),
 (8,  N'Canela en polvo 80g',           10, 2, N'20 frascos x caja',   6.40, 40,  10, 15, 0),
 (9,  N'Torta de chocolate congelada',  3,  3, N'6 unidades x caja',   22.00, 18,  6,  10, 0),
 (10, N'Galletas surtidas 400g',        1,  3, N'12 paquetes x caja',  9.80, 130, 0,  30, 0),
 (11, N'Pan de molde integral',         5,  3, N'1 unidad',            5.20, 45,  0,  15, 0),
 (12, N'Queso fresco 500g',             3,  4, N'1 unidad',            12.90, 55,  0,  20, 0),
 (13, N'Queso parmesano en trozo 250g', 3,  4, N'1 unidad',            18.50, 30,  0,  10, 0),
 (14, N'Leche entera 1L',               3,  4, N'12 cajas x paq',      4.10, 240, 0,  60, 0),
 (15, N'Yogur natural 1kg',             3,  4, N'1 unidad',            7.60, 70,  0,  20, 0),
 (16, N'Harina de trigo 1kg',           5,  5, N'10 bolsas x paq',     3.80, 320, 0,  80, 0),
 (17, N'Arroz extra 5kg',               5,  5, N'1 bolsa',             21.00, 150, 0,  40, 0),
 (18, N'Fideos spaghetti 500g',         5,  5, N'20 paquetes x caja',  2.90, 260, 0,  60, 0),
 (19, N'Avena en hojuelas 900g',        6,  5, N'12 bolsas x caja',    8.75, 95,  0,  25, 0),
 (20, N'Pechuga de pollo congelada 1kg',6,  6, N'1 unidad',            13.40, 80,  20, 30, 0),
 (21, N'Lomo fino de res 1kg',          6,  6, N'1 unidad',            34.90, 25,  0,  10, 1),
 (22, N'Chorizo parrillero 500g',       6,  6, N'1 unidad',            11.20, 60,  0,  20, 0),
 (23, N'Manzana roja x kg',             8,  7, N'1 kg',                4.50, 180, 0,  50, 0),
 (24, N'Palta fuerte x kg',             8,  7, N'1 kg',                7.80, 90,  0,  25, 0),
 (25, N'Zanahoria x kg',                8,  7, N'1 kg',                2.30, 200, 0,  60, 0),
 (26, N'Uva sin pepa x kg',             8,  7, N'1 kg',                9.50, 40,  0,  15, 1),
 (27, N'Filete de trucha 500g',         9,  8, N'1 unidad',            16.70, 50,  0,  20, 0),
 (28, N'Conchas de abanico 500g',       9,  8, N'1 unidad',            24.00, 22,  0,  10, 0),
 (29, N'Langostinos pelados 400g',      9,  8, N'1 unidad',            28.90, 15,  8,  10, 0),
 (30, N'Atun en conserva 170g',         2,  8, N'48 latas x caja',     3.60, 400, 0,  100,0);
SET IDENTITY_INSERT dbo.Productos OFF;
GO

SET IDENTITY_INSERT dbo.Clientes ON;
INSERT INTO dbo.Clientes (IdCliente, NombreCompania, NombreContacto, Ciudad, Pais, Telefono) VALUES
 (1, N'Supermercados Metro',     N'Elena Vargas',   N'Lima',     N'Peru',   N'01-6112233'),
 (2, N'Bodega Don Pepe',         N'Jose Palomino',  N'Lima',     N'Peru',   N'01-4455667'),
 (3, N'Minimarket La Esquina',   N'Rosa Chavez',    N'Arequipa', N'Peru',   N'054-778899'),
 (4, N'Restaurante El Fogon',    N'Andres Loayza',  N'Cusco',    N'Peru',   N'084-334455'),
 (5, N'Cafeteria Aroma',         N'Diana Reyes',    N'Trujillo', N'Peru',   N'044-221100'),
 (6, N'Distribuidora Norte SAC', N'Hugo Salazar',   N'Piura',    N'Peru',   N'073-556644'),
 (7, N'Hotel Miramar',           N'Paola Guzman',   N'Ica',      N'Peru',   N'056-889977'),
 (8, N'Comercial El Sol',        N'Victor Nunez',   N'Tacna',    N'Peru',   N'052-445566');
SET IDENTITY_INSERT dbo.Clientes OFF;
GO

SET IDENTITY_INSERT dbo.Empleados ON;
INSERT INTO dbo.Empleados (IdEmpleado, Apellidos, Nombre, Cargo) VALUES
 (1, N'Gonzales',  N'Pedro',   N'Representante de Ventas'),
 (2, N'Flores',    N'Sofia',   N'Representante de Ventas'),
 (3, N'Rivera',    N'Marcos',  N'Gerente de Ventas'),
 (4, N'Campos',    N'Lucia',   N'Coordinadora de Pedidos'),
 (5, N'Delgado',   N'Raul',    N'Representante de Ventas');
SET IDENTITY_INSERT dbo.Empleados OFF;
GO

SET IDENTITY_INSERT dbo.Pedidos ON;
INSERT INTO dbo.Pedidos (IdPedido, IdCliente, IdEmpleado, FechaPedido, FechaEntrega, FechaEnvio, Flete, Destinatario, CiudadDestino, PaisDestino) VALUES
 (1,  1, 1, '2026-01-08', '2026-01-15', '2026-01-10', 35.00, N'Supermercados Metro',   N'Lima',     N'Peru'),
 (2,  2, 2, '2026-01-12', '2026-01-19', '2026-01-14', 18.50, N'Bodega Don Pepe',       N'Lima',     N'Peru'),
 (3,  3, 1, '2026-01-20', '2026-01-27', '2026-01-23', 42.00, N'Minimarket La Esquina', N'Arequipa', N'Peru'),
 (4,  4, 3, '2026-02-03', '2026-02-10', '2026-02-05', 55.75, N'Restaurante El Fogon',  N'Cusco',    N'Peru'),
 (5,  5, 2, '2026-02-11', '2026-02-18', NULL,         12.00, N'Cafeteria Aroma',       N'Trujillo', N'Peru'),
 (6,  1, 4, '2026-02-19', '2026-02-26', '2026-02-21', 30.00, N'Supermercados Metro',   N'Lima',     N'Peru'),
 (7,  6, 5, '2026-03-02', '2026-03-09', '2026-03-04', 47.20, N'Distribuidora Norte SAC',N'Piura',   N'Peru'),
 (8,  7, 1, '2026-03-10', '2026-03-17', '2026-03-12', 26.90, N'Hotel Miramar',         N'Ica',      N'Peru'),
 (9,  8, 3, '2026-03-18', '2026-03-25', NULL,         33.40, N'Comercial El Sol',      N'Tacna',    N'Peru'),
 (10, 2, 2, '2026-03-24', '2026-03-31', '2026-03-26', 15.00, N'Bodega Don Pepe',       N'Lima',     N'Peru'),
 (11, 3, 4, '2026-04-05', '2026-04-12', '2026-04-08', 40.10, N'Minimarket La Esquina', N'Arequipa', N'Peru'),
 (12, 4, 5, '2026-04-14', '2026-04-21', '2026-04-16', 51.30, N'Restaurante El Fogon',  N'Cusco',    N'Peru');
SET IDENTITY_INSERT dbo.Pedidos OFF;
GO

INSERT INTO dbo.DetallesPedidos (IdPedido, IdProducto, PrecioUnidad, Cantidad, Descuento) VALUES
 (1,  1,  28.50, 10, 0.00),
 (1,  10,  9.80, 20, 0.05),
 (1,  14,  4.10, 48, 0.00),
 (2,  3,   6.50, 24, 0.00),
 (2,  30,  3.60, 48, 0.10),
 (3,  12, 12.90, 15, 0.00),
 (3,  13, 18.50,  6, 0.00),
 (3,  15,  7.60, 12, 0.05),
 (4,  20, 13.40, 20, 0.00),
 (4,  21, 34.90,  8, 0.00),
 (4,  22, 11.20, 15, 0.05),
 (5,  1,  28.50,  6, 0.00),
 (5,  2,   9.90, 12, 0.00),
 (6,  16,  3.80, 40, 0.10),
 (6,  17, 21.00, 10, 0.00),
 (6,  18,  2.90, 30, 0.00),
 (7,  27, 16.70, 12, 0.00),
 (7,  28, 24.00,  6, 0.00),
 (7,  29, 28.90,  5, 0.05),
 (8,  4,   4.20, 48, 0.00),
 (8,  3,   6.50, 36, 0.10),
 (9,  23,  4.50, 30, 0.00),
 (9,  24,  7.80, 15, 0.00),
 (9,  25,  2.30, 40, 0.00),
 (10, 30,  3.60, 96, 0.15),
 (10, 5,   5.75, 24, 0.00),
 (11, 9,  22.00,  6, 0.00),
 (11, 11,  5.20, 20, 0.05),
 (12, 19,  8.75, 12, 0.00),
 (12, 7,   7.10, 20, 0.00),
 (12, 8,   6.40, 15, 0.10);
GO

PRINT 'NeptunoDB creada correctamente.';
GO
