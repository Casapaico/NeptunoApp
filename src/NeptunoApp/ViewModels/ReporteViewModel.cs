using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NeptunoApp.Data;

namespace NeptunoApp.ViewModels;

/// <summary>
/// Reporte: detalles de pedidos (INNER JOIN con Pedidos) filtrando por un
/// intervalo de fechas (usp_DetallePedido_ListarPorRangoFechas). Se muestra
/// en modo desconectado: el repositorio llena un DataTable y aqui solo se
/// navega/lee, sin conexion abierta con el servidor.
/// </summary>
public partial class ReporteViewModel : ObservableObject
{
    private readonly IReporteRepository _repo;

    [ObservableProperty] private DataTable lineas = new();

    [ObservableProperty] private DateTime fechaInicio = new(DateTime.Today.Year, 1, 1);
    [ObservableProperty] private DateTime fechaFin = DateTime.Today;
    [ObservableProperty] private string? mensaje;
    [ObservableProperty] private decimal totalGeneral;

    public ReporteViewModel(IReporteRepository repo)
    {
        _repo = repo;
        _ = GenerarAsync();
    }

    [RelayCommand]
    private async Task GenerarAsync()
    {
        Mensaje = null;
        if (FechaFin < FechaInicio)
        {
            Mensaje = "La fecha final no puede ser anterior a la inicial.";
            return;
        }

        try
        {
            Lineas = await _repo.DetallePedidosPorRangoFechasAsync(FechaInicio, FechaFin);

            decimal total = 0;
            foreach (DataRow fila in Lineas.Rows)
                total += (decimal)fila["Subtotal"];
            TotalGeneral = total;
            Mensaje = $"{Lineas.Rows.Count} lineas de detalle · Total: {TotalGeneral:C2}";
        }
        catch (Exception ex) { Mensaje = $"Error al generar el reporte: {ex.Message}"; }
    }
}
