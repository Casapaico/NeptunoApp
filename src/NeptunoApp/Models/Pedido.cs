using System;

namespace NeptunoApp.Models
{
    public class Pedido
    {
        public int IdPedido { get; set; }
        public int? IdCliente { get; set; }
        public int? IdEmpleado { get; set; }
        public string NombreCliente { get; set; }
        public string NombreEmpleado { get; set; }
        public DateTime? FechaPedido { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public decimal Flete { get; set; }
        public string Destinatario { get; set; }
        public string CiudadDestino { get; set; }
        public string PaisDestino { get; set; }
        public decimal TotalPedido { get; set; }
    }
}
