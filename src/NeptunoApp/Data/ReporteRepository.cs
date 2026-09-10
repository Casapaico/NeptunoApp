using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

public class ReporteRepository : IReporteRepository
{
    private readonly string _connectionString;

    public ReporteRepository(string connectionString) => _connectionString = connectionString;

    public async Task<List<LineaReporte>> DetallePedidosPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_DetallePedido_ListarPorRangoFechas", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = fechaInicio.Date;
        cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = fechaFin.Date;

        await cn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        var lista = new List<LineaReporte>();
        while (await reader.ReadAsync())
        {
            lista.Add(new LineaReporte
            {
                PedidoID = reader.GetInt32(reader.GetOrdinal("PedidoID")),
                FechaPedido = Convert.ToDateTime(reader["FechaPedido"]),
                Cliente = reader.GetStringOrNull("Cliente"),
                Producto = reader.GetStringOrNull("Producto"),
                PrecioUnidad = reader.GetDecimalSafe("PrecioUnidad"),
                Cantidad = reader.GetInt16Safe("Cantidad"),
                Descuento = reader.GetDecimalSafe("Descuento"),
                Subtotal = reader.GetDecimalSafe("Subtotal")
            });
        }
        return lista;
    }
}
