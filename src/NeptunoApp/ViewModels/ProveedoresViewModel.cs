using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NeptunoApp.Data;
using NeptunoApp.Models;

namespace NeptunoApp.ViewModels;

/// <summary>Mantenimiento (CRUD) de Proveedores + busqueda por nombreContacto y ciudad.</summary>
public partial class ProveedoresViewModel : ObservableObject
{
    private readonly IProveedorRepository _repo;

    public ObservableCollection<Proveedor> Items { get; } = new();

    [ObservableProperty] private Proveedor? seleccionado;

    [ObservableProperty] private int proveedorID;
    [ObservableProperty] private string companiaNombre = string.Empty;
    [ObservableProperty] private string? nombreContacto;
    [ObservableProperty] private string? cargoContacto;
    [ObservableProperty] private string? direccion;
    [ObservableProperty] private string? ciudad;
    [ObservableProperty] private string? codigoPostal;
    [ObservableProperty] private string? pais;
    [ObservableProperty] private string? telefono;
    [ObservableProperty] private string? fax;

    [ObservableProperty] private string? filtroContacto;
    [ObservableProperty] private string? filtroCiudad;

    [ObservableProperty] private string? mensaje;
    [ObservableProperty] private bool isBusy;

    public ProveedoresViewModel(IProveedorRepository repo)
    {
        _repo = repo;
        _ = CargarAsync();
    }

    partial void OnSeleccionadoChanged(Proveedor? value)
    {
        if (value is null) return;
        ProveedorID = value.ProveedorID;
        CompaniaNombre = value.CompaniaNombre;
        NombreContacto = value.NombreContacto;
        CargoContacto = value.CargoContacto;
        Direccion = value.Direccion;
        Ciudad = value.Ciudad;
        CodigoPostal = value.CodigoPostal;
        Pais = value.Pais;
        Telefono = value.Telefono;
        Fax = value.Fax;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        IsBusy = true;
        Mensaje = null;
        FiltroContacto = null;
        FiltroCiudad = null;
        try
        {
            Reemplazar(await _repo.ListarTodasAsync());
            Nuevo();
        }
        catch (Exception ex) { Mensaje = $"Error al cargar: {ex.Message}"; }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task BuscarAsync()
    {
        Mensaje = null;
        try
        {
            Reemplazar(await _repo.BuscarPorContactoCiudadAsync(FiltroContacto, FiltroCiudad));
            Mensaje = $"{Items.Count} proveedor(es) encontrados.";
        }
        catch (Exception ex) { Mensaje = $"Error en la busqueda: {ex.Message}"; }
    }

    [RelayCommand]
    private void Nuevo()
    {
        Seleccionado = null;
        ProveedorID = 0;
        CompaniaNombre = string.Empty;
        NombreContacto = CargoContacto = Direccion = Ciudad = CodigoPostal = Pais = Telefono = Fax = null;
        Mensaje = null;
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(CompaniaNombre))
        {
            Mensaje = "El nombre de la compania es obligatorio.";
            return;
        }

        try
        {
            var p = new Proveedor
            {
                ProveedorID = ProveedorID,
                CompaniaNombre = CompaniaNombre.Trim(),
                NombreContacto = NombreContacto,
                CargoContacto = CargoContacto,
                Direccion = Direccion,
                Ciudad = Ciudad,
                CodigoPostal = CodigoPostal,
                Pais = Pais,
                Telefono = Telefono,
                Fax = Fax
            };

            if (ProveedorID == 0)
            {
                var id = await _repo.CrearAsync(p);
                Mensaje = $"Proveedor creado (ID {id}).";
            }
            else
            {
                await _repo.ActualizarAsync(p);
                Mensaje = "Proveedor actualizado.";
            }
            await CargarAsync();
        }
        catch (Exception ex) { Mensaje = $"No se pudo guardar: {ex.Message}"; }
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (ProveedorID == 0) return;
        if (MessageBox.Show($"¿Eliminar el proveedor \"{CompaniaNombre}\"?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

        try
        {
            await _repo.EliminarAsync(ProveedorID);
            Mensaje = "Proveedor eliminado.";
            await CargarAsync();
        }
        catch (Exception ex) { Mensaje = $"No se pudo eliminar: {ex.Message}"; }
    }

    private void Reemplazar(IEnumerable<Proveedor> datos)
    {
        Items.Clear();
        foreach (var p in datos) Items.Add(p);
    }
}
