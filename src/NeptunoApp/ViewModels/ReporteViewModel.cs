using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NeptunoApp.Data;
using NeptunoApp.Models;

namespace NeptunoApp.ViewModels;

/// <summary>
/// Reporte: detalles de pedidos (INNER JOIN con Pedidos) filtrando por un
/// intervalo de fechas (usp_DetallePedido_ListarPorRangoFechas).
/// </summary>
public partial class ReporteViewModel : ObservableObject
{
    private readonly IReporteRepository _repo;

    public ObservableCollection<LineaReporte> Lineas { get; } = new();

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
            var datos = await _repo.DetallePedidosPorRangoFechasAsync(FechaInicio, FechaFin);
            Lineas.Clear();
            foreach (var l in datos) Lineas.Add(l);
            TotalGeneral = datos.Sum(l => l.Subtotal);
            Mensaje = $"{Lineas.Count} lineas de detalle · Total: {TotalGeneral:C2}";
        }
        catch (Exception ex) { Mensaje = $"Error al generar el reporte: {ex.Message}"; }
    }
}
