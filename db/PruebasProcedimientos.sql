/* ============================================================
   PruebasProcedimientos.sql
   Ejercita TODOS los procedimientos almacenados de NeptunoDB y
   muestra el resultado, para verificar que funcionan.

   Semana 05: ademas de crear/leer/actualizar, verifica que
   usp_*_Eliminar hace baja LOGICA (Activo = 0, la fila sigue
   existiendo) y que los listados/busquedas la excluyen. Al final
   limpia los datos de prueba con DELETE directo (no via SP), solo
   para no dejar basura en la base de datos de demostracion.

   Ejecutar despues de NeptunoDB.sql, Migracion_ActivoLogico.sql
   y ProcedimientosAlmacenados.sql:
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
EXEC dbo.usp_Categoria_Crear @NombreCategoria = N'Cat prueba', @Descripcion = N'temporal',
                             @CategoriaID = @idCat OUTPUT;
PRINT '> Creada CategoriaID = ' + CAST(@idCat AS VARCHAR(10)) + ' (recibido por OUTPUT, sin SELECT)';
EXEC dbo.usp_Categoria_ObtenerPorId @idCat;
EXEC dbo.usp_Categoria_Actualizar @CategoriaID = @idCat, @NombreCategoria = N'Cat modificada', @Descripcion = NULL;
EXEC dbo.usp_Categoria_ObtenerPorId @idCat;
EXEC dbo.usp_Categoria_Eliminar @idCat;
PRINT '> Eliminada (logicamente). Verificacion Activo = 0, la fila sigue existiendo:';
SELECT CategoriaID, NombreCategoria, Activo FROM dbo.Categorias WHERE CategoriaID = @idCat;
PRINT '> Ya no aparece en usp_Categoria_ListarTodas (filtra Activo = 1):';
EXEC dbo.usp_Categoria_ListarTodas;
GO

/* -------------------- CRUD DE PROVEEDORES -------------------- */
PRINT '';
PRINT '======== CRUD DE PROVEEDORES ========';
DECLARE @idProv INT;
EXEC dbo.usp_Proveedor_Crear @CompaniaNombre = N'Proveedor Prueba SAC',
                             @NombreContacto = N'Contacto Prueba', @Ciudad = N'Lima', @Pais = N'Peru',
                             @ProveedorID = @idProv OUTPUT;
PRINT '> Creado ProveedorID = ' + CAST(@idProv AS VARCHAR(10)) + ' (recibido por OUTPUT, sin SELECT)';
EXEC dbo.usp_Proveedor_ObtenerPorId @idProv;
EXEC dbo.usp_Proveedor_Actualizar @ProveedorID = @idProv, @CompaniaNombre = N'Proveedor Prueba SAC',
                                  @NombreContacto = N'Contacto Prueba', @Ciudad = N'Arequipa', @Pais = N'Peru';
EXEC dbo.usp_Proveedor_ObtenerPorId @idProv;
EXEC dbo.usp_Proveedor_Eliminar @idProv;
PRINT '> Eliminado (logicamente). Verificacion Activo = 0, la fila sigue existiendo:';
SELECT ProveedorID, CompaniaNombre, Activo FROM dbo.Proveedores WHERE ProveedorID = @idProv;
GO

/* ----- LISTADO DE PROVEEDORES POR nombreContacto Y ciudad ----- */
PRINT '';
PRINT '======== usp_Proveedor_BuscarPorContactoCiudad (solo Activo = 1) ========';
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
                            @CantidadPorUnidad = N'1 unidad', @PrecioUnidad = 9.99, @UnidadesEnExistencia = 50,
                            @ProductoID = @idProd OUTPUT;
PRINT '> Creado ProductoID = ' + CAST(@idProd AS VARCHAR(10)) + ' (recibido por OUTPUT, sin SELECT)';
EXEC dbo.usp_Producto_ObtenerPorId @idProd;
EXEC dbo.usp_Producto_Actualizar @ProductoID = @idProd, @NombreProducto = N'Producto de prueba',
                                 @ProveedorID = 1, @CategoriaID = 1, @PrecioUnidad = 12.50,
                                 @UnidadesEnExistencia = 50, @Descontinuado = 1;
