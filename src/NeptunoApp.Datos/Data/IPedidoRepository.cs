using NeptunoApp.Models;

namespace NeptunoApp.Data;

public interface IPedidoRepository
{
    Task<int> CrearAsync(Pedido pedido);
    Task<Pedido?> ObtenerPorIdAsync(int pedidoId);
    Task<List<DetallePedido>> ObtenerDetalleAsync(int pedidoId);
    Task<List<Pedido>> ListarTodasAsync();
    Task ActualizarAsync(Pedido pedido);
    Task EliminarAsync(int pedidoId);
}
