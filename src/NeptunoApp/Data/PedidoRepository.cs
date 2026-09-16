using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

public class PedidoRepository : IPedidoRepository
{
    private readonly string _connectionString;

    public PedidoRepository(string connectionString) => _connectionString = connectionString;

    public async Task<int> CrearAsync(Pedido p)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Pedido_Crear", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(cmd, p, incluirId: false);
        var idParam = cmd.Parameters.Add("@PedidoID", SqlDbType.Int);
        idParam.Direction = ParameterDirection.Output;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return (int)idParam.Value;
    }

    public async Task<Pedido?> ObtenerPorIdAsync(int pedidoId)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Pedido_ObtenerPorId", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@PedidoID", SqlDbType.Int).Value = pedidoId;

        await cn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapCabecera(reader) : null;
    }

    public async Task<List<DetallePedido>> ObtenerDetalleAsync(int pedidoId)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Pedido_ObtenerPorId", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@PedidoID", SqlDbType.Int).Value = pedidoId;

        await cn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();

        var detalle = new List<DetallePedido>();
        // Result set 1 = cabecera (se omite); Result set 2 = detalle.
        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                detalle.Add(new DetallePedido
                {
                    ProductoID = reader.GetInt32(reader.GetOrdinal("ProductoID")),
                    NombreProducto = reader.GetString(reader.GetOrdinal("NombreProducto")),
                    PrecioUnidad = reader.GetDecimalSafe("PrecioUnidad"),
                    Cantidad = reader.GetInt16Safe("Cantidad"),
                    Descuento = reader.GetDecimalSafe("Descuento"),
                    Subtotal = reader.GetDecimalSafe("Subtotal")
                });
            }
        }
        return detalle;
    }

    public async Task<List<Pedido>> ListarTodasAsync()
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Pedido_ListarTodas", cn)
        {
            CommandType = CommandType.StoredProcedure
        };

        await cn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        var lista = new List<Pedido>();
        while (await reader.ReadAsync())
            lista.Add(MapLista(reader));
        return lista;
    }

    public async Task ActualizarAsync(Pedido p)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Pedido_Actualizar", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(cmd, p, incluirId: true);

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task EliminarAsync(int pedidoId)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Pedido_Eliminar", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@PedidoID", SqlDbType.Int).Value = pedidoId;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    // -----------------------------------------------------------------
    private static void AgregarParametros(SqlCommand cmd, Pedido p, bool incluirId)
    {
        if (incluirId) cmd.Parameters.Add("@PedidoID", SqlDbType.Int).Value = p.PedidoID;
        cmd.Parameters.Add("@ClienteID", SqlDbType.Int).Value = (object?)p.ClienteID ?? DBNull.Value;
        cmd.Parameters.Add("@EmpleadoID", SqlDbType.Int).Value = (object?)p.EmpleadoID ?? DBNull.Value;
        cmd.Parameters.Add("@FechaPedido", SqlDbType.Date).Value = (object?)p.FechaPedido ?? DateTime.Today;
        cmd.Parameters.Add("@FechaRequerida", SqlDbType.Date).Value = (object?)p.FechaRequerida ?? DBNull.Value;
        cmd.Parameters.Add("@FechaEnvio", SqlDbType.Date).Value = (object?)p.FechaEnvio ?? DBNull.Value;
        cmd.Parameters.Add("@TransportistaID", SqlDbType.Int).Value = (object?)p.TransportistaID ?? DBNull.Value;
        cmd.Parameters.Add("@Destinatario", SqlDbType.NVarChar, 60).Value = Val(p.Destinatario);
        cmd.Parameters.Add("@CiudadDestino", SqlDbType.NVarChar, 30).Value = Val(p.CiudadDestino);
        cmd.Parameters.Add("@PaisDestino", SqlDbType.NVarChar, 30).Value = Val(p.PaisDestino);
    }

    private static object Val(string? s) => string.IsNullOrWhiteSpace(s) ? DBNull.Value : s.Trim();

    private static Pedido MapCabecera(SqlDataReader r) => new()
    {
        PedidoID = r.GetInt32(r.GetOrdinal("PedidoID")),
        ClienteID = r.GetIntOrNull("ClienteID"),
        EmpleadoID = r.GetIntOrNull("EmpleadoID"),
        TransportistaID = r.GetIntOrNull("TransportistaID"),
        FechaPedido = r.GetDateTimeOrNull("FechaPedido"),
        FechaRequerida = r.GetDateTimeOrNull("FechaRequerida"),
        FechaEnvio = r.GetDateTimeOrNull("FechaEnvio"),
        Destinatario = r.GetStringOrNull("Destinatario"),
        CiudadDestino = r.GetStringOrNull("CiudadDestino"),
        PaisDestino = r.GetStringOrNull("PaisDestino")
    };

    private static Pedido MapLista(SqlDataReader r) => new()
    {
        PedidoID = r.GetInt32(r.GetOrdinal("PedidoID")),
        ClienteID = r.GetIntOrNull("ClienteID"),
        EmpleadoID = r.GetIntOrNull("EmpleadoID"),
        TransportistaID = r.GetIntOrNull("TransportistaID"),
        Cliente = r.GetStringOrNull("Cliente"),
        Empleado = r.GetStringOrNull("Empleado"),
        FechaPedido = r.GetDateTimeOrNull("FechaPedido"),
        FechaRequerida = r.GetDateTimeOrNull("FechaRequerida"),
        FechaEnvio = r.GetDateTimeOrNull("FechaEnvio"),
        Destinatario = r.GetStringOrNull("Destinatario"),
        CiudadDestino = r.GetStringOrNull("CiudadDestino"),
        PaisDestino = r.GetStringOrNull("PaisDestino"),
        TotalPedido = r.GetDecimalSafe("TotalPedido")
    };
}
