using NeptunoApp.Models;

namespace NeptunoApp.Data;

public interface IProveedorRepository
{
    Task<int> CrearAsync(Proveedor proveedor);
    Task<Proveedor?> ObtenerPorIdAsync(int proveedorId);
    Task<List<Proveedor>> ListarTodasAsync();
    Task<List<Proveedor>> BuscarPorContactoCiudadAsync(string? nombreContacto, string? ciudad);
    Task ActualizarAsync(Proveedor proveedor);
    Task EliminarAsync(int proveedorId);
}