EXEC dbo.usp_Producto_ObtenerPorId @idProd;
EXEC dbo.usp_Producto_Eliminar @idProd;
PRINT '> Eliminado (logicamente). Verificacion Activo = 0, la fila sigue existiendo:';
SELECT ProductoID, NombreProducto, Activo FROM dbo.Productos WHERE ProductoID = @idProd;
PRINT '> Ya no aparece en usp_Producto_ListarTodas (filtra Activo = 1):';
EXEC dbo.usp_Producto_ListarTodas;
GO

/* -------------------- CRUD DE PEDIDOS -------------------- */
PRINT '';
PRINT '======== CRUD DE PEDIDOS ========';
DECLARE @idPed INT;
EXEC dbo.usp_Pedido_Crear @ClienteID = 1, @EmpleadoID = 2, @FechaPedido = '2026-09-01',
                          @TransportistaID = 1, @Destinatario = N'Cliente Prueba',
                          @CiudadDestino = N'Lima', @PaisDestino = N'Peru',
                          @PedidoID = @idPed OUTPUT;
PRINT '> Creado PedidoID = ' + CAST(@idPed AS VARCHAR(10)) + ' (recibido por OUTPUT, sin SELECT)';
EXEC dbo.usp_Pedido_ObtenerPorId @idPed;
EXEC dbo.usp_Pedido_Actualizar @PedidoID = @idPed, @ClienteID = 1, @EmpleadoID = 2,
                               @FechaPedido = '2026-09-01', @Destinatario = N'Cliente Prueba (mod)',
                               @CiudadDestino = N'Cusco', @PaisDestino = N'Peru';
EXEC dbo.usp_Pedido_Eliminar @idPed;
PRINT '> Eliminado (logicamente). Verificacion Activo = 0, la fila sigue existiendo:';
SELECT PedidoID, Destinatario, Activo FROM dbo.Pedidos WHERE PedidoID = @idPed;
PRINT '> Ya no aparece en usp_Pedido_ListarTodas (filtra Activo = 1):';
EXEC dbo.usp_Pedido_ListarTodas;
PRINT '> usp_Pedido_ObtenerPorId de un pedido real con detalle (PedidoID = 1):';
EXEC dbo.usp_Pedido_ObtenerPorId 1;
GO

/* ----- DETALLES DE PEDIDOS (INNER JOIN) POR RANGO DE FECHAS ----- */
PRINT '';
PRINT '======== usp_DetallePedido_ListarPorRangoFechas (excluye Pedidos.Activo = 0) ========';
PRINT '> Detalles con FechaPedido entre 2026-08-01 y 2026-08-31';
EXEC dbo.usp_DetallePedido_ListarPorRangoFechas @FechaInicio = '2026-08-01', @FechaFin = '2026-08-31';
PRINT '> Detalles con FechaPedido = 2026-09-01 (debe salir vacio: el pedido de prueba quedo Activo = 0)';
EXEC dbo.usp_DetallePedido_ListarPorRangoFechas @FechaInicio = '2026-09-01', @FechaFin = '2026-09-01';
GO

/* -------------------- LIMPIEZA DE DATOS DE PRUEBA -------------------- */
-- DELETE directo (no via SP): solo para no dejar basura de esta prueba en la BD.
-- No forma parte del flujo de la aplicacion, que siempre usa baja logica.
PRINT '';
PRINT '======== LIMPIEZA DE DATOS DE PRUEBA ========';
DELETE FROM dbo.Pedidos     WHERE Destinatario  = N'Cliente Prueba (mod)';
DELETE FROM dbo.Productos   WHERE NombreProducto = N'Producto de prueba';
DELETE FROM dbo.Proveedores WHERE CompaniaNombre = N'Proveedor Prueba SAC';
DELETE FROM dbo.Categorias  WHERE NombreCategoria = N'Cat modificada';
PRINT '> Datos de prueba eliminados fisicamente (limpieza de la demo).';
GO

PRINT '';
PRINT '*** PRUEBAS FINALIZADAS - todos los procedimientos se ejecutaron ***';
GO
