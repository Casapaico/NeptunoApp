/* ============================================================
   NeptunoDB - Procedimientos Almacenados

   Requiere que NeptunoDB.sql y Migracion_ActivoLogico.sql ya se
   hayan ejecutado (esta version depende de la columna Activo).
   Convencion:
     SET NOCOUNT ON;  BEGIN TRY / BEGIN CATCH THROW;
     Crear -> parametro @<Entidad>ID OUTPUT con SCOPE_IDENTITY()
              (permite invocar el alta con ExecuteNonQuery, Semana 05)
     Eliminar -> eliminacion logica (UPDATE Activo = 0), nunca DELETE fisico
     Listar/Buscar -> filtra WHERE Activo = 1

   Ejecutar:
     sqlcmd -S localhost -U sa -P 'TU_PASSWORD' -C -i db/Migracion_ActivoLogico.sql
     sqlcmd -S localhost -U sa -P 'TU_PASSWORD' -C -i db/ProcedimientosAlmacenados.sql
   ============================================================ */

USE NeptunoDB;
GO

/* ============================================================
   CRUD DE CATEGORIAS
   ============================================================ */

CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Crear
    @NombreCategoria NVARCHAR(30),
    @Descripcion     NVARCHAR(200) = NULL,
    @CategoriaID     INT = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.Categorias (NombreCategoria, Descripcion)
        VALUES (@NombreCategoria, @Descripcion);

        SET @CategoriaID = CAST(SCOPE_IDENTITY() AS INT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Categoria_ObtenerPorId
    @CategoriaID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CategoriaID, NombreCategoria, Descripcion
    FROM dbo.Categorias
    WHERE CategoriaID = @CategoriaID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Categoria_ListarTodas
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CategoriaID, NombreCategoria, Descripcion
    FROM dbo.Categorias
    WHERE Activo = 1
    ORDER BY NombreCategoria;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Actualizar
    @CategoriaID     INT,
    @NombreCategoria NVARCHAR(30),
    @Descripcion     NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE dbo.Categorias
        SET NombreCategoria = @NombreCategoria,
            Descripcion     = @Descripcion
        WHERE CategoriaID = @CategoriaID;

        IF @@ROWCOUNT = 0 THROW 51001, 'Categoria no encontrada.', 1;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- Eliminacion logica: nunca DELETE fisico, solo Activo = 0.
CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Eliminar
    @CategoriaID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE dbo.Categorias SET Activo = 0 WHERE CategoriaID = @CategoriaID;

        IF @@ROWCOUNT = 0 THROW 51003, 'Categoria no encontrada.', 1;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

/* ============================================================
   CRUD DE PROVEEDORES
   ============================================================ */

CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Crear
    @CompaniaNombre NVARCHAR(60),
    @NombreContacto NVARCHAR(40) = NULL,
    @CargoContacto  NVARCHAR(40) = NULL,
    @Direccion      NVARCHAR(80) = NULL,
    @Ciudad         NVARCHAR(30) = NULL,
    @CodigoPostal   NVARCHAR(10) = NULL,
    @Pais           NVARCHAR(30) = NULL,
    @Telefono       NVARCHAR(24) = NULL,
    @Fax            NVARCHAR(24) = NULL,
    @ProveedorID    INT = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.Proveedores
            (CompaniaNombre, NombreContacto, CargoContacto, Direccion, Ciudad,
             CodigoPostal, Pais, Telefono, Fax)
        VALUES
            (@CompaniaNombre, @NombreContacto, @CargoContacto, @Direccion, @Ciudad,
             @CodigoPostal, @Pais, @Telefono, @Fax);

        SET @ProveedorID = CAST(SCOPE_IDENTITY() AS INT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_ObtenerPorId
    @ProveedorID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProveedorID, CompaniaNombre, NombreContacto, CargoContacto, Direccion,
           Ciudad, CodigoPostal, Pais, Telefono, Fax
    FROM dbo.Proveedores
    WHERE ProveedorID = @ProveedorID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_ListarTodas
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProveedorID, CompaniaNombre, NombreContacto, CargoContacto, Direccion,
           Ciudad, CodigoPostal, Pais, Telefono, Fax
    FROM dbo.Proveedores
    WHERE Activo = 1
    ORDER BY CompaniaNombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Actualizar
    @ProveedorID    INT,
    @CompaniaNombre NVARCHAR(60),
    @NombreContacto NVARCHAR(40) = NULL,
    @CargoContacto  NVARCHAR(40) = NULL,
    @Direccion      NVARCHAR(80) = NULL,
    @Ciudad         NVARCHAR(30) = NULL,
    @CodigoPostal   NVARCHAR(10) = NULL,
    @Pais           NVARCHAR(30) = NULL,
    @Telefono       NVARCHAR(24) = NULL,
    @Fax            NVARCHAR(24) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE dbo.Proveedores
        SET CompaniaNombre = @CompaniaNombre,
            NombreContacto = @NombreContacto,
            CargoContacto  = @CargoContacto,
            Direccion      = @Direccion,
            Ciudad         = @Ciudad,
            CodigoPostal   = @CodigoPostal,
            Pais           = @Pais,
            Telefono       = @Telefono,
            Fax            = @Fax
        WHERE ProveedorID = @ProveedorID;

        IF @@ROWCOUNT = 0 THROW 52001, 'Proveedor no encontrado.', 1;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- Eliminacion logica: nunca DELETE fisico, solo Activo = 0.
CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Eliminar
    @ProveedorID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE dbo.Proveedores SET Activo = 0 WHERE ProveedorID = @ProveedorID;

        IF @@ROWCOUNT = 0 THROW 52003, 'Proveedor no encontrado.', 1;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- Listado de proveedores buscando por nombreContacto y ciudad (filtros opcionales).
-- Solo muestra proveedores activos (Activo = 1).
CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_BuscarPorContactoCiudad
    @NombreContacto NVARCHAR(40) = NULL,
    @Ciudad         NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProveedorID, CompaniaNombre, NombreContacto, CargoContacto, Direccion,
           Ciudad, CodigoPostal, Pais, Telefono, Fax
    FROM dbo.Proveedores
    WHERE Activo = 1
      AND (@NombreContacto IS NULL OR @NombreContacto = ''
           OR NombreContacto LIKE '%' + @NombreContacto + '%')
      AND (@Ciudad IS NULL OR @Ciudad = ''
           OR Ciudad LIKE '%' + @Ciudad + '%')
    ORDER BY NombreContacto;
END
GO

/* ============================================================
   CRUD DE PRODUCTOS
   ============================================================ */

CREATE OR ALTER PROCEDURE dbo.usp_Producto_Crear
    @NombreProducto       NVARCHAR(60),
    @ProveedorID          INT = NULL,
    @CategoriaID          INT = NULL,
    @CantidadPorUnidad    NVARCHAR(30) = NULL,
    @PrecioUnidad         DECIMAL(10,2) = 0,
    @UnidadesEnExistencia SMALLINT = 0,
    @UnidadesEnPedido     SMALLINT = 0,
    @NivelDeReorden       SMALLINT = 0,
    @Descontinuado        BIT = 0,
    @ProductoID           INT = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.Productos
            (NombreProducto, ProveedorID, CategoriaID, CantidadPorUnidad, PrecioUnidad,
             UnidadesEnExistencia, UnidadesEnPedido, NivelDeReorden, Descontinuado)
        VALUES
            (@NombreProducto, @ProveedorID, @CategoriaID, @CantidadPorUnidad, @PrecioUnidad,
             @UnidadesEnExistencia, @UnidadesEnPedido, @NivelDeReorden, @Descontinuado);

        SET @ProductoID = CAST(SCOPE_IDENTITY() AS INT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Producto_ObtenerPorId
    @ProductoID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProductoID, NombreProducto, ProveedorID, CategoriaID, CantidadPorUnidad,
           PrecioUnidad, UnidadesEnExistencia, UnidadesEnPedido, NivelDeReorden, Descontinuado
    FROM dbo.Productos
    WHERE ProductoID = @ProductoID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Producto_ListarTodas
AS
BEGIN
    SET NOCOUNT ON;
    SELECT  p.ProductoID, p.NombreProducto, p.ProveedorID, p.CategoriaID,
            pr.CompaniaNombre  AS Proveedor,
            c.NombreCategoria   AS Categoria,
            p.CantidadPorUnidad, p.PrecioUnidad, p.UnidadesEnExistencia,
            p.UnidadesEnPedido, p.NivelDeReorden, p.Descontinuado
    FROM dbo.Productos p
    LEFT JOIN dbo.Proveedores pr ON pr.ProveedorID = p.ProveedorID
    LEFT JOIN dbo.Categorias  c  ON c.CategoriaID  = p.CategoriaID
    WHERE p.Activo = 1
    ORDER BY p.NombreProducto;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Producto_Actualizar
    @ProductoID           INT,
    @NombreProducto       NVARCHAR(60),
    @ProveedorID          INT = NULL,
    @CategoriaID          INT = NULL,
    @CantidadPorUnidad    NVARCHAR(30) = NULL,
    @PrecioUnidad         DECIMAL(10,2) = 0,
    @UnidadesEnExistencia SMALLINT = 0,
    @UnidadesEnPedido     SMALLINT = 0,
    @NivelDeReorden       SMALLINT = 0,
    @Descontinuado        BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE dbo.Productos
        SET NombreProducto       = @NombreProducto,
            ProveedorID          = @ProveedorID,
            CategoriaID          = @CategoriaID,
            CantidadPorUnidad    = @CantidadPorUnidad,
            PrecioUnidad         = @PrecioUnidad,
            UnidadesEnExistencia = @UnidadesEnExistencia,
            UnidadesEnPedido     = @UnidadesEnPedido,
            NivelDeReorden       = @NivelDeReorden,
            Descontinuado        = @Descontinuado
        WHERE ProductoID = @ProductoID;

        IF @@ROWCOUNT = 0 THROW 53001, 'Producto no encontrado.', 1;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- Eliminacion logica: nunca DELETE fisico, solo Activo = 0.
CREATE OR ALTER PROCEDURE dbo.usp_Producto_Eliminar
    @ProductoID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE dbo.Productos SET Activo = 0 WHERE ProductoID = @ProductoID;

        IF @@ROWCOUNT = 0 THROW 53003, 'Producto no encontrado.', 1;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

/* ============================================================
   CRUD DE PEDIDOS  (cabecera)
   ============================================================ */

CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Crear
    @ClienteID       INT = NULL,
    @EmpleadoID      INT = NULL,
    @FechaPedido     DATE,
    @FechaRequerida  DATE = NULL,
    @FechaEnvio      DATE = NULL,
    @TransportistaID INT = NULL,
    @Destinatario    NVARCHAR(60) = NULL,
    @CiudadDestino   NVARCHAR(30) = NULL,
    @PaisDestino     NVARCHAR(30) = NULL,
    @PedidoID        INT = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.Pedidos
            (ClienteID, EmpleadoID, FechaPedido, FechaRequerida, FechaEnvio,
             TransportistaID, Destinatario, CiudadDestino, PaisDestino)
        VALUES
            (@ClienteID, @EmpleadoID, @FechaPedido, @FechaRequerida, @FechaEnvio,
             @TransportistaID, @Destinatario, @CiudadDestino, @PaisDestino);

        SET @PedidoID = CAST(SCOPE_IDENTITY() AS INT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pedido_ObtenerPorId
    @PedidoID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Result set 1: cabecera
    SELECT PedidoID, ClienteID, EmpleadoID, FechaPedido, FechaRequerida, FechaEnvio,
           TransportistaID, Destinatario, CiudadDestino, PaisDestino
    FROM dbo.Pedidos
    WHERE PedidoID = @PedidoID;

    -- Result set 2: detalle del pedido
    SELECT d.ProductoID, pr.NombreProducto, d.PrecioUnidad, d.Cantidad, d.Descuento,
           CAST(d.PrecioUnidad * d.Cantidad * (1 - d.Descuento) AS DECIMAL(12,2)) AS Subtotal
    FROM dbo.DetallePedidos d
    INNER JOIN dbo.Productos pr ON pr.ProductoID = d.ProductoID
    WHERE d.PedidoID = @PedidoID
    ORDER BY pr.NombreProducto;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pedido_ListarTodas
AS
BEGIN
    SET NOCOUNT ON;
    SELECT  p.PedidoID, p.ClienteID, p.EmpleadoID, p.TransportistaID,
            cl.Empresa                       AS Cliente,
            (e.Nombre + ' ' + e.Apellidos)   AS Empleado,
            p.FechaPedido, p.FechaRequerida, p.FechaEnvio,
            p.Destinatario, p.CiudadDestino, p.PaisDestino,
            CAST(ISNULL((SELECT SUM(d.PrecioUnidad * d.Cantidad * (1 - d.Descuento))
                         FROM dbo.DetallePedidos d
                         WHERE d.PedidoID = p.PedidoID), 0) AS DECIMAL(12,2)) AS TotalPedido
    FROM dbo.Pedidos p
    LEFT JOIN dbo.Clientes  cl ON cl.ClienteID  = p.ClienteID
    LEFT JOIN dbo.Empleados e  ON e.EmpleadoID  = p.EmpleadoID
    WHERE p.Activo = 1
    ORDER BY p.FechaPedido DESC, p.PedidoID DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Actualizar
    @PedidoID        INT,
    @ClienteID       INT = NULL,
    @EmpleadoID      INT = NULL,
    @FechaPedido     DATE,
    @FechaRequerida  DATE = NULL,
    @FechaEnvio      DATE = NULL,
    @TransportistaID INT = NULL,
    @Destinatario    NVARCHAR(60) = NULL,
    @CiudadDestino   NVARCHAR(30) = NULL,
    @PaisDestino     NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE dbo.Pedidos
        SET ClienteID       = @ClienteID,
            EmpleadoID      = @EmpleadoID,
            FechaPedido     = @FechaPedido,
            FechaRequerida  = @FechaRequerida,
            FechaEnvio      = @FechaEnvio,
            TransportistaID = @TransportistaID,
            Destinatario    = @Destinatario,
            CiudadDestino   = @CiudadDestino,
            PaisDestino     = @PaisDestino
        WHERE PedidoID = @PedidoID;

        IF @@ROWCOUNT = 0 THROW 54001, 'Pedido no encontrado.', 1;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- Eliminacion logica: nunca DELETE fisico, solo Activo = 0 (el detalle se conserva).
CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Eliminar
    @PedidoID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE dbo.Pedidos SET Activo = 0 WHERE PedidoID = @PedidoID;

        IF @@ROWCOUNT = 0 THROW 54002, 'Pedido no encontrado.', 1;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

/* ============================================================
   REPORTE: detalles de pedidos (INNER JOIN con Pedidos)
            filtrando por un intervalo de fechas
   ============================================================ */

CREATE OR ALTER PROCEDURE dbo.usp_DetallePedido_ListarPorRangoFechas
    @FechaInicio DATE,
    @FechaFin    DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT  ped.PedidoID,
            ped.FechaPedido,
            cl.Empresa            AS Cliente,
            pr.NombreProducto     AS Producto,
            d.PrecioUnidad,
            d.Cantidad,
            d.Descuento,
            CAST(d.PrecioUnidad * d.Cantidad * (1 - d.Descuento) AS DECIMAL(12,2)) AS Subtotal
    FROM dbo.DetallePedidos d
    INNER JOIN dbo.Pedidos    ped ON ped.PedidoID   = d.PedidoID
    INNER JOIN dbo.Productos  pr  ON pr.ProductoID  = d.ProductoID
    LEFT  JOIN dbo.Clientes   cl  ON cl.ClienteID   = ped.ClienteID
    WHERE ped.Activo = 1
      AND ped.FechaPedido BETWEEN @FechaInicio AND @FechaFin
    ORDER BY ped.FechaPedido, ped.PedidoID, pr.NombreProducto;
END
GO

/* ============================================================
   Consultas de apoyo para ComboBox (catalogos)
   ============================================================ */

CREATE OR ALTER PROCEDURE dbo.usp_Catalogo_Categorias
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CategoriaID AS Id, NombreCategoria AS Texto
    FROM dbo.Categorias ORDER BY NombreCategoria;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Catalogo_Proveedores
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProveedorID AS Id, CompaniaNombre AS Texto
    FROM dbo.Proveedores ORDER BY CompaniaNombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Catalogo_Clientes
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ClienteID AS Id, Empresa AS Texto
    FROM dbo.Clientes ORDER BY Empresa;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Catalogo_Empleados
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EmpleadoID AS Id, (Nombre + ' ' + Apellidos) AS Texto
    FROM dbo.Empleados ORDER BY Apellidos;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Catalogo_Transportistas
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TransportistaID AS Id, CompaniaNombre AS Texto
    FROM dbo.Transportistas ORDER BY CompaniaNombre;
END
GO

PRINT 'Procedimientos almacenados creados correctamente.';
GO
