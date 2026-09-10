using CommunityToolkit.Mvvm.ComponentModel;

namespace NeptunoApp.Models;

public partial class Producto : ObservableObject
{
    [ObservableProperty] private int productoID;
    [ObservableProperty] private string nombreProducto = string.Empty;
    [ObservableProperty] private int? proveedorID;
    [ObservableProperty] private int? categoriaID;
    [ObservableProperty] private string? proveedor;
    [ObservableProperty] private string? categoria;
    [ObservableProperty] private string? cantidadPorUnidad;
    [ObservableProperty] private decimal precioUnidad;
    [ObservableProperty] private short unidadesEnExistencia;
    [ObservableProperty] private short unidadesEnPedido;
    [ObservableProperty] private short nivelDeReorden;
    [ObservableProperty] private bool descontinuado;
}
