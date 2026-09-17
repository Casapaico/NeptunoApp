# Neptuno

Sistema de escritorio para administrar un catálogo comercial (categorías,
proveedores, productos y pedidos), con **eliminación lógica** (campo `Activo`).
Aplicación **WPF (.NET 10)** con patrón **MVVM** y acceso a datos mediante
**ADO.NET** y **procedimientos almacenados** sobre **SQL Server**.

- MVVM con **CommunityToolkit.Mvvm** (`ObservableObject`, `[ObservableProperty]`, `[RelayCommand]`).
- Acceso a datos **asíncrono** con `Microsoft.Data.SqlClient` y `CommandType.StoredProcedure`.
- Toda operación de escritura (alta, edición, baja) se ejecuta con `ExecuteNonQueryAsync`.
- Baja **lógica**: eliminar nunca hace `DELETE` físico, solo `UPDATE Activo = 0`.
- Un repositorio por entidad detrás de una interfaz (`I{Entidad}Repository`).
- Cadena de conexión centralizada en `Data/DbConfig.cs`.

---

## 1. Arquitectura de despliegue

La base de datos y la aplicación pueden ejecutarse en máquinas distintas dentro
de la misma red local. En el entorno de desarrollo actual, SQL Server corre en un
equipo y la aplicación WPF en otro (Windows), ambos conectados a la misma red
institucional ("Comunidad de innovadores").

```
   Equipo Windows (.NET 10 / VS 2022)        Servidor de BD
   ┌──────────────────┐    red local       ┌──────────────────────┐
   │  NeptunoApp.exe  │ ───────────────▶   │   SQL Server 2022    │
   │  (WPF + ADO.NET) │   10.200.x.x/21    │   10.200.171.203:1433│
   └──────────────────┘                    │   BD: NeptunoDB      │
                                           └──────────────────────┘
```

| Parámetro | Valor actual |
|---|---|
| Servidor SQL | `10.200.171.203`, puerto TCP `1433`, usuario `sa` |
| Base de datos | `NeptunoDB` |
| Red | Institucional "Comunidad de innovadores" (ambos equipos en la misma red) |

La IP del servidor se configura en `src/NeptunoApp/Data/DbConfig.cs` (`Server=`);
si la IP de la laptop servidor cambia (DHCP), actualizar ese valor. Comprobación
rápida desde el equipo Windows:
`Test-NetConnection 10.200.171.203 -Port 1433` debe devolver `TcpTestSucceeded : True`.

---

## 2. Base de datos (`db/`)

Ejecutar en el servidor SQL, en este orden:

| Script | Contenido |
|---|---|
| [`db/NeptunoDB.sql`](db/NeptunoDB.sql) | Crea la base `NeptunoDB` con sus 8 tablas (`Categorias`, `Proveedores`, `Clientes`, `Empleados`, `Transportistas`, `Productos`, `Pedidos`, `DetallePedidos`), sus claves foráneas y datos base. |
| [`db/Migracion_ActivoLogico.sql`](db/Migracion_ActivoLogico.sql) | Agrega la columna `Activo BIT DEFAULT 1` a `Categorias`, `Proveedores`, `Productos` y `Pedidos` (Semana 05, idempotente). |
| [`db/ProcedimientosAlmacenados.sql`](db/ProcedimientosAlmacenados.sql) | Crea los procedimientos almacenados (`CREATE OR ALTER`). |
| [`db/PruebasProcedimientos.sql`](db/PruebasProcedimientos.sql) | Ejercita todos los procedimientos, incluida la baja lógica, y muestra el resultado (verificación). |

```bash
sqlcmd -S localhost -U sa -P 'TU_PASSWORD' -C -i db/NeptunoDB.sql
sqlcmd -S localhost -U sa -P 'TU_PASSWORD' -C -i db/Migracion_ActivoLogico.sql
sqlcmd -S localhost -U sa -P 'TU_PASSWORD' -C -i db/ProcedimientosAlmacenados.sql
sqlcmd -S localhost -U sa -P 'TU_PASSWORD' -C -W -i db/PruebasProcedimientos.sql
```

### Procedimientos almacenados

