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
src/NeptunoApp/                     (proyecto de inicio, WinExe)
├── App.config                     → cadena de conexión (ConfigurationManager)
├── ViewModels/                    → MainViewModel + una VM por sección (CommunityToolkit.Mvvm)
├── Views/                         → un UserControl por sección
├── Converters/  Themes/Theme.xaml → estilos y convertidores de la interfaz
├── MainWindow.xaml               → barra lateral de navegación + ContentControl
└── App.xaml                      → merge del tema + DataTemplates ViewModel→View

src/NeptunoApp.Datos/                (Class Library, referenciada por NeptunoApp)
├── Data/
│   ├── I{Categoria,Proveedor,Producto,Pedido,Reporte,Catalogo}Repository.cs
│   ├── {…}Repository.cs           → ADO.NET asíncrono + CommandType.StoredProcedure
│   └── ReaderExtensions.cs        → helpers para columnas NULL
└── Models/                        → Categoria, Proveedor, Producto, Pedido, DetallePedido, OpcionCombo
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
2. Ajustar `Server=` en `src/NeptunoApp/App.config` (sección `connectionStrings`,
   entrada `NeptunoDB`) con la IP del servidor.
3. Desde el equipo Windows: comprobar la conectividad al puerto `1433`
   (`Test-NetConnection <IP> -Port 1433`).
4. Abrir `src/NeptunoApp.slnx` en Visual Studio 2022 y pulsar **F5** (o
   `dotnet run --project NeptunoApp/NeptunoApp.csproj`); el proyecto de inicio
   restaura la referencia a `NeptunoApp.Datos` automáticamente.

---

## 5. Explicación (ADO .NET Semana 05 / Class Library y DataSet Semana 06)

**`ExecuteNonQuery` en cada operación de escritura:**
Los cuatro repositorios (`CategoriaRepository`, `ProveedorRepository`,
`ProductoRepository`, `PedidoRepository`) usan `SqlCommand` con
`CommandType.StoredProcedure` para las tres operaciones de escritura:

- **Alta:** el procedimiento `usp_<Entidad>_Crear` recibe un parámetro de salida
  `@<Entidad>ID INT = NULL OUTPUT` y hace `SET @<Entidad>ID = SCOPE_IDENTITY()`
  en vez de `SELECT`. El repositorio agrega ese parámetro con
  `Direction = ParameterDirection.Output`, llama a `ExecuteNonQueryAsync()` y
  lee el Id generado desde `idParam.Value` (ver p. ej.
  `CategoriaRepository.CrearAsync`).
- **Edición:** `usp_<Entidad>_Actualizar` se invoca con `ExecuteNonQueryAsync()`;
  no hay result set que leer.
- **Baja:** `usp_<Entidad>_Eliminar` se invoca con `ExecuteNonQueryAsync()`.

**Cómo se resolvió la eliminación lógica:**
Se agregó la columna `Activo BIT NOT NULL DEFAULT 1` a `Categorias`,
`Proveedores`, `Productos` y `Pedidos` (`db/Migracion_ActivoLogico.sql`). Cada
procedimiento `_Eliminar` ya no ejecuta `DELETE`, sino
`UPDATE dbo.<Tabla> SET Activo = 0 WHERE <Tabla>ID = @Id` — el registro persiste
en la base de datos, solo cambia su estado. En el botón "Eliminar" de cada
vista WPF no cambió nada en el ViewModel: sigue llamando a
`{Entidad}Repository.EliminarAsync(id)`, y es el procedimiento almacenado el
que decide que la baja es lógica.

Esa baja se verifica en los listados y consultas agregando `WHERE Activo = 1`
(o `AND Activo = 1`) en:
- `usp_Categoria_ListarTodas`, `usp_Proveedor_ListarTodas`, `usp_Producto_ListarTodas`, `usp_Pedido_ListarTodas`.
- `usp_Proveedor_BuscarPorContactoCiudad` (búsqueda por contacto/ciudad).
- `usp_DetallePedido_ListarPorRangoFechas`, filtrando `ped.Activo = 1` sobre la
  tabla `Pedidos` con la que hace `INNER JOIN`, de modo que un pedido dado de
  baja lógicamente desaparece del reporte por rango de fechas aunque su detalle
  siga existiendo en `DetallePedidos`.

