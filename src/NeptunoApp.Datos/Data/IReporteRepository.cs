using System.Data;

namespace NeptunoApp.Data;

public interface IReporteRepository
{
    Task<DataTable> DetallePedidosPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);
}
