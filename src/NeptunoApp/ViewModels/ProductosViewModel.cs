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
    /// <summary>Mantenimiento (CRUD) de Productos usando procedimientos almacenados.</summary>
    public class ProductosViewModel : ViewModelBase
    {
        private readonly ProductoRepository _repo = new ProductoRepository();
        private readonly CatalogoRepository _catalogo = new CatalogoRepository();

        public ObservableCollection<Producto> Lista { get; } = new ObservableCollection<Producto>();
        public ObservableCollection<ItemCombo> Categorias { get; } = new ObservableCollection<ItemCombo>();
        public ObservableCollection<ItemCombo> Proveedores { get; } = new ObservableCollection<ItemCombo>();

        private Producto _seleccionado;
        public Producto Seleccionado
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

        private string _nombreProducto;
        public string NombreProducto { get => _nombreProducto; set => SetProperty(ref _nombreProducto, value); }

        private ItemCombo _categoriaSeleccionada;
        public ItemCombo CategoriaSeleccionada { get => _categoriaSeleccionada; set => SetProperty(ref _categoriaSeleccionada, value); }

        private ItemCombo _proveedorSeleccionado;
        public ItemCombo ProveedorSeleccionado { get => _proveedorSeleccionado; set => SetProperty(ref _proveedorSeleccionado, value); }

        private string _cantidadPorUnidad;
        public string CantidadPorUnidad { get => _cantidadPorUnidad; set => SetProperty(ref _cantidadPorUnidad, value); }

        private decimal _precioUnidad;
        public decimal PrecioUnidad { get => _precioUnidad; set => SetProperty(ref _precioUnidad, value); }

        private short _unidadesEnExistencia;
        public short UnidadesEnExistencia { get => _unidadesEnExistencia; set => SetProperty(ref _unidadesEnExistencia, value); }

        private short _unidadesEnPedido;
        public short UnidadesEnPedido { get => _unidadesEnPedido; set => SetProperty(ref _unidadesEnPedido, value); }

        private short _nivelNuevoPedido;
        public short NivelNuevoPedido { get => _nivelNuevoPedido; set => SetProperty(ref _nivelNuevoPedido, value); }

        private bool _suspendido;
        public bool Suspendido { get => _suspendido; set => SetProperty(ref _suspendido, value); }

        private string _mensaje;
        public string Mensaje { get => _mensaje; set => SetProperty(ref _mensaje, value); }

        public ICommand NuevoCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand RefrescarCommand { get; }

        public ProductosViewModel()
        {
            NuevoCommand = new RelayCommand(_ => Limpiar());
            GuardarCommand = new RelayCommand(_ => Guardar());
            EliminarCommand = new RelayCommand(_ => Eliminar(), _ => Id > 0);
            RefrescarCommand = new RelayCommand(_ => Cargar());

            Ejecutar(() =>
            {
                foreach (var c in _catalogo.Categorias()) Categorias.Add(c);
                foreach (var p in _catalogo.Proveedores()) Proveedores.Add(p);
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

        private void CargarFormulario(Producto p)
        {
            Id = p.IdProducto;
            NombreProducto = p.NombreProducto;
            CategoriaSeleccionada = Categorias.FirstOrDefault(c => c.Id == p.IdCategoria);
            ProveedorSeleccionado = Proveedores.FirstOrDefault(v => v.Id == p.IdProveedor);
            CantidadPorUnidad = p.CantidadPorUnidad;
            PrecioUnidad = p.PrecioUnidad;
            UnidadesEnExistencia = p.UnidadesEnExistencia;
            UnidadesEnPedido = p.UnidadesEnPedido;
            NivelNuevoPedido = p.NivelNuevoPedido;
            Suspendido = p.Suspendido;
        }

        private void Limpiar()
        {
            Seleccionado = null;
            Id = 0;
            NombreProducto = string.Empty;
            CategoriaSeleccionada = null;
            ProveedorSeleccionado = null;
            CantidadPorUnidad = string.Empty;
            PrecioUnidad = 0;
            UnidadesEnExistencia = UnidadesEnPedido = NivelNuevoPedido = 0;
            Suspendido = false;
            Mensaje = string.Empty;
        }

        private void Guardar()
        {
            if (string.IsNullOrWhiteSpace(NombreProducto))
            {
                Mensaje = "El nombre del producto es obligatorio.";
                return;
            }

            Ejecutar(() =>
            {
                var p = new Producto
                {
                    IdProducto = Id,
                    NombreProducto = NombreProducto.Trim(),
                    IdCategoria = CategoriaSeleccionada?.Id,
                    IdProveedor = ProveedorSeleccionado?.Id,
                    CantidadPorUnidad = CantidadPorUnidad,
                    PrecioUnidad = PrecioUnidad,
                    UnidadesEnExistencia = UnidadesEnExistencia,
                    UnidadesEnPedido = UnidadesEnPedido,
                    NivelNuevoPedido = NivelNuevoPedido,
                    Suspendido = Suspendido
                };

                if (Id == 0)
                {
                    int nuevoId = _repo.Insertar(p);
                    Mensaje = $"Producto creado (Id {nuevoId}).";
                }
                else
                {
                    _repo.Actualizar(p);
                    Mensaje = "Producto actualizado.";
                }
                Cargar();
            });
        }

        private void Eliminar()
        {
            if (Id == 0) return;
            if (MessageBox.Show($"Eliminar el producto '{NombreProducto}'?", "Confirmar",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            Ejecutar(() =>
            {
                _repo.Eliminar(Id);
                Mensaje = "Producto eliminado.";
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
