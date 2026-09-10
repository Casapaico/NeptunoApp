using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using NeptunoApp.Data;
using NeptunoApp.Helpers;
using NeptunoApp.Models;

namespace NeptunoApp.ViewModels
{
    /// <summary>
    /// Mantenimiento (CRUD) de Proveedores + busqueda por nombreContacto y ciudad
    /// (procedimiento usp_Proveedores_Buscar).
    /// </summary>
    public class ProveedoresViewModel : ViewModelBase
    {
        private readonly ProveedorRepository _repo = new ProveedorRepository();

        public ObservableCollection<Proveedor> Lista { get; } = new ObservableCollection<Proveedor>();

        private Proveedor _seleccionado;
        public Proveedor Seleccionado
        {
            get => _seleccionado;
            set
            {
                if (SetProperty(ref _seleccionado, value) && value != null)
                    CargarFormulario(value);
            }
        }

        // --- Campos del formulario ---
        private int _id;
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        private string _nombreCompania;
        public string NombreCompania { get => _nombreCompania; set => SetProperty(ref _nombreCompania, value); }

        private string _nombreContacto;
        public string NombreContacto { get => _nombreContacto; set => SetProperty(ref _nombreContacto, value); }

        private string _cargoContacto;
        public string CargoContacto { get => _cargoContacto; set => SetProperty(ref _cargoContacto, value); }

        private string _direccion;
        public string Direccion { get => _direccion; set => SetProperty(ref _direccion, value); }

        private string _ciudad;
        public string Ciudad { get => _ciudad; set => SetProperty(ref _ciudad, value); }

        private string _region;
        public string Region { get => _region; set => SetProperty(ref _region, value); }

        private string _codPostal;
        public string CodPostal { get => _codPostal; set => SetProperty(ref _codPostal, value); }

        private string _pais;
        public string Pais { get => _pais; set => SetProperty(ref _pais, value); }

        private string _telefono;
        public string Telefono { get => _telefono; set => SetProperty(ref _telefono, value); }

        private string _fax;
        public string Fax { get => _fax; set => SetProperty(ref _fax, value); }

        // --- Filtros de busqueda ---
        private string _filtroContacto;
        public string FiltroContacto { get => _filtroContacto; set => SetProperty(ref _filtroContacto, value); }

        private string _filtroCiudad;
        public string FiltroCiudad { get => _filtroCiudad; set => SetProperty(ref _filtroCiudad, value); }

        private string _mensaje;
        public string Mensaje { get => _mensaje; set => SetProperty(ref _mensaje, value); }

        public ICommand NuevoCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand BuscarCommand { get; }
        public ICommand VerTodosCommand { get; }

        public ProveedoresViewModel()
        {
            NuevoCommand = new RelayCommand(_ => Limpiar());
            GuardarCommand = new RelayCommand(_ => Guardar());
            EliminarCommand = new RelayCommand(_ => Eliminar(), _ => Id > 0);
            BuscarCommand = new RelayCommand(_ => Buscar());
            VerTodosCommand = new RelayCommand(_ => Cargar());
            Cargar();
        }

        private void Cargar()
        {
            FiltroContacto = string.Empty;
            FiltroCiudad = string.Empty;
            Ejecutar(() => Reemplazar(_repo.Listar()));
        }

        private void Buscar()
        {
            Ejecutar(() =>
            {
                Reemplazar(_repo.Buscar(FiltroContacto, FiltroCiudad));
                Mensaje = $"{Lista.Count} proveedor(es) encontrados.";
            });
        }

        private void Reemplazar(System.Collections.Generic.IEnumerable<Proveedor> items)
        {
            Lista.Clear();
            foreach (var p in items) Lista.Add(p);
        }

        private void CargarFormulario(Proveedor p)
        {
            Id = p.IdProveedor;
            NombreCompania = p.NombreCompania;
            NombreContacto = p.NombreContacto;
            CargoContacto = p.CargoContacto;
            Direccion = p.Direccion;
            Ciudad = p.Ciudad;
            Region = p.Region;
            CodPostal = p.CodPostal;
            Pais = p.Pais;
            Telefono = p.Telefono;
            Fax = p.Fax;
        }

        private void Limpiar()
        {
            Seleccionado = null;
            Id = 0;
            NombreCompania = NombreContacto = CargoContacto = Direccion = Ciudad =
                Region = CodPostal = Pais = Telefono = Fax = string.Empty;
            Mensaje = string.Empty;
        }

        private void Guardar()
        {
            if (string.IsNullOrWhiteSpace(NombreCompania))
            {
                Mensaje = "El nombre de la compania es obligatorio.";
                return;
            }

            Ejecutar(() =>
            {
                var p = new Proveedor
                {
                    IdProveedor = Id,
                    NombreCompania = NombreCompania?.Trim(),
                    NombreContacto = NombreContacto,
                    CargoContacto = CargoContacto,
                    Direccion = Direccion,
                    Ciudad = Ciudad,
                    Region = Region,
                    CodPostal = CodPostal,
                    Pais = Pais,
                    Telefono = Telefono,
                    Fax = Fax
                };

                if (Id == 0)
                {
                    int nuevoId = _repo.Insertar(p);
                    Mensaje = $"Proveedor creado (Id {nuevoId}).";
                }
                else
                {
                    _repo.Actualizar(p);
                    Mensaje = "Proveedor actualizado.";
                }
                Cargar();
            });
        }

        private void Eliminar()
        {
            if (Id == 0) return;
            if (MessageBox.Show($"Eliminar el proveedor '{NombreCompania}'?", "Confirmar",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            Ejecutar(() =>
            {
                _repo.Eliminar(Id);
                Mensaje = "Proveedor eliminado.";
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
