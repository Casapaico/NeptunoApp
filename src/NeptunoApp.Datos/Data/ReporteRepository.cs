using System.Data;
using Microsoft.Data.SqlClient;

namespace NeptunoApp.Data;

/// <summary>
/// Reporte en modo desconectado: SqlDataAdapter.Fill llena un DataTable y
/// cierra la conexion; la UI navega/ordena el DataTable sin mantener el
/// vinculo abierto con el servidor (no hay escritura de vuelta, por eso no
/// hace falta un DataSet tipado ni CommandBuilder de actualizacion).
/// </summary>
public class ReporteRepository : IReporteRepository
{
    private readonly string _connectionString;

    public ReporteRepository(string connectionString) => _connectionString = connectionString;

    public async Task<DataTable> DetallePedidosPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("dbo.usp_DetallePedido_ListarPorRangoFechas", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = fechaInicio.Date;
        cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = fechaFin.Date;

        var tabla = new DataTable("DetallePedidos");
        using var adapter = new SqlDataAdapter(cmd);
        // SqlDataAdapter.Fill no tiene overload async: se corre en un pool
        // thread para no bloquear el hilo de UI (Fill abre y cierra la
        // conexion por su cuenta).
        await Task.Run(() => adapter.Fill(tabla));
        return tabla;
    }
}
