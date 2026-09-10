/* ============================================================================
   NeptunoDB  -  Procedimientos Almacenados  (ADO .NET - Semana 04)

   Contenido:
     - CRUD de Categorias
     - CRUD de Proveedores
     - CRUD de Productos
     - CRUD de Pedidos (cabecera)
     - Listado de proveedores buscando por nombreContacto y ciudad
     - Listado de detalles de pedidos (INNER JOIN con Pedidos) filtrando
       por un intervalo de fechas

   Ejecutar despues de 01_NeptunoDB.sql:
     sqlcmd -S localhost -U sa -P 'TU_PASSWORD' -C -i db/02_Procedimientos.sql
   ============================================================================ */

USE NeptunoDB;
GO

/* ===========================================================================
   CRUD  -  CATEGORIAS
   =========================================================================== */

CREATE OR ALTER PROCEDURE dbo.usp_Categorias_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdCategoria, NombreCategoria, Descripcion
    FROM dbo.Categorias
    ORDER BY NombreCategoria;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Categorias_ObtenerPorId
    @IdCategoria INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdCategoria, NombreCategoria, Descripcion
    FROM dbo.Categorias
    WHERE IdCategoria = @IdCategoria;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Categorias_Insertar
    @NombreCategoria NVARCHAR(50),
    @Descripcion     NVARCHAR(400) = NULL,
    @IdCategoria     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Categorias (NombreCategoria, Descripcion)
    VALUES (@NombreCategoria, @Descripcion);

    SET @IdCategoria = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Categorias_Actualizar
    @IdCategoria     INT,
    @NombreCategoria NVARCHAR(50),
    @Descripcion     NVARCHAR(400) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Categorias
    SET NombreCategoria = @NombreCategoria,
        Descripcion     = @Descripcion
    WHERE IdCategoria = @IdCategoria;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Categorias_Eliminar
    @IdCategoria INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Productos WHERE IdCategoria = @IdCategoria)
    BEGIN
        RAISERROR('No se puede eliminar la categoria: tiene productos asociados.', 16, 1);
        RETURN;
    END

    DELETE FROM dbo.Categorias
    WHERE IdCategoria = @IdCategoria;
END
GO

/* ===========================================================================
   CRUD  -  PROVEEDORES
   =========================================================================== */

CREATE OR ALTER PROCEDURE dbo.usp_Proveedores_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdProveedor, NombreCompania, NombreContacto, CargoContacto,
           Direccion, Ciudad, Region, CodPostal, Pais, Telefono, Fax
    FROM dbo.Proveedores
    ORDER BY NombreCompania;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedores_ObtenerPorId
    @IdProveedor INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdProveedor, NombreCompania, NombreContacto, CargoContacto,
           Direccion, Ciudad, Region, CodPostal, Pais, Telefono, Fax
    FROM dbo.Proveedores
    WHERE IdProveedor = @IdProveedor;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedores_Insertar
    @NombreCompania NVARCHAR(60),
    @NombreContacto NVARCHAR(40) = NULL,
    @CargoContacto  NVARCHAR(40) = NULL,
    @Direccion      NVARCHAR(80) = NULL,
    @Ciudad         NVARCHAR(30) = NULL,
    @Region         NVARCHAR(30) = NULL,
    @CodPostal      NVARCHAR(15) = NULL,
    @Pais           NVARCHAR(30) = NULL,
    @Telefono       NVARCHAR(30) = NULL,
    @Fax            NVARCHAR(30) = NULL,
    @IdProveedor    INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Proveedores
        (NombreCompania, NombreContacto, CargoContacto, Direccion, Ciudad,
         Region, CodPostal, Pais, Telefono, Fax)
    VALUES
        (@NombreCompania, @NombreContacto, @CargoContacto, @Direccion, @Ciudad,
         @Region, @CodPostal, @Pais, @Telefono, @Fax);

    SET @IdProveedor = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedores_Actualizar
    @IdProveedor    INT,
    @NombreCompania NVARCHAR(60),
    @NombreContacto NVARCHAR(40) = NULL,
    @CargoContacto  NVARCHAR(40) = NULL,
    @Direccion      NVARCHAR(80) = NULL,
    @Ciudad         NVARCHAR(30) = NULL,
    @Region         NVARCHAR(30) = NULL,
    @CodPostal      NVARCHAR(15) = NULL,
    @Pais           NVARCHAR(30) = NULL,
    @Telefono       NVARCHAR(30) = NULL,
    @Fax            NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Proveedores
    SET NombreCompania = @NombreCompania,
        NombreContacto = @NombreContacto,
        CargoContacto  = @CargoContacto,
        Direccion      = @Direccion,
        Ciudad         = @Ciudad,
        Region         = @Region,
        CodPostal      = @CodPostal,
        Pais           = @Pais,
        Telefono       = @Telefono,
        Fax            = @Fax
    WHERE IdProveedor = @IdProveedor;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedores_Eliminar
    @IdProveedor INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Productos WHERE IdProveedor = @IdProveedor)
    BEGIN
        RAISERROR('No se puede eliminar el proveedor: tiene productos asociados.', 16, 1);
        RETURN;
    END

    DELETE FROM dbo.Proveedores
    WHERE IdProveedor = @IdProveedor;
