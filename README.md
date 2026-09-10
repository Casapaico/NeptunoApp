# Neptuno — ADO .NET Semana 04 (WPF + MVVM + Stored Procedures)

Desafío del curso **Desarrollo de Aplicaciones Empresariales Avanzado** (Tecsup).
Docente: *Arévalo Sermeño, Edwin William*.

Aplicación de escritorio **WPF (.NET 10)** construida siguiendo la plantilla
[`EdwinArevalo/WPFMMVMSP`](https://github.com/EdwinArevalo/WPFMMVMSP):

- **MVVM** con **CommunityToolkit.Mvvm** (`[ObservableProperty]`, `[RelayCommand]`, `ObservableObject`).
- Acceso a datos con **ADO.NET** (`Microsoft.Data.SqlClient`) **asíncrono**,
  consumiendo **procedimientos almacenados** (`CommandType.StoredProcedure`).
- Un repositorio por entidad detrás de una interfaz (`I{Entidad}Repository`).
- Cadena de conexión en `Data/DbConfig.cs` (igual que la plantilla).
- `Converters/` y `Themes/Theme.xaml` tomados de la plantilla.

---

## 1. Arquitectura de red

La base de datos vive en la **laptop** (Ubuntu + SQL Server) y la aplicación .NET
se ejecuta en la **PC Windows**. Ambas se conectan por la WiFi **"iPhone de Alumno"**.

```
   PC Windows (VS 2022 / .NET 10)            Laptop Ubuntu
   ┌──────────────────┐   WiFi "iPhone     ┌────────────────────┐
   │  NeptunoApp.exe  │   de Alumno"       │  SQL Server 2022   │
   │  (WPF + ADO.NET) │ ───────────────▶   │  172.20.10.4:1433  │
   └──────────────────┘   172.20.10.0/28   │  BD: NeptunoDB     │
                                           └────────────────────┘
```

| Parámetro | Valor |
|---|---|
| Red WiFi | iPhone de Alumno (`172.20.10.0/28`, gateway `172.20.10.1`) |
| Laptop / SQL Server | `172.20.10.4`, puerto TCP `1433`, usuario `sa` |
| Base de datos | `NeptunoDB` |

En la PC Windows: `ipconfig` debe mostrar una IP `172.20.10.x`; probar con
`Test-NetConnection 172.20.10.4 -Port 1433`. Si la IP de la laptop cambia
(`hostname -I` en Ubuntu), actualizar `Server=` en `src/NeptunoApp/Data/DbConfig.cs`.

---

## 2. Base de datos (carpeta `db/`)

Ejecutar **en la laptop**, en orden:

| Script | Contenido |
|---|---|
| [`db/NeptunoDB.sql`](db/NeptunoDB.sql) | Archivo **NeptunoDB** provisto en el desafío: crea `NeptunoDB` con las 8 tablas (`Categorias`, `Proveedores`, `Clientes`, `Empleados`, `Transportistas`, `Productos`, `Pedidos`, `DetallePedidos`) y datos base. *(Los acentos de los datos de ejemplo se normalizaron a ASCII porque el archivo original venía con la codificación dañada.)* |
| [`db/ProcedimientosAlmacenados.sql`](db/ProcedimientosAlmacenados.sql) | Los procedimientos almacenados (`CREATE OR ALTER`). |
| [`db/PruebasProcedimientos.sql`](db/PruebasProcedimientos.sql) | Script opcional que ejercita **todos** los procedimientos y muestra el resultado (evidencia). |

```bash
sqlcmd -S localhost -U sa -P 'TU_PASSWORD' -C -i db/NeptunoDB.sql
sqlcmd -S localhost -U sa -P 'TU_PASSWORD' -C -i db/ProcedimientosAlmacenados.sql
sqlcmd -S localhost -U sa -P 'TU_PASSWORD' -C -W -i db/PruebasProcedimientos.sql
```

### Procedimientos almacenados

| Procedimiento(s) | Requisito del enunciado |
|---|---|
| `usp_Categoria_Crear / _ObtenerPorId / _ListarTodas / _Actualizar / _Eliminar` | CRUD de categorías |
| `usp_Proveedor_Crear / _ObtenerPorId / _ListarTodas / _Actualizar / _Eliminar` | CRUD de proveedores |
| `usp_Proveedor_BuscarPorContactoCiudad` | **Listado de proveedores buscando por `nombreContacto` y `ciudad`** |
| `usp_Producto_Crear / _ObtenerPorId / _ListarTodas / _Actualizar / _Eliminar` | CRUD de productos |
| `usp_Pedido_Crear / _ObtenerPorId / _ListarTodas / _Actualizar / _Eliminar` | CRUD de pedidos (cabecera; `_ObtenerPorId` devuelve 2 result sets: cabecera + detalle) |
| `usp_DetallePedido_ListarPorRangoFechas` | **Listado de detalles de pedidos con `INNER JOIN` a `Pedidos`, filtrando por un intervalo de fechas** |
| `usp_Catalogo_*` | Consultas de apoyo para poblar los ComboBox |

---

## 3. Aplicación WPF (`src/`)

Solución [`src/NeptunoApp.slnx`](src/NeptunoApp.slnx) — `net10.0-windows`.

```powershell
cd src
dotnet run --project NeptunoApp\NeptunoApp.csproj      # o abrir NeptunoApp.slnx en Visual Studio y pulsar F5
```

En Windows también funciona el script [`abrir.ps1`](abrir.ps1) (comprueba la red y ejecuta).

### Estructura

```
src/NeptunoApp/
├── Data/
│   ├── DbConfig.cs                → cadena de conexión (IP de la laptop)
│   ├── I{Categoria,Proveedor,Producto,Pedido,Reporte,Catalogo}Repository.cs
│   ├── {…}Repository.cs           → ADO.NET async + CommandType.StoredProcedure
│   └── ReaderExtensions.cs        → helpers para columnas NULL
├── Models/                        → Categoria, Proveedor, Producto, Pedido, DetallePedido, OpcionCombo, LineaReporte
├── ViewModels/                    → MainViewModel + una VM por sección (CommunityToolkit.Mvvm)
├── Views/                         → un UserControl por sección
├── Converters/  Themes/Theme.xaml → tomados de la plantilla WPFMMVMSP
├── MainWindow.xaml               → barra lateral de navegación + ContentControl
└── App.xaml                      → merge del tema + DataTemplates ViewModel→View
```

### Pantallas (requisitos "WPF + ADO .NET")

| Vista | Requisito |
|---|---|
| **Productos** | Mantenimiento de productos (CRUD) con combos de categoría y proveedor |
| **Categorías** | Mantenimiento de categorías (CRUD) |
| **Proveedores** | Mantenimiento de proveedores (CRUD) + **búsqueda por contacto y ciudad** |
| **Pedidos** | Mantenimiento de pedidos (CRUD de cabecera) + detalle del pedido seleccionado |
| **Detalle de pedidos por fecha** | Reporte **filtrando por rango de fechas** |

---

## 4. Puesta en marcha (checklist)

1. **Laptop** y **PC Windows** en la WiFi "iPhone de Alumno".
2. **Laptop**: SQL Server activo (`systemctl is-active mssql-server`) y ejecutar los scripts de `db/`.
3. **Laptop**: confirmar IP con `hostname -I` (esperado `172.20.10.4`).
4. **PC Windows**: `Test-NetConnection 172.20.10.4 -Port 1433` → `True`.
5. **PC Windows**: `git clone`, abrir `src/NeptunoApp.slnx` en Visual Studio 2022 → **F5**
   (o `dotnet run`). Ajustar la IP en `Data/DbConfig.cs` si cambió.

---

## 5. Repositorio · Capturas · Explicación · Observaciones

- **Repositorio:** este repo.
- **Capturas de las vistas:** carpeta [`docs/`](docs/).
- **Explicación / Observaciones y Conclusiones:** completar tras las pruebas en la PC Windows.
