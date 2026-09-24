using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

public class ProveedorRepository : IProveedorRepository
{
    private readonly string _connectionString;

    public ProveedorRepository(string connectionString) => _connectionString = connectionString;

    public async Task<int> CrearAsync(Proveedor p)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Proveedor_Crear", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(cmd, p, incluirId: false);
        var idParam = cmd.Parameters.Add("@ProveedorID", SqlDbType.Int);
        idParam.Direction = ParameterDirection.Output;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return (int)idParam.Value;
    }

    public async Task<Proveedor?> ObtenerPorIdAsync(int proveedorId)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Proveedor_ObtenerPorId", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@ProveedorID", SqlDbType.Int).Value = proveedorId;

        await cn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Map(reader) : null;
    }

    public async Task<List<Proveedor>> ListarTodasAsync()
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Proveedor_ListarTodas", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        return await LeerListaAsync(cn, cmd);
    }

    public async Task<List<Proveedor>> BuscarPorContactoCiudadAsync(string? nombreContacto, string? ciudad)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Proveedor_BuscarPorContactoCiudad", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@NombreContacto", SqlDbType.NVarChar, 40).Value =
            string.IsNullOrWhiteSpace(nombreContacto) ? DBNull.Value : nombreContacto.Trim();
        cmd.Parameters.Add("@Ciudad", SqlDbType.NVarChar, 30).Value =
            string.IsNullOrWhiteSpace(ciudad) ? DBNull.Value : ciudad.Trim();
        return await LeerListaAsync(cn, cmd);
    }

    public async Task ActualizarAsync(Proveedor p)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Proveedor_Actualizar", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(cmd, p, incluirId: true);

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task EliminarAsync(int proveedorId)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.usp_Proveedor_Eliminar", cn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@ProveedorID", SqlDbType.Int).Value = proveedorId;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    // -----------------------------------------------------------------
    private static async Task<List<Proveedor>> LeerListaAsync(SqlConnection cn, SqlCommand cmd)
    {
        await cn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        var lista = new List<Proveedor>();
        while (await reader.ReadAsync())
            lista.Add(Map(reader));
        return lista;
    }

    private static void AgregarParametros(SqlCommand cmd, Proveedor p, bool incluirId)
    {
        if (incluirId) cmd.Parameters.Add("@ProveedorID", SqlDbType.Int).Value = p.ProveedorID;
        cmd.Parameters.Add("@CompaniaNombre", SqlDbType.NVarChar, 60).Value = p.CompaniaNombre;
        cmd.Parameters.Add("@NombreContacto", SqlDbType.NVarChar, 40).Value = Val(p.NombreContacto);
        cmd.Parameters.Add("@CargoContacto", SqlDbType.NVarChar, 40).Value = Val(p.CargoContacto);
        cmd.Parameters.Add("@Direccion", SqlDbType.NVarChar, 80).Value = Val(p.Direccion);
        cmd.Parameters.Add("@Ciudad", SqlDbType.NVarChar, 30).Value = Val(p.Ciudad);
        cmd.Parameters.Add("@CodigoPostal", SqlDbType.NVarChar, 10).Value = Val(p.CodigoPostal);
        cmd.Parameters.Add("@Pais", SqlDbType.NVarChar, 30).Value = Val(p.Pais);
        cmd.Parameters.Add("@Telefono", SqlDbType.NVarChar, 24).Value = Val(p.Telefono);
        cmd.Parameters.Add("@Fax", SqlDbType.NVarChar, 24).Value = Val(p.Fax);
    }

    private static object Val(string? s) => string.IsNullOrWhiteSpace(s) ? DBNull.Value : s.Trim();

    private static Proveedor Map(SqlDataReader r) => new()
    {
        ProveedorID = r.GetInt32(r.GetOrdinal("ProveedorID")),
        CompaniaNombre = r.GetString(r.GetOrdinal("CompaniaNombre")),
        NombreContacto = r.GetStringOrNull("NombreContacto"),
        CargoContacto = r.GetStringOrNull("CargoContacto"),
        Direccion = r.GetStringOrNull("Direccion"),
        Ciudad = r.GetStringOrNull("Ciudad"),
        CodigoPostal = r.GetStringOrNull("CodigoPostal"),
        Pais = r.GetStringOrNull("Pais"),
        Telefono = r.GetStringOrNull("Telefono"),
        Fax = r.GetStringOrNull("Fax")
    };
}