END
GO

/* ---------------------------------------------------------------------------
   Listado de proveedores buscando por nombreContacto y ciudad
   (ambos parametros son opcionales: si van NULL o vacios no filtran)
   --------------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.usp_Proveedores_Buscar
    @NombreContacto NVARCHAR(40) = NULL,
    @Ciudad         NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdProveedor, NombreCompania, NombreContacto, CargoContacto,
           Direccion, Ciudad, Region, CodPostal, Pais, Telefono, Fax
    FROM dbo.Proveedores
    WHERE (@NombreContacto IS NULL OR @NombreContacto = ''
           OR NombreContacto LIKE '%' + @NombreContacto + '%')
      AND (@Ciudad IS NULL OR @Ciudad = ''
           OR Ciudad LIKE '%' + @Ciudad + '%')
    ORDER BY NombreContacto;
END
GO

/* ===========================================================================
   CRUD  -  PRODUCTOS
   =========================================================================== */

CREATE OR ALTER PROCEDURE dbo.usp_Productos_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.IdProducto, p.NombreProducto, p.IdProveedor, p.IdCategoria,
           pr.NombreCompania AS NombreProveedor,
           c.NombreCategoria AS NombreCategoria,
           p.CantidadPorUnidad, p.PrecioUnidad, p.UnidadesEnExistencia,
           p.UnidadesEnPedido, p.NivelNuevoPedido, p.Suspendido
    FROM dbo.Productos p
    LEFT JOIN dbo.Proveedores pr ON pr.IdProveedor = p.IdProveedor
    LEFT JOIN dbo.Categorias  c  ON c.IdCategoria  = p.IdCategoria
    ORDER BY p.NombreProducto;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Productos_ObtenerPorId
    @IdProducto INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdProducto, NombreProducto, IdProveedor, IdCategoria,
           CantidadPorUnidad, PrecioUnidad, UnidadesEnExistencia,
           UnidadesEnPedido, NivelNuevoPedido, Suspendido
    FROM dbo.Productos
    WHERE IdProducto = @IdProducto;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Productos_Insertar
    @NombreProducto       NVARCHAR(80),
    @IdProveedor          INT = NULL,
    @IdCategoria          INT = NULL,
    @CantidadPorUnidad    NVARCHAR(40) = NULL,
    @PrecioUnidad         MONEY = 0,
    @UnidadesEnExistencia SMALLINT = 0,
    @UnidadesEnPedido     SMALLINT = 0,
    @NivelNuevoPedido     SMALLINT = 0,
    @Suspendido           BIT = 0,
    @IdProducto           INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Productos
        (NombreProducto, IdProveedor, IdCategoria, CantidadPorUnidad, PrecioUnidad,
         UnidadesEnExistencia, UnidadesEnPedido, NivelNuevoPedido, Suspendido)
    VALUES
        (@NombreProducto, @IdProveedor, @IdCategoria, @CantidadPorUnidad, @PrecioUnidad,
         @UnidadesEnExistencia, @UnidadesEnPedido, @NivelNuevoPedido, @Suspendido);

    SET @IdProducto = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Productos_Actualizar
    @IdProducto           INT,
    @NombreProducto       NVARCHAR(80),
    @IdProveedor          INT = NULL,
    @IdCategoria          INT = NULL,
    @CantidadPorUnidad    NVARCHAR(40) = NULL,
    @PrecioUnidad         MONEY = 0,
    @UnidadesEnExistencia SMALLINT = 0,
    @UnidadesEnPedido     SMALLINT = 0,
    @NivelNuevoPedido     SMALLINT = 0,
    @Suspendido           BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Productos
    SET NombreProducto       = @NombreProducto,
        IdProveedor          = @IdProveedor,
        IdCategoria          = @IdCategoria,
        CantidadPorUnidad    = @CantidadPorUnidad,
        PrecioUnidad         = @PrecioUnidad,
        UnidadesEnExistencia = @UnidadesEnExistencia,
        UnidadesEnPedido     = @UnidadesEnPedido,
        NivelNuevoPedido     = @NivelNuevoPedido,
        Suspendido           = @Suspendido
    WHERE IdProducto = @IdProducto;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Productos_Eliminar
    @IdProducto INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.DetallesPedidos WHERE IdProducto = @IdProducto)
    BEGIN
        RAISERROR('No se puede eliminar el producto: aparece en pedidos.', 16, 1);
        RETURN;
    END

    DELETE FROM dbo.Productos
    WHERE IdProducto = @IdProducto;
END
GO

/* ===========================================================================
   CRUD  -  PEDIDOS  (cabecera)
   =========================================================================== */

