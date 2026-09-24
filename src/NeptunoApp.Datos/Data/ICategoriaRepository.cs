using NeptunoApp.Models;

namespace NeptunoApp.Data;

public interface ICategoriaRepository
{
    Task<int> CrearAsync(Categoria categoria);
    Task<Categoria?> ObtenerPorIdAsync(int categoriaId);
    Task<List<Categoria>> ListarTodasAsync();
    Task ActualizarAsync(Categoria categoria);
    Task EliminarAsync(int categoriaId);
}
