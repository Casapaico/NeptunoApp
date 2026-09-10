using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NeptunoApp.Data;
using NeptunoApp.Helpers;
using NeptunoApp.Models;

namespace NeptunoApp.ViewModels
{
    /// <summary>
    /// Mantenimiento (CRUD) de Pedidos (cabecera) usando procedimientos almacenados.
    /// Al seleccionar un pedido se muestra su detalle (segundo result set del SP
    /// usp_Pedidos_ObtenerPorId).
    /// </summary>
    public class PedidosViewModel : ViewModelBase
    {
        private readonly PedidoRepository _repo = new PedidoRepository();
        private readonly CatalogoRepository _catalogo = new CatalogoRepository();

        public ObservableCollection<Pedido> Lista { get; } = new ObservableCollection<Pedido>();
        public ObservableCollection<DetallePedido> Detalle { get; } = new ObservableCollection<DetallePedido>();
        public ObservableCollection<ItemCombo> Clientes { get; } = new ObservableCollection<ItemCombo>();
        public ObservableCollection<ItemCombo> Empleados { get; } = new ObservableCollection<ItemCombo>();

        private Pedido _seleccionado;
        public Pedido Seleccionado
        {
            get => _seleccionado;
            set
            {
                if (SetProperty(ref _seleccionado, value) && value != null)
                    CargarFormulario(value);
            }
        }

        private int _id;
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        private ItemCombo _clienteSeleccionado;
        public ItemCombo ClienteSeleccionado { get => _clienteSeleccionado; set => SetProperty(ref _clienteSeleccionado, value); }

        private ItemCombo _empleadoSeleccionado;
        public ItemCombo EmpleadoSeleccionado { get => _empleadoSeleccionado; set => SetProperty(ref _empleadoSeleccionado, value); }

        private DateTime? _fechaPedido = DateTime.Today;
        public DateTime? FechaPedido { get => _fechaPedido; set => SetProperty(ref _fechaPedido, value); }

        private DateTime? _fechaEntrega;
        public DateTime? FechaEntrega { get => _fechaEntrega; set => SetProperty(ref _fechaEntrega, value); }

        private DateTime? _fechaEnvio;
        public DateTime? FechaEnvio { get => _fechaEnvio; set => SetProperty(ref _fechaEnvio, value); }

        private decimal _flete;
        public decimal Flete { get => _flete; set => SetProperty(ref _flete, value); }

        private string _destinatario;
        public string Destinatario { get => _destinatario; set => SetProperty(ref _destinatario, value); }

        private string _ciudadDestino;
        public string CiudadDestino { get => _ciudadDestino; set => SetProperty(ref _ciudadDestino, value); }

        private string _paisDestino;
        public string PaisDestino { get => _paisDestino; set => SetProperty(ref _paisDestino, value); }

        private string _mensaje;
        public string Mensaje { get => _mensaje; set => SetProperty(ref _mensaje, value); }

        public ICommand NuevoCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand RefrescarCommand { get; }

        public PedidosViewModel()
        {
            NuevoCommand = new RelayCommand(_ => Limpiar());
            GuardarCommand = new RelayCommand(_ => Guardar());
            EliminarCommand = new RelayCommand(_ => Eliminar(), _ => Id > 0);
            RefrescarCommand = new RelayCommand(_ => Cargar());

            Ejecutar(() =>
            {
                foreach (var c in _catalogo.Clientes()) Clientes.Add(c);
                foreach (var e in _catalogo.Empleados()) Empleados.Add(e);
            });
            Cargar();
        }

        private void Cargar()
        {
            Ejecutar(() =>
            {
                Lista.Clear();
                foreach (var p in _repo.Listar()) Lista.Add(p);
                Limpiar();
            });
        }

        private void CargarFormulario(Pedido p)
        {
            Id = p.IdPedido;
            ClienteSeleccionado = Clientes.FirstOrDefault(c => c.Id == p.IdCliente);
            EmpleadoSeleccionado = Empleados.FirstOrDefault(e => e.Id == p.IdEmpleado);
            FechaPedido = p.FechaPedido;
            FechaEntrega = p.FechaEntrega;
            FechaEnvio = p.FechaEnvio;
            Flete = p.Flete;
            Destinatario = p.Destinatario;
            CiudadDestino = p.CiudadDestino;
            PaisDestino = p.PaisDestino;

            Detalle.Clear();
            Ejecutar(() =>
            {
                foreach (var d in _repo.ObtenerDetalle(p.IdPedido)) Detalle.Add(d);
            });
        }

        private void Limpiar()
        {
            Seleccionado = null;
            Id = 0;
            ClienteSeleccionado = null;
            EmpleadoSeleccionado = null;
            FechaPedido = DateTime.Today;
            FechaEntrega = null;
            FechaEnvio = null;
            Flete = 0;
            Destinatario = CiudadDestino = PaisDestino = string.Empty;
            Detalle.Clear();
            Mensaje = string.Empty;
        }

        private void Guardar()
        {
            if (ClienteSeleccionado == null)
            {
                Mensaje = "Seleccione un cliente.";
                return;
            }

            Ejecutar(() =>
            {
                var p = new Pedido
                {
                    IdPedido = Id,
                    IdCliente = ClienteSeleccionado?.Id,
                    IdEmpleado = EmpleadoSeleccionado?.Id,
                    FechaPedido = FechaPedido,
                    FechaEntrega = FechaEntrega,
                    FechaEnvio = FechaEnvio,
                    Flete = Flete,
                    Destinatario = Destinatario,
                    CiudadDestino = CiudadDestino,
                    PaisDestino = PaisDestino
                };

                if (Id == 0)
                {
                    int nuevoId = _repo.Insertar(p);
                    Mensaje = $"Pedido creado (Id {nuevoId}).";
                }
                else
                {
                    _repo.Actualizar(p);
                    Mensaje = "Pedido actualizado.";
                }
                Cargar();
            });
        }

        private void Eliminar()
        {
            if (Id == 0) return;
            if (MessageBox.Show($"Eliminar el pedido #{Id} y su detalle?", "Confirmar",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            Ejecutar(() =>
            {
                _repo.Eliminar(Id);
                Mensaje = "Pedido eliminado.";
                Cargar();
            });
        }

        private void Ejecutar(Action accion)
        {
            try { accion(); }
            catch (Exception ex) { Mensaje = "Error: " + ex.Message; }
        }
    }
}