| Procedimiento(s) | Función |
|---|---|
| `usp_Categoria_Crear / _ObtenerPorId / _ListarTodas / _Actualizar / _Eliminar` | CRUD de categorías; `_ListarTodas` filtra `Activo = 1`, `_Eliminar` hace baja lógica |
| `usp_Proveedor_Crear / _ObtenerPorId / _ListarTodas / _Actualizar / _Eliminar` | CRUD de proveedores; `_ListarTodas` filtra `Activo = 1`, `_Eliminar` hace baja lógica |
| `usp_Proveedor_BuscarPorContactoCiudad` | Listado de proveedores filtrado por `NombreContacto` y `Ciudad`, solo `Activo = 1` |
| `usp_Producto_Crear / _ObtenerPorId / _ListarTodas / _Actualizar / _Eliminar` | CRUD de productos; `_ListarTodas` filtra `Activo = 1`, `_Eliminar` hace baja lógica |
| `usp_Pedido_Crear / _ObtenerPorId / _ListarTodas / _Actualizar / _Eliminar` | CRUD de pedidos; `_ObtenerPorId` devuelve dos result sets (cabecera + detalle); `_ListarTodas` filtra `Activo = 1`, `_Eliminar` hace baja lógica (conserva el detalle) |
| `usp_DetallePedido_ListarPorRangoFechas` | Detalles de pedidos con `INNER JOIN` a `Pedidos`, filtrados por un intervalo de fechas; excluye pedidos con `Activo = 0` |
| `usp_Catalogo_*` | Consultas de apoyo para poblar los ComboBox |

Convención de los procedimientos: `SET NOCOUNT ON`, bloques `BEGIN TRY … BEGIN CATCH THROW`
para propagar los errores. El alta (`_Crear`) devuelve el Id generado por un
parámetro `@<Entidad>ID INT = NULL OUTPUT` con `SCOPE_IDENTITY()` (no `SELECT`),
para poder invocarse desde ADO.NET con `ExecuteNonQuery`. El borrado (`_Eliminar`)
nunca hace `DELETE` físico: siempre `UPDATE ... SET Activo = 0`.

---

## 3. Aplicación WPF (`src/`)

Solución [`src/NeptunoApp.slnx`](src/NeptunoApp.slnx) — objetivo `net10.0-windows`.

```powershell
cd src
dotnet run --project NeptunoApp\NeptunoApp.csproj      # o abrir NeptunoApp.slnx en Visual Studio y pulsar F5
```

En Windows también está el script [`abrir.ps1`](abrir.ps1) (comprueba la red y ejecuta).

### Estructura

```
src/NeptunoApp/
├── Data/
│   ├── DbConfig.cs                → cadena de conexión al servidor SQL
│   ├── I{Categoria,Proveedor,Producto,Pedido,Reporte,Catalogo}Repository.cs
│   ├── {…}Repository.cs           → ADO.NET asíncrono + CommandType.StoredProcedure
│   └── ReaderExtensions.cs        → helpers para columnas NULL
├── Models/                        → Categoria, Proveedor, Producto, Pedido, DetallePedido, OpcionCombo, LineaReporte
├── ViewModels/                    → MainViewModel + una VM por sección (CommunityToolkit.Mvvm)
├── Views/                         → un UserControl por sección
├── Converters/  Themes/Theme.xaml → estilos y convertidores de la interfaz
├── MainWindow.xaml               → barra lateral de navegación + ContentControl
└── App.xaml                      → merge del tema + DataTemplates ViewModel→View
```

### Pantallas

| Vista | Función |
|---|---|
| **Productos** | Mantenimiento de productos (CRUD) con selección de categoría y proveedor |
| **Categorías** | Mantenimiento de categorías (CRUD) |
| **Proveedores** | Mantenimiento de proveedores (CRUD) y búsqueda por contacto y ciudad |
| **Pedidos** | Mantenimiento de pedidos (cabecera) y detalle del pedido seleccionado |
| **Detalle de pedidos por fecha** | Reporte filtrado por rango de fechas |

---

## 4. Puesta en marcha

1. Servidor SQL activo y accesible en la red local; ejecutar los scripts de `db/`
   en orden: `NeptunoDB.sql` → `Migracion_ActivoLogico.sql` → `ProcedimientosAlmacenados.sql`.
2. Ajustar `Server=` en `src/NeptunoApp/Data/DbConfig.cs` con la IP del servidor.
3. Desde el equipo Windows: comprobar la conectividad al puerto `1433`
   (`Test-NetConnection <IP> -Port 1433`).
4. Abrir `src/NeptunoApp.slnx` en Visual Studio 2022 y pulsar **F5** (o `dotnet run`).

---

