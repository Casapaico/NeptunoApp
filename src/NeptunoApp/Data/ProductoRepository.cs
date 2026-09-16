using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

public class ProductoRepository : IProductoRepository
{
    private readonly string _connectionString;

    public ProductoRepository(string connectionString) => _connectionString = connectionString;

    public async Task<int> CrearAsync(Producto p)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Producto_Crear", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(cmd, p, incluirId: false);
        var idParam = cmd.Parameters.Add("@ProductoID", SqlDbType.Int);
        idParam.Direction = ParameterDirection.Output;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return (int)idParam.Value;
    }

    public async Task<Producto?> ObtenerPorIdAsync(int productoId)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Producto_ObtenerPorId", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@ProductoID", SqlDbType.Int).Value = productoId;

        await cn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Map(reader) : null;
    }

    public async Task<List<Producto>> ListarTodasAsync()
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Producto_ListarTodas", cn)
        {
            CommandType = CommandType.StoredProcedure
        };

        await cn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        var lista = new List<Producto>();
        while (await reader.ReadAsync())
            lista.Add(Map(reader));
        return lista;
    }

    public async Task ActualizarAsync(Producto p)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Producto_Actualizar", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(cmd, p, incluirId: true);

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task EliminarAsync(int productoId)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Producto_Eliminar", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@ProductoID", SqlDbType.Int).Value = productoId;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    // -----------------------------------------------------------------
    private static void AgregarParametros(SqlCommand cmd, Producto p, bool incluirId)
    {
        if (incluirId) cmd.Parameters.Add("@ProductoID", SqlDbType.Int).Value = p.ProductoID;
        cmd.Parameters.Add("@NombreProducto", SqlDbType.NVarChar, 60).Value = p.NombreProducto;
        cmd.Parameters.Add("@ProveedorID", SqlDbType.Int).Value = (object?)p.ProveedorID ?? DBNull.Value;
        cmd.Parameters.Add("@CategoriaID", SqlDbType.Int).Value = (object?)p.CategoriaID ?? DBNull.Value;
        cmd.Parameters.Add("@CantidadPorUnidad", SqlDbType.NVarChar, 30).Value =
            string.IsNullOrWhiteSpace(p.CantidadPorUnidad) ? DBNull.Value : p.CantidadPorUnidad.Trim();
        cmd.Parameters.Add("@PrecioUnidad", SqlDbType.Decimal).Value = p.PrecioUnidad;
        cmd.Parameters.Add("@UnidadesEnExistencia", SqlDbType.SmallInt).Value = p.UnidadesEnExistencia;
        cmd.Parameters.Add("@UnidadesEnPedido", SqlDbType.SmallInt).Value = p.UnidadesEnPedido;
        cmd.Parameters.Add("@NivelDeReorden", SqlDbType.SmallInt).Value = p.NivelDeReorden;
        cmd.Parameters.Add("@Descontinuado", SqlDbType.Bit).Value = p.Descontinuado;
    }

    private static Producto Map(SqlDataReader r) => new()
    {
        ProductoID = r.GetInt32(r.GetOrdinal("ProductoID")),
        NombreProducto = r.GetString(r.GetOrdinal("NombreProducto")),
        ProveedorID = r.GetIntOrNull("ProveedorID"),
        CategoriaID = r.GetIntOrNull("CategoriaID"),
        Proveedor = HasColumn(r, "Proveedor") ? r.GetStringOrNull("Proveedor") : null,
        Categoria = HasColumn(r, "Categoria") ? r.GetStringOrNull("Categoria") : null,
        CantidadPorUnidad = r.GetStringOrNull("CantidadPorUnidad"),
        PrecioUnidad = r.GetDecimalSafe("PrecioUnidad"),
        UnidadesEnExistencia = r.GetInt16Safe("UnidadesEnExistencia"),
        UnidadesEnPedido = r.GetInt16Safe("UnidadesEnPedido"),
        NivelDeReorden = r.GetInt16Safe("NivelDeReorden"),
        Descontinuado = !r.IsDBNull(r.GetOrdinal("Descontinuado")) && r.GetBoolean(r.GetOrdinal("Descontinuado"))
    };

    private static bool HasColumn(SqlDataReader r, string name)
    {
        for (int i = 0; i < r.FieldCount; i++)
            if (string.Equals(r.GetName(i), name, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }
}
