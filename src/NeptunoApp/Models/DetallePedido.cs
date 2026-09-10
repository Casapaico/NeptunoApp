namespace NeptunoApp.Models
{
    public class DetallePedido
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public decimal PrecioUnidad { get; set; }
        public short Cantidad { get; set; }
        public double Descuento { get; set; }
        public decimal Subtotal { get; set; }
    }
}