CREATE OR ALTER PROCEDURE dbo.usp_Pedidos_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ped.IdPedido, ped.IdCliente, ped.IdEmpleado,
           cli.NombreCompania AS NombreCliente,
           emp.Nombre + ' ' + emp.Apellidos AS NombreEmpleado,
           ped.FechaPedido, ped.FechaEntrega, ped.FechaEnvio,
           ped.Flete, ped.Destinatario, ped.CiudadDestino, ped.PaisDestino,
           CAST(ISNULL((SELECT SUM(d.PrecioUnidad * d.Cantidad * (1 - d.Descuento))
                        FROM dbo.DetallesPedidos d
                        WHERE d.IdPedido = ped.IdPedido), 0) AS DECIMAL(12,2)) AS TotalPedido
    FROM dbo.Pedidos ped
    LEFT JOIN dbo.Clientes  cli ON cli.IdCliente  = ped.IdCliente
    LEFT JOIN dbo.Empleados emp ON emp.IdEmpleado = ped.IdEmpleado
    ORDER BY ped.FechaPedido DESC, ped.IdPedido DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pedidos_ObtenerPorId
    @IdPedido INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdPedido, IdCliente, IdEmpleado, FechaPedido, FechaEntrega, FechaEnvio,
           Flete, Destinatario, CiudadDestino, PaisDestino
    FROM dbo.Pedidos
    WHERE IdPedido = @IdPedido;

    -- Segundo result set: el detalle del pedido
    SELECT d.IdProducto, pr.NombreProducto, d.PrecioUnidad, d.Cantidad, d.Descuento,
           (d.PrecioUnidad * d.Cantidad * (1 - d.Descuento)) AS Subtotal
    FROM dbo.DetallesPedidos d
    INNER JOIN dbo.Productos pr ON pr.IdProducto = d.IdProducto
    WHERE d.IdPedido = @IdPedido
    ORDER BY pr.NombreProducto;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pedidos_Insertar
    @IdCliente     INT = NULL,
    @IdEmpleado    INT = NULL,
    @FechaPedido   DATETIME = NULL,
    @FechaEntrega  DATETIME = NULL,
    @FechaEnvio    DATETIME = NULL,
    @Flete         MONEY = 0,
    @Destinatario  NVARCHAR(60) = NULL,
    @CiudadDestino NVARCHAR(30) = NULL,
    @PaisDestino   NVARCHAR(30) = NULL,
    @IdPedido      INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @FechaPedido IS NULL SET @FechaPedido = CAST(GETDATE() AS DATE);

    INSERT INTO dbo.Pedidos
        (IdCliente, IdEmpleado, FechaPedido, FechaEntrega, FechaEnvio,
         Flete, Destinatario, CiudadDestino, PaisDestino)
    VALUES
        (@IdCliente, @IdEmpleado, @FechaPedido, @FechaEntrega, @FechaEnvio,
         @Flete, @Destinatario, @CiudadDestino, @PaisDestino);

    SET @IdPedido = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pedidos_Actualizar
    @IdPedido      INT,
    @IdCliente     INT = NULL,
    @IdEmpleado    INT = NULL,
    @FechaPedido   DATETIME = NULL,
    @FechaEntrega  DATETIME = NULL,
    @FechaEnvio    DATETIME = NULL,
    @Flete         MONEY = 0,
    @Destinatario  NVARCHAR(60) = NULL,
    @CiudadDestino NVARCHAR(30) = NULL,
    @PaisDestino   NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Pedidos
    SET IdCliente     = @IdCliente,
        IdEmpleado    = @IdEmpleado,
        FechaPedido   = @FechaPedido,
        FechaEntrega  = @FechaEntrega,
        FechaEnvio    = @FechaEnvio,
        Flete         = @Flete,
        Destinatario  = @Destinatario,
        CiudadDestino = @CiudadDestino,
        PaisDestino   = @PaisDestino
    WHERE IdPedido = @IdPedido;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pedidos_Eliminar
    @IdPedido INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.DetallesPedidos WHERE IdPedido = @IdPedido;
    DELETE FROM dbo.Pedidos         WHERE IdPedido = @IdPedido;
END
GO

/* ---------------------------------------------------------------------------
   Listado de detalles de pedidos haciendo un INNER JOIN con Pedidos,
   filtrando por un intervalo de fechas (FechaPedido entre @FechaInicio y @FechaFin)
   --------------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.usp_DetallesPedidos_PorRangoFechas
    @FechaInicio DATETIME,
    @FechaFin    DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ped.IdPedido,
           ped.FechaPedido,
           cli.NombreCompania AS NombreCliente,
           pr.NombreProducto,
           d.PrecioUnidad,
           d.Cantidad,
           d.Descuento,
           CAST(d.PrecioUnidad * d.Cantidad * (1 - d.Descuento) AS DECIMAL(12,2)) AS Subtotal
    FROM dbo.DetallesPedidos d
    INNER JOIN dbo.Pedidos    ped ON ped.IdPedido   = d.IdPedido
    INNER JOIN dbo.Productos  pr  ON pr.IdProducto  = d.IdProducto
    LEFT  JOIN dbo.Clientes   cli ON cli.IdCliente  = ped.IdCliente
    WHERE ped.FechaPedido >= @FechaInicio
      AND ped.FechaPedido <  DATEADD(DAY, 1, @FechaFin)
    ORDER BY ped.FechaPedido, ped.IdPedido, pr.NombreProducto;
END
GO

PRINT 'Procedimientos almacenados creados correctamente.';
GO
