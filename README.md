# Neptuno — ADO .NET Semana 04 (WPF + MVVM)

Desafío del curso **Desarrollo de Aplicaciones Empresariales Avanzado** (Tecsup).
Docente: *Arévalo Sermeño, Edwin William*.

Aplicación de escritorio **WPF (.NET 8)** con patrón **MVVM** y acceso a datos con
**ADO.NET clásico** (`Microsoft.Data.SqlClient`) consumiendo **procedimientos
almacenados**: `SqlConnection`, `SqlCommand` (modo conectado, `SqlDataReader`)
y `SqlDataAdapter` + `DataTable` (modo desconectado).

---

## 1. Arquitectura de red

La base de datos vive en la **laptop** (Ubuntu + SQL Server) y la aplicación .NET
se ejecuta en la **PC Windows** institucional. Ambas se conectan por la red WiFi
**"iPhone de Alumno"**.

```
   PC Windows (VS 2022)                     Laptop Ubuntu
   ┌──────────────────┐   WiFi "iPhone     ┌────────────────────┐
   │  NeptunoApp.exe  │   de Alumno"       │  SQL Server 2022   │
   │  (WPF + ADO.NET) │ ───────────────▶   │  172.20.10.4:1433  │
   └──────────────────┘   172.20.10.0/28   │  BD: NeptunoDB     │
                                           └────────────────────┘
```

| Parámetro | Valor |
|---|---|
| Red WiFi | iPhone de Alumno (rango `172.20.10.0/28`) |
| Gateway | `172.20.10.1` |
| Laptop / SQL Server | `172.20.10.4`, puerto TCP `1433` |
| Usuario SQL | `sa` |
| Base de datos | `NeptunoDB` |

> Si la IP de la laptop cambia, verificarla en Ubuntu con `hostname -I` y
> actualizar `Server=` en [`src/NeptunoApp/App.config`](src/NeptunoApp/App.config).

---

## 2. Base de datos

Scripts en la carpeta [`db/`](db/) — se ejecutan **en la laptop**, en orden:

| Script | Contenido |
|---|---|
| [`db/01_NeptunoDB.sql`](db/01_NeptunoDB.sql) | Crea `NeptunoDB` (re-ejecutable) con las tablas `Categorias`, `Proveedores`, `Productos`, `Clientes`, `Empleados`, `Pedidos`, `DetallesPedidos` y datos de prueba. |
| [`db/02_Procedimientos.sql`](db/02_Procedimientos.sql) | Crea los procedimientos almacenados (`CREATE OR ALTER`). |

```bash
# desde la carpeta del repo, en Ubuntu
sqlcmd -S localhost -U sa -P 'TU_PASSWORD' -C -i db/01_NeptunoDB.sql
sqlcmd -S localhost -U sa -P 'TU_PASSWORD' -C -i db/02_Procedimientos.sql
```

### Procedimientos almacenados

| Procedimiento | Descripción |
|---|---|
| `usp_Categorias_Listar / _ObtenerPorId / _Insertar / _Actualizar / _Eliminar` | CRUD de categorías |
| `usp_Proveedores_Listar / _ObtenerPorId / _Insertar / _Actualizar / _Eliminar` | CRUD de proveedores |
| `usp_Proveedores_Buscar` | **Listado de proveedores buscando por `nombreContacto` y `ciudad`** (filtros opcionales, `LIKE`) |
| `usp_Productos_Listar / _ObtenerPorId / _Insertar / _Actualizar / _Eliminar` | CRUD de productos |
| `usp_Pedidos_Listar / _ObtenerPorId / _Insertar / _Actualizar / _Eliminar` | CRUD de pedidos (cabecera). `_ObtenerPorId` devuelve 2 result sets: cabecera + detalle |
| `usp_DetallesPedidos_PorRangoFechas` | **Listado de detalles de pedidos con `INNER JOIN` a `Pedidos`, filtrando por un intervalo de fechas** |

---

## 3. Aplicación WPF

Solución en [`src/NeptunoApp.sln`](src/NeptunoApp.sln).

```bash
cd src
dotnet build NeptunoApp.sln      # compila (en Windows además: F5 en Visual Studio)
```

> El proyecto usa `net8.0-windows`; WPF **solo se ejecuta en Windows**. Desde
> Linux se puede compilar (gracias a `EnableWindowsTargeting`), pero no correr.

### Estructura

```
src/NeptunoApp/
├── App.config            → cadena de conexión (IP de la laptop)
├── App.xaml              → mapeo ViewModel → View (DataTemplates)
├── Helpers/              → ViewModelBase (INotifyPropertyChanged), RelayCommand
├── Models/               → Categoria, Proveedor, Producto, Pedido, DetallePedido, ItemCombo
├── Data/                 → repositorios ADO.NET (uno por entidad) + ConexionBD
│   ├── CategoriaRepository.cs    (SqlDataAdapter → DataTable, desconectado)
│   ├── ProveedorRepository.cs    (SqlDataReader, conectado) + Buscar
│   ├── ProductoRepository.cs     (SqlDataReader, conectado)
│   ├── PedidoRepository.cs       (cabecera + detalle con NextResult)
│   ├── CatalogoRepository.cs     (combos: categorías, proveedores, clientes, empleados)
│   └── ReporteRepository.cs      (SqlDataAdapter → DataTable, desconectado)
├── ViewModels/           → uno por pantalla (MVVM)
└── Views/                → MainWindow + 5 UserControls
```

### Pantallas

| Vista | Requisito del enunciado |
|---|---|
| **Productos** | Mantenimiento de productos (CRUD) con combos de categoría y proveedor |
| **Categorías** | Mantenimiento de categorías (CRUD) |
| **Proveedores** | Mantenimiento de proveedores (CRUD) + **búsqueda por contacto y ciudad** |
| **Pedidos** | Mantenimiento de pedidos (CRUD de cabecera) + detalle del pedido seleccionado |
| **Reporte** | Detalle de pedidos **filtrando por rango de fechas** |

Todas las operaciones de datos se hacen con `CommandType.StoredProcedure`.

---

## 4. Puesta en marcha (checklist)

1. **Laptop** — misma WiFi "iPhone de Alumno"; SQL Server activo:
   `systemctl is-active mssql-server` → `active`.
2. **Laptop** — ejecutar los 2 scripts de `db/`.
3. **Laptop** — confirmar la IP: `hostname -I` (esperado `172.20.10.4`).
4. **PC Windows** — misma WiFi; probar `ping 172.20.10.4`.
5. **PC Windows** — abrir `src/NeptunoApp.sln` en Visual Studio 2022, ajustar la IP
   en `App.config` si cambió, y presionar **F5**.

> SQL Server escucha en `0.0.0.0:1433` (todas las interfaces) y el firewall de la
> laptop está inactivo (`ufw status` → inactive).

---

## 5. Capturas de las vistas

Ver carpeta [`docs/`](docs/).

---

## 6. Explicación

*(Completar: describir el patrón MVVM aplicado, el uso de procedimientos
almacenados desde ADO.NET, y la diferencia entre el modo conectado
—`SqlDataReader`— y el desconectado —`SqlDataAdapter` + `DataTable`—.)*

---

## 7. Observaciones y Conclusiones

*(Completar tras las pruebas en la PC Windows.)*
