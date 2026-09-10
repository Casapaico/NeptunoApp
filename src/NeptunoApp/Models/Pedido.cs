using CommunityToolkit.Mvvm.ComponentModel;

namespace NeptunoApp.Models;

public partial class Pedido : ObservableObject
{
    [ObservableProperty] private int pedidoID;
    [ObservableProperty] private int? clienteID;
    [ObservableProperty] private int? empleadoID;
    [ObservableProperty] private int? transportistaID;
    [ObservableProperty] private string? cliente;
    [ObservableProperty] private string? empleado;
    [ObservableProperty] private DateTime? fechaPedido;
    [ObservableProperty] private DateTime? fechaRequerida;
    [ObservableProperty] private DateTime? fechaEnvio;
    [ObservableProperty] private string? destinatario;
    [ObservableProperty] private string? ciudadDestino;
    [ObservableProperty] private string? paisDestino;
    [ObservableProperty] private decimal totalPedido;
}
