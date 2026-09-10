using System;
using System.Data;
using System.Windows.Input;
using NeptunoApp.Data;
using NeptunoApp.Helpers;

namespace NeptunoApp.ViewModels
{
    /// <summary>
    /// Reporte: detalles de pedidos (INNER JOIN con Pedidos) filtrando por un
    /// intervalo de fechas. Ejecuta usp_DetallesPedidos_PorRangoFechas y enlaza
    /// el DataTable resultante (modo DESCONECTADO) a un DataGrid.
    /// </summary>
    public class ReportesViewModel : ViewModelBase
    {
        private readonly ReporteRepository _repo = new ReporteRepository();

        private DateTime _fechaInicio = new DateTime(DateTime.Today.Year, 1, 1);
        public DateTime FechaInicio { get => _fechaInicio; set => SetProperty(ref _fechaInicio, value); }

        private DateTime _fechaFin = DateTime.Today;
        public DateTime FechaFin { get => _fechaFin; set => SetProperty(ref _fechaFin, value); }

        private DataView _resultado;
        public DataView Resultado { get => _resultado; set => SetProperty(ref _resultado, value); }

        private string _mensaje;
        public string Mensaje { get => _mensaje; set => SetProperty(ref _mensaje, value); }

        public ICommand GenerarCommand { get; }

        public ReportesViewModel()
        {
            GenerarCommand = new RelayCommand(_ => Generar());
            Generar();
        }

        private void Generar()
        {
            try
            {
                if (FechaFin < FechaInicio)
                {
                    Mensaje = "La fecha final no puede ser anterior a la inicial.";
                    return;
                }

                DataTable tabla = _repo.DetallesPedidosPorRangoFechas(FechaInicio, FechaFin);
                Resultado = tabla.DefaultView;

                decimal total = 0m;
                foreach (DataRow r in tabla.Rows)
                    total += Convert.ToDecimal(r["Subtotal"]);

                Mensaje = $"{tabla.Rows.Count} lineas de detalle | Total: {total:C2}";
            }
            catch (Exception ex)
            {
                Mensaje = "Error: " + ex.Message;
            }
        }
    }
}
