using NeptunoApp.Models;

namespace NeptunoApp.Data;

public interface IReporteRepository
{
    Task<List<LineaReporte>> DetallePedidosPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);
}