`db/PruebasProcedimientos.sql` ejercita este comportamiento: crea un registro,
lo elimina, comprueba con un `SELECT` directo que `Activo = 0` y la fila sigue
existiendo, y confirma que ya no aparece en el listado/búsqueda/reporte
correspondiente.

**Separación en Class Library (`NeptunoApp.Datos`):**
Los Modelos y todo el acceso a datos (`Data/`, `Models/`) viven en un proyecto
de biblioteca de clases aparte (`src/NeptunoApp.Datos`, `TargetFramework=net10.0`,
sin dependencia de WPF), y el proyecto WPF (`src/NeptunoApp`) lo referencia con
`<ProjectReference>`. Los namespaces (`NeptunoApp.Data`, `NeptunoApp.Models`) no
cambiaron: solo se movió su ubicación física/ensamblado, así que ViewModels y
Views siguen consumiéndolos igual. `MainWindow.xaml.cs` sigue siendo el único
lugar donde se instancian los repositorios concretos e inyectan en el
`MainViewModel` (constructor injection manual, sin contenedor de DI).

**Criterio para el modo desconectado:**
Las operaciones CRUD (alta, edición, baja) siguen siendo **conectadas**
(`SqlConnection` + `ExecuteNonQueryAsync`), porque cada cambio del usuario debe
confirmarse contra la base de datos de inmediato — no tiene sentido cachear una
edición o un alta localmente. En cambio, el **reporte de detalle de pedidos por
rango de fechas** (`ReporteRepository.DetallePedidosPorRangoFechasAsync`) es de
**solo lectura**: el usuario elige un rango de fechas, se trae todo el resultado
una sola vez y luego solo lo mira/ordena en pantalla, sin ninguna escritura de
vuelta a la BD. Por eso ese reporte usa `SqlDataAdapter.Fill(DataTable)` en vez
de `SqlDataReader`: el `DataAdapter` abre la conexión, llena el `DataTable` y la
cierra por su cuenta, y desde ahí la UI navega el `DataTable` completamente
desconectada del servidor (el `DataGrid` de `ReporteView.xaml` bindea
`ItemsSource` directo al `DataTable`, usando el indexador de `DataRowView`
—`{Binding [Columna]}`— para cada columna). Como `SqlDataAdapter.Fill` no tiene
una sobrecarga async, se ejecuta dentro de `Task.Run(...)` para no bloquear el
hilo de UI y mantener el async/await de punta a punta en toda la aplicación.

**El "gotcha" de `App.config`:**
Al mover el acceso a datos a la Class Library, la cadena de conexión no puede
vivir ahí: `ConfigurationManager` solo lee el `App.config` del **ensamblado de
inicio** (el `.exe`/`.dll` que realmente arranca), nunca el de una biblioteca
referenciada. Por eso la cadena de conexión está en
`src/NeptunoApp/App.config` (`<connectionStrings><add name="NeptunoDB" .../>`),
el proyecto WPF referencia el paquete `System.Configuration.ConfigurationManager`,
y `MainWindow.xaml.cs` la lee con
`ConfigurationManager.ConnectionStrings["NeptunoDB"].ConnectionString` antes de
construir los repositorios. `NeptunoApp.Datos` no sabe de dónde viene la cadena:
cada repositorio solo recibe un `string connectionString` por constructor.

**Bloqueos `.Result`/`.Wait()`:**
Se auditó todo el proyecto (`grep -rn "\.Result\b\|\.Wait("`) y no se encontró
ninguna llamada bloqueante sobre código async: todos los métodos de ViewModels
y repositorios ya son `async`/`await` de punta a punta (incluidos los comandos
de `CommunityToolkit.Mvvm`, que generan `async void`/`async Task` correctamente
para los eventos de los botones). No fue necesaria ninguna corrección en este
punto.

---

