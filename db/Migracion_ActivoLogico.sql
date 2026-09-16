/* ============================================================
   Migracion_ActivoLogico.sql  (ADO.NET Semana 05)

   Agrega el campo Activo BIT (default 1) a Productos, Categorias,
   Proveedores y Pedidos, para reemplazar el DELETE fisico por
   eliminacion logica (UPDATE Activo = 0).

   Requiere que NeptunoDB.sql ya se haya ejecutado. Es idempotente:
   se puede ejecutar varias veces sin error.

   Ejecutar:
     sqlcmd -S localhost -U sa -P 'TU_PASSWORD' -C -i db/Migracion_ActivoLogico.sql
   ============================================================ */

USE NeptunoDB;
GO

IF COL_LENGTH('dbo.Categorias', 'Activo') IS NULL
    ALTER TABLE dbo.Categorias ADD Activo BIT NOT NULL CONSTRAINT DF_Categorias_Activo DEFAULT (1);
GO

IF COL_LENGTH('dbo.Proveedores', 'Activo') IS NULL
    ALTER TABLE dbo.Proveedores ADD Activo BIT NOT NULL CONSTRAINT DF_Proveedores_Activo DEFAULT (1);
GO

IF COL_LENGTH('dbo.Productos', 'Activo') IS NULL
    ALTER TABLE dbo.Productos ADD Activo BIT NOT NULL CONSTRAINT DF_Productos_Activo DEFAULT (1);
GO

IF COL_LENGTH('dbo.Pedidos', 'Activo') IS NULL
    ALTER TABLE dbo.Pedidos ADD Activo BIT NOT NULL CONSTRAINT DF_Pedidos_Activo DEFAULT (1);
GO

PRINT 'Columna Activo agregada (o ya existente) en Categorias, Proveedores, Productos y Pedidos.';
GO
