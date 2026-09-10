/* ============================================================
   PruebasProcedimientos.sql
   Ejercita TODOS los procedimientos almacenados de NeptunoDB y
   muestra el resultado, para verificar que funcionan.
   Lo que inserta lo elimina al final (no deja basura).

   Ejecutar despues de NeptunoDB.sql y ProcedimientosAlmacenados.sql:
     sqlcmd -S localhost -U sa -P 'TU_PASSWORD' -C -W -i db/PruebasProcedimientos.sql
   ============================================================ */
USE NeptunoDB;
SET NOCOUNT ON;
GO

PRINT '======== VERIFICACION: TABLAS Y PROCEDIMIENTOS ========';
SELECT t.name AS Tabla, SUM(p.rows) AS Filas
FROM sys.tables t
JOIN sys.partitions p ON p.object_id = t.object_id AND p.index_id IN (0,1)
GROUP BY t.name ORDER BY t.name;

SELECT name AS Procedimiento FROM sys.procedures ORDER BY name;
GO

/* -------------------- CRUD DE CATEGORIAS -------------------- */
PRINT '';
PRINT '======== CRUD DE CATEGORIAS ========';
DECLARE @idCat INT;
EXEC dbo.usp_Categoria_Crear @NombreCategoria = N'Cat prueba', @Descripcion = N'temporal';
SELECT @idCat = MAX(CategoriaID) FROM dbo.Categorias;
PRINT '> Creada CategoriaID = ' + CAST(@idCat AS VARCHAR(10));
EXEC dbo.usp_Categoria_ObtenerPorId @idCat;
EXEC dbo.usp_Categoria_Actualizar @CategoriaID = @idCat, @NombreCategoria = N'Cat modificada', @Descripcion = NULL;
EXEC dbo.usp_Categoria_ObtenerPorId @idCat;
EXEC dbo.usp_Categoria_Eliminar @idCat;
PRINT '> Eliminada. Listado final:';
EXEC dbo.usp_Categoria_ListarTodas;
GO

/* -------------------- CRUD DE PROVEEDORES -------------------- */
PRINT '';
PRINT '======== CRUD DE PROVEEDORES ========';
DECLARE @idProv INT;
EXEC dbo.usp_Proveedor_Crear @CompaniaNombre = N'Proveedor Prueba SAC',
                             @NombreContacto = N'Contacto Prueba', @Ciudad = N'Lima', @Pais = N'Peru';
SELECT @idProv = MAX(ProveedorID) FROM dbo.Proveedores;
PRINT '> Creado ProveedorID = ' + CAST(@idProv AS VARCHAR(10));
EXEC dbo.usp_Proveedor_ObtenerPorId @idProv;
EXEC dbo.usp_Proveedor_Actualizar @ProveedorID = @idProv, @CompaniaNombre = N'Proveedor Prueba SAC',
                                  @NombreContacto = N'Contacto Prueba', @Ciudad = N'Arequipa', @Pais = N'Peru';
EXEC dbo.usp_Proveedor_ObtenerPorId @idProv;
EXEC dbo.usp_Proveedor_Eliminar @idProv;
PRINT '> Eliminado.';
GO

/* ----- LISTADO DE PROVEEDORES POR nombreContacto Y ciudad ----- */
PRINT '';
PRINT '======== usp_Proveedor_BuscarPorContactoCiudad ========';
PRINT '> Por ciudad = Lima';
EXEC dbo.usp_Proveedor_BuscarPorContactoCiudad @Ciudad = N'Lima';
PRINT '> Por contacto que contiene ''ar'' y ciudad ''Lima''';
EXEC dbo.usp_Proveedor_BuscarPorContactoCiudad @NombreContacto = N'ar', @Ciudad = N'Lima';
GO

/* -------------------- CRUD DE PRODUCTOS -------------------- */
PRINT '';
PRINT '======== CRUD DE PRODUCTOS ========';
DECLARE @idProd INT;
EXEC dbo.usp_Producto_Crear @NombreProducto = N'Producto de prueba', @ProveedorID = 1, @CategoriaID = 1,
                            @CantidadPorUnidad = N'1 unidad', @PrecioUnidad = 9.99, @UnidadesEnExistencia = 50;
SELECT @idProd = MAX(ProductoID) FROM dbo.Productos;
PRINT '> Creado ProductoID = ' + CAST(@idProd AS VARCHAR(10));
EXEC dbo.usp_Producto_ObtenerPorId @idProd;
EXEC dbo.usp_Producto_Actualizar @ProductoID = @idProd, @NombreProducto = N'Producto de prueba',
                                 @ProveedorID = 1, @CategoriaID = 1, @PrecioUnidad = 12.50,
                                 @UnidadesEnExistencia = 50, @Descontinuado = 1;
EXEC dbo.usp_Producto_ObtenerPorId @idProd;
EXEC dbo.usp_Producto_Eliminar @idProd;
PRINT '> Eliminado. Listado (con JOIN a categoria y proveedor):';
EXEC dbo.usp_Producto_ListarTodas;
GO

/* -------------------- CRUD DE PEDIDOS -------------------- */
PRINT '';
PRINT '======== CRUD DE PEDIDOS ========';
DECLARE @idPed INT;
EXEC dbo.usp_Pedido_Crear @ClienteID = 1, @EmpleadoID = 2, @FechaPedido = '2026-09-01',
                          @TransportistaID = 1, @Destinatario = N'Cliente Prueba',
                          @CiudadDestino = N'Lima', @PaisDestino = N'Peru';
SELECT @idPed = MAX(PedidoID) FROM dbo.Pedidos;
PRINT '> Creado PedidoID = ' + CAST(@idPed AS VARCHAR(10));
EXEC dbo.usp_Pedido_ObtenerPorId @idPed;
EXEC dbo.usp_Pedido_Actualizar @PedidoID = @idPed, @ClienteID = 1, @EmpleadoID = 2,
                               @FechaPedido = '2026-09-01', @Destinatario = N'Cliente Prueba (mod)',
                               @CiudadDestino = N'Cusco', @PaisDestino = N'Peru';
EXEC dbo.usp_Pedido_Eliminar @idPed;
PRINT '> Eliminado. Listado final (con total por pedido):';
EXEC dbo.usp_Pedido_ListarTodas;
PRINT '> usp_Pedido_ObtenerPorId de un pedido real con detalle (PedidoID = 1):';
EXEC dbo.usp_Pedido_ObtenerPorId 1;
GO

/* ----- DETALLES DE PEDIDOS (INNER JOIN) POR RANGO DE FECHAS ----- */
PRINT '';
PRINT '======== usp_DetallePedido_ListarPorRangoFechas ========';
PRINT '> Detalles con FechaPedido entre 2026-08-01 y 2026-08-31';
EXEC dbo.usp_DetallePedido_ListarPorRangoFechas @FechaInicio = '2026-08-01', @FechaFin = '2026-08-31';
GO

PRINT '';
PRINT '*** PRUEBAS FINALIZADAS - todos los procedimientos se ejecutaron ***';
GO
