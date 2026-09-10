using NeptunoApp.Models;

namespace NeptunoApp.Data;

public interface ICatalogoRepository
{
    Task<List<OpcionCombo>> CategoriasAsync();
    Task<List<OpcionCombo>> ProveedoresAsync();
    Task<List<OpcionCombo>> ClientesAsync();
    Task<List<OpcionCombo>> EmpleadosAsync();
    Task<List<OpcionCombo>> TransportistasAsync();
}
