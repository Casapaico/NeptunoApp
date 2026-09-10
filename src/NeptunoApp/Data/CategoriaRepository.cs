using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly string _connectionString;

    public CategoriaRepository(string connectionString) => _connectionString = connectionString;

    public async Task<int> CrearAsync(Categoria c)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Categoria_Crear", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@NombreCategoria", SqlDbType.NVarChar, 30).Value = c.NombreCategoria;
        cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 200).Value = (object?)c.Descripcion ?? DBNull.Value;

        await cn.OpenAsync();
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<Categoria?> ObtenerPorIdAsync(int categoriaId)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Categoria_ObtenerPorId", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@CategoriaID", SqlDbType.Int).Value = categoriaId;

        await cn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Map(reader) : null;
    }

    public async Task<List<Categoria>> ListarTodasAsync()
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Categoria_ListarTodas", cn)
        {
            CommandType = CommandType.StoredProcedure
        };

        await cn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        var lista = new List<Categoria>();
        while (await reader.ReadAsync())
            lista.Add(Map(reader));
        return lista;
    }

    public async Task ActualizarAsync(Categoria c)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Categoria_Actualizar", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@CategoriaID", SqlDbType.Int).Value = c.CategoriaID;
        cmd.Parameters.Add("@NombreCategoria", SqlDbType.NVarChar, 30).Value = c.NombreCategoria;
        cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 200).Value = (object?)c.Descripcion ?? DBNull.Value;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task EliminarAsync(int categoriaId)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Categoria_Eliminar", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@CategoriaID", SqlDbType.Int).Value = categoriaId;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    private static Categoria Map(SqlDataReader r) => new()
    {
        CategoriaID = r.GetInt32(r.GetOrdinal("CategoriaID")),
        NombreCategoria = r.GetString(r.GetOrdinal("NombreCategoria")),
        Descripcion = r.IsDBNull(r.GetOrdinal("Descripcion")) ? null : r.GetString(r.GetOrdinal("Descripcion"))
    };
}
