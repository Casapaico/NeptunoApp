using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

/// <summary>Listas para poblar ComboBox (categorias, proveedores, clientes, empleados, transportistas).</summary>
public class CatalogoRepository : ICatalogoRepository
{
    private readonly string _connectionString;

    public CatalogoRepository(string connectionString) => _connectionString = connectionString;

    public Task<List<OpcionCombo>> CategoriasAsync()     => LeerAsync("dbo.usp_Catalogo_Categorias");
    public Task<List<OpcionCombo>> ProveedoresAsync()    => LeerAsync("dbo.usp_Catalogo_Proveedores");
    public Task<List<OpcionCombo>> ClientesAsync()       => LeerAsync("dbo.usp_Catalogo_Clientes");
    public Task<List<OpcionCombo>> EmpleadosAsync()      => LeerAsync("dbo.usp_Catalogo_Empleados");
    public Task<List<OpcionCombo>> TransportistasAsync() => LeerAsync("dbo.usp_Catalogo_Transportistas");

    private async Task<List<OpcionCombo>> LeerAsync(string sp)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sp, cn) { CommandType = CommandType.StoredProcedure };

        await cn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        var lista = new List<OpcionCombo>();
        while (await reader.ReadAsync())
            lista.Add(new OpcionCombo { Id = reader.GetInt32(0), Texto = reader.GetString(1) });
        return lista;
    }
}
