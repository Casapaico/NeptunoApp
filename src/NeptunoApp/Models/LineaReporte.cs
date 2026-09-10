namespace NeptunoApp.Models;

/// <summary>Fila del reporte de detalles de pedidos por rango de fechas.</summary>
public class LineaReporte
{
    public int PedidoID { get; set; }
    public DateTime FechaPedido { get; set; }
    public string? Cliente { get; set; }
    public string? Producto { get; set; }
    public decimal PrecioUnidad { get; set; }
    public short Cantidad { get; set; }
    public decimal Descuento { get; set; }
    public decimal Subtotal { get; set; }
}
