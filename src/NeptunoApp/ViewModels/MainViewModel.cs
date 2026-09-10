using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NeptunoApp.Data;

namespace NeptunoApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public ProductosViewModel Productos { get; }
    public CategoriasViewModel Categorias { get; }
    public ProveedoresViewModel Proveedores { get; }
    public PedidosViewModel Pedidos { get; }
    public ReporteViewModel Reporte { get; }

    [ObservableProperty]
    private ObservableObject seccionActual;

    public MainViewModel(
        IProductoRepository productoRepo,
        ICategoriaRepository categoriaRepo,
        IProveedorRepository proveedorRepo,
        IPedidoRepository pedidoRepo,
        IReporteRepository reporteRepo,
        ICatalogoRepository catalogoRepo)
    {
        Productos = new ProductosViewModel(productoRepo, catalogoRepo);
        Categorias = new CategoriasViewModel(categoriaRepo);
        Proveedores = new ProveedoresViewModel(proveedorRepo);
        Pedidos = new PedidosViewModel(pedidoRepo, catalogoRepo);
        Reporte = new ReporteViewModel(reporteRepo);

        seccionActual = Productos;
    }

    [RelayCommand] private void IrAProductos() => SeccionActual = Productos;
    [RelayCommand] private void IrACategorias() => SeccionActual = Categorias;
    [RelayCommand] private void IrAProveedores() => SeccionActual = Proveedores;
    [RelayCommand] private void IrAPedidos() => SeccionActual = Pedidos;
    [RelayCommand] private void IrAReporte() => SeccionActual = Reporte;

    public string SeccionActualNombre => SeccionActual switch
    {
        ProductosViewModel => "Productos",
        CategoriasViewModel => "Categorias",
        ProveedoresViewModel => "Proveedores",
        PedidosViewModel => "Pedidos",
        ReporteViewModel => "Reporte por fechas",
        _ => string.Empty
    };

    partial void OnSeccionActualChanged(ObservableObject value) => OnPropertyChanged(nameof(SeccionActualNombre));
}
