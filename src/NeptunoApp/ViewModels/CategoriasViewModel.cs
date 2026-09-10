using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NeptunoApp.Data;
using NeptunoApp.Models;

namespace NeptunoApp.ViewModels;

/// <summary>Mantenimiento (CRUD) de Categorias.</summary>
public partial class CategoriasViewModel : ObservableObject
{
    private readonly ICategoriaRepository _repo;

    public ObservableCollection<Categoria> Items { get; } = new();

    [ObservableProperty] private Categoria? seleccionada;
    [ObservableProperty] private int categoriaID;
    [ObservableProperty] private string nombreCategoria = string.Empty;
    [ObservableProperty] private string? descripcion;
    [ObservableProperty] private string? mensaje;
    [ObservableProperty] private bool isBusy;

    public bool EsEdicion => CategoriaID != 0;

    public CategoriasViewModel(ICategoriaRepository repo)
    {
        _repo = repo;
        _ = CargarAsync();
    }

    partial void OnSeleccionadaChanged(Categoria? value)
    {
        if (value is null) return;
        CategoriaID = value.CategoriaID;
        NombreCategoria = value.NombreCategoria;
        Descripcion = value.Descripcion;
        OnPropertyChanged(nameof(EsEdicion));
    }

    partial void OnCategoriaIDChanged(int value) => OnPropertyChanged(nameof(EsEdicion));

    [RelayCommand]
    private async Task CargarAsync()
    {
        IsBusy = true;
        Mensaje = null;
        try
        {
            var datos = await _repo.ListarTodasAsync();
            Items.Clear();
            foreach (var c in datos) Items.Add(c);
            Nuevo();
        }
        catch (Exception ex) { Mensaje = $"Error al cargar: {ex.Message}"; }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private void Nuevo()
    {
        Seleccionada = null;
        CategoriaID = 0;
        NombreCategoria = string.Empty;
        Descripcion = null;
        Mensaje = null;
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(NombreCategoria))
        {
            Mensaje = "El nombre de la categoria es obligatorio.";
            return;
        }

        try
        {
            var c = new Categoria
            {
                CategoriaID = CategoriaID,
                NombreCategoria = NombreCategoria.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(Descripcion) ? null : Descripcion.Trim()
            };

            if (CategoriaID == 0)
            {
                var id = await _repo.CrearAsync(c);
                Mensaje = $"Categoria creada (ID {id}).";
            }
            else
            {
                await _repo.ActualizarAsync(c);
                Mensaje = "Categoria actualizada.";
            }
            await CargarAsync();
        }
        catch (Exception ex) { Mensaje = $"No se pudo guardar: {ex.Message}"; }
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (CategoriaID == 0) return;
        if (MessageBox.Show($"¿Eliminar la categoria \"{NombreCategoria}\"?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

        try
        {
            await _repo.EliminarAsync(CategoriaID);
            Mensaje = "Categoria eliminada.";
            await CargarAsync();
        }
        catch (Exception ex) { Mensaje = $"No se pudo eliminar: {ex.Message}"; }
    }
}
