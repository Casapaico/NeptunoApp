using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace NeptunoApp.Data
{
    /// <summary>
    /// Reportes con PROCEDIMIENTOS ALMACENADOS.
    /// Devuelve DataTable en modo DESCONECTADO (SqlDataAdapter.Fill) para
    /// enlazarlo directamente a un DataGrid.
    /// </summary>
    public class ReporteRepository
    {
        /// <summary>
        /// Detalles de pedidos (INNER JOIN con Pedidos) filtrando por un
        /// intervalo de fechas. Ejecuta usp_DetallesPedidos_PorRangoFechas.
        /// </summary>
        public DataTable DetallesPedidosPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            var tabla = new DataTable("DetallesPedidos");

            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_DetallesPedidos_PorRangoFechas", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Date);

                using (var adapter = new SqlDataAdapter(cmd))
                    adapter.Fill(tabla);   // la conexion se abre y cierra internamente
            }

            return tabla;                  // datos desconectados, viven en memoria
        }
    }
}
