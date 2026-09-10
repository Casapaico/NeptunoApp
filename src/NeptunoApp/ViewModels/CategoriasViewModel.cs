using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using NeptunoApp.Data;
using NeptunoApp.Helpers;
using NeptunoApp.Models;

namespace NeptunoApp.ViewModels
{
    /// <summary>Mantenimiento (CRUD) de Categorias usando procedimientos almacenados.</summary>
    public class CategoriasViewModel : ViewModelBase
    {
        private readonly CategoriaRepository _repo = new CategoriaRepository();

        public ObservableCollection<Categoria> Lista { get; } = new ObservableCollection<Categoria>();

        private Categoria _seleccionada;
        public Categoria Seleccionada
        {
            get => _seleccionada;
            set
            {
                if (SetProperty(ref _seleccionada, value) && value != null)
                {
                    IdCategoria = value.IdCategoria;
                    NombreCategoria = value.NombreCategoria;
                    Descripcion = value.Descripcion;
                }
            }
        }

        private int _idCategoria;
        public int IdCategoria { get => _idCategoria; set => SetProperty(ref _idCategoria, value); }

        private string _nombreCategoria;
        public string NombreCategoria { get => _nombreCategoria; set => SetProperty(ref _nombreCategoria, value); }

        private string _descripcion;
        public string Descripcion { get => _descripcion; set => SetProperty(ref _descripcion, value); }

        private string _mensaje;
        public string Mensaje { get => _mensaje; set => SetProperty(ref _mensaje, value); }

        public ICommand NuevoCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand RefrescarCommand { get; }

        public CategoriasViewModel()
        {
            NuevoCommand = new RelayCommand(_ => Limpiar());
            GuardarCommand = new RelayCommand(_ => Guardar());
            EliminarCommand = new RelayCommand(_ => Eliminar(), _ => IdCategoria > 0);
            RefrescarCommand = new RelayCommand(_ => Cargar());
            Cargar();
        }

        private void Cargar()
        {
            Ejecutar(() =>
            {
                Lista.Clear();
                foreach (var c in _repo.Listar()) Lista.Add(c);
                Limpiar();
            });
        }

        private void Limpiar()
        {
            Seleccionada = null;
            IdCategoria = 0;
            NombreCategoria = string.Empty;
            Descripcion = string.Empty;
            Mensaje = string.Empty;
        }

        private void Guardar()
        {
            if (string.IsNullOrWhiteSpace(NombreCategoria))
            {
                Mensaje = "El nombre de la categoria es obligatorio.";
                return;
            }

            Ejecutar(() =>
            {
                var c = new Categoria
                {
                    IdCategoria = IdCategoria,
                    NombreCategoria = NombreCategoria.Trim(),
                    Descripcion = string.IsNullOrWhiteSpace(Descripcion) ? null : Descripcion.Trim()
                };

                if (IdCategoria == 0)
                {
                    int nuevoId = _repo.Insertar(c);
                    Mensaje = $"Categoria creada (Id {nuevoId}).";
                }
                else
                {
                    _repo.Actualizar(c);
                    Mensaje = "Categoria actualizada.";
                }
                Cargar();
            });
        }

        private void Eliminar()
        {
            if (IdCategoria == 0) return;
            if (MessageBox.Show($"Eliminar la categoria '{NombreCategoria}'?", "Confirmar",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            Ejecutar(() =>
            {
                _repo.Eliminar(IdCategoria);
                Mensaje = "Categoria eliminada.";
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
