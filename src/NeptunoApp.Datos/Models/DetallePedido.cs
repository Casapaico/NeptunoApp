using CommunityToolkit.Mvvm.ComponentModel;

namespace NeptunoApp.Models;

public partial class DetallePedido : ObservableObject
{
    [ObservableProperty] private int productoID;
    [ObservableProperty] private string nombreProducto = string.Empty;
    [ObservableProperty] private decimal precioUnidad;
    [ObservableProperty] private short cantidad;
    [ObservableProperty] private decimal descuento;
    [ObservableProperty] private decimal subtotal;
}
