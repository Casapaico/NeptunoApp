using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NeptunoApp.Data;
using NeptunoApp.Models;

namespace NeptunoApp.ViewModels;

/// <summary>Mantenimiento (CRUD) de Productos.</summary>
public partial class ProductosViewModel : ObservableObject
{
    private readonly IProductoRepository _repo;
    private readonly ICatalogoRepository _catalogo;

    public ObservableCollection<Producto> Items { get; } = new();
    public ObservableCollection<OpcionCombo> Categorias { get; } = new();
    public ObservableCollection<OpcionCombo> Proveedores { get; } = new();

    [ObservableProperty] private Producto? seleccionado;

    [ObservableProperty] private int productoID;
    [ObservableProperty] private string nombreProducto = string.Empty;
    [ObservableProperty] private OpcionCombo? categoriaSeleccionada;
    [ObservableProperty] private OpcionCombo? proveedorSeleccionado;
    [ObservableProperty] private string? cantidadPorUnidad;
    [ObservableProperty] private decimal precioUnidad;
    [ObservableProperty] private short unidadesEnExistencia;
    [ObservableProperty] private short unidadesEnPedido;
    [ObservableProperty] private short nivelDeReorden;
    [ObservableProperty] private bool descontinuado;

    [ObservableProperty] private string? mensaje;
    [ObservableProperty] private bool isBusy;

    public ProductosViewModel(IProductoRepository repo, ICatalogoRepository catalogo)
    {
        _repo = repo;
        _catalogo = catalogo;
        _ = InicializarAsync();
    }

    private async Task InicializarAsync()
    {
        try
        {
            foreach (var c in await _catalogo.CategoriasAsync()) Categorias.Add(c);
            foreach (var p in await _catalogo.ProveedoresAsync()) Proveedores.Add(p);
        }
        catch (Exception ex) { Mensaje = $"Error al cargar catalogos: {ex.Message}"; }
        await CargarAsync();
    }

    partial void OnSeleccionadoChanged(Producto? value)
    {
        if (value is null) return;
        ProductoID = value.ProductoID;
        NombreProducto = value.NombreProducto;
        CategoriaSeleccionada = Categorias.FirstOrDefault(c => c.Id == value.CategoriaID);
        ProveedorSeleccionado = Proveedores.FirstOrDefault(p => p.Id == value.ProveedorID);
        CantidadPorUnidad = value.CantidadPorUnidad;
        PrecioUnidad = value.PrecioUnidad;
        UnidadesEnExistencia = value.UnidadesEnExistencia;
        UnidadesEnPedido = value.UnidadesEnPedido;
        NivelDeReorden = value.NivelDeReorden;
        Descontinuado = value.Descontinuado;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        IsBusy = true;
        Mensaje = null;
        try
        {
            var datos = await _repo.ListarTodasAsync();
            Items.Clear();
            foreach (var p in datos) Items.Add(p);
            Nuevo();
        }
        catch (Exception ex) { Mensaje = $"Error al cargar: {ex.Message}"; }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private void Nuevo()
    {
        Seleccionado = null;
        ProductoID = 0;
        NombreProducto = string.Empty;
        CategoriaSeleccionada = null;
        ProveedorSeleccionado = null;
        CantidadPorUnidad = null;
        PrecioUnidad = 0;
        UnidadesEnExistencia = UnidadesEnPedido = NivelDeReorden = 0;
        Descontinuado = false;
        Mensaje = null;
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(NombreProducto))
        {
            Mensaje = "El nombre del producto es obligatorio.";
            return;
        }

        try
        {
            var p = new Producto
            {
                ProductoID = ProductoID,
                NombreProducto = NombreProducto.Trim(),
                CategoriaID = CategoriaSeleccionada?.Id,
                ProveedorID = ProveedorSeleccionado?.Id,
                CantidadPorUnidad = CantidadPorUnidad,
                PrecioUnidad = PrecioUnidad,
                UnidadesEnExistencia = UnidadesEnExistencia,
                UnidadesEnPedido = UnidadesEnPedido,
                NivelDeReorden = NivelDeReorden,
                Descontinuado = Descontinuado
            };

            if (ProductoID == 0)
            {
                var id = await _repo.CrearAsync(p);
                Mensaje = $"Producto creado (ID {id}).";
            }
            else
            {
                await _repo.ActualizarAsync(p);
                Mensaje = "Producto actualizado.";
            }
            await CargarAsync();
        }
        catch (Exception ex) { Mensaje = $"No se pudo guardar: {ex.Message}"; }
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (ProductoID == 0) return;
        if (MessageBox.Show($"¿Eliminar el producto \"{NombreProducto}\"?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

        try
        {
            await _repo.EliminarAsync(ProductoID);
            Mensaje = "Producto eliminado.";
            await CargarAsync();
        }
        catch (Exception ex) { Mensaje = $"No se pudo eliminar: {ex.Message}"; }
    }
}
