using NeptunoApp.Models;

namespace NeptunoApp.Data;

public interface IProductoRepository
{
    Task<int> CrearAsync(Producto producto);
    Task<Producto?> ObtenerPorIdAsync(int productoId);
    Task<List<Producto>> ListarTodasAsync();
    Task ActualizarAsync(Producto producto);
    Task EliminarAsync(int productoId);
}
