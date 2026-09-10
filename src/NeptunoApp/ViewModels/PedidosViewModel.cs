using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NeptunoApp.Data;
using NeptunoApp.Models;

namespace NeptunoApp.ViewModels;

/// <summary>Mantenimiento (CRUD) de Pedidos (cabecera) + detalle del pedido seleccionado.</summary>
public partial class PedidosViewModel : ObservableObject
{
    private readonly IPedidoRepository _repo;
    private readonly ICatalogoRepository _catalogo;

    public ObservableCollection<Pedido> Items { get; } = new();
    public ObservableCollection<DetallePedido> Detalle { get; } = new();
    public ObservableCollection<OpcionCombo> Clientes { get; } = new();
    public ObservableCollection<OpcionCombo> Empleados { get; } = new();
    public ObservableCollection<OpcionCombo> Transportistas { get; } = new();

    [ObservableProperty] private Pedido? seleccionado;

    [ObservableProperty] private int pedidoID;
    [ObservableProperty] private OpcionCombo? clienteSeleccionado;
    [ObservableProperty] private OpcionCombo? empleadoSeleccionado;
    [ObservableProperty] private OpcionCombo? transportistaSeleccionado;
    [ObservableProperty] private DateTime? fechaPedido = DateTime.Today;
    [ObservableProperty] private DateTime? fechaRequerida;
    [ObservableProperty] private DateTime? fechaEnvio;
    [ObservableProperty] private string? destinatario;
    [ObservableProperty] private string? ciudadDestino;
    [ObservableProperty] private string? paisDestino;

    [ObservableProperty] private string? mensaje;
    [ObservableProperty] private bool isBusy;

    public PedidosViewModel(IPedidoRepository repo, ICatalogoRepository catalogo)
    {
        _repo = repo;
        _catalogo = catalogo;
        _ = InicializarAsync();
    }

    private async Task InicializarAsync()
    {
        try
        {
            foreach (var c in await _catalogo.ClientesAsync()) Clientes.Add(c);
            foreach (var e in await _catalogo.EmpleadosAsync()) Empleados.Add(e);
            foreach (var t in await _catalogo.TransportistasAsync()) Transportistas.Add(t);
        }
        catch (Exception ex) { Mensaje = $"Error al cargar catalogos: {ex.Message}"; }
        await CargarAsync();
    }

    partial void OnSeleccionadoChanged(Pedido? value)
    {
        if (value is null) return;
        PedidoID = value.PedidoID;
        ClienteSeleccionado = Clientes.FirstOrDefault(c => c.Id == value.ClienteID);
        EmpleadoSeleccionado = Empleados.FirstOrDefault(e => e.Id == value.EmpleadoID);
        TransportistaSeleccionado = Transportistas.FirstOrDefault(t => t.Id == value.TransportistaID);
        FechaPedido = value.FechaPedido;
        FechaRequerida = value.FechaRequerida;
        FechaEnvio = value.FechaEnvio;
        Destinatario = value.Destinatario;
        CiudadDestino = value.CiudadDestino;
        PaisDestino = value.PaisDestino;
        _ = CargarDetalleAsync(value.PedidoID);
    }

    private async Task CargarDetalleAsync(int pedidoId)
    {
        Detalle.Clear();
        try
        {
            foreach (var d in await _repo.ObtenerDetalleAsync(pedidoId)) Detalle.Add(d);
        }
        catch (Exception ex) { Mensaje = $"Error al cargar el detalle: {ex.Message}"; }
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
        PedidoID = 0;
        ClienteSeleccionado = null;
        EmpleadoSeleccionado = null;
        TransportistaSeleccionado = null;
        FechaPedido = DateTime.Today;
        FechaRequerida = null;
        FechaEnvio = null;
        Destinatario = CiudadDestino = PaisDestino = null;
        Detalle.Clear();
        Mensaje = null;
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (ClienteSeleccionado is null)
        {
            Mensaje = "Seleccione un cliente.";
            return;
        }
        if (FechaPedido is null)
        {
            Mensaje = "La fecha del pedido es obligatoria.";
            return;
        }

        try
        {
            var p = new Pedido
            {
                PedidoID = PedidoID,
                ClienteID = ClienteSeleccionado?.Id,
                EmpleadoID = EmpleadoSeleccionado?.Id,
                TransportistaID = TransportistaSeleccionado?.Id,
                FechaPedido = FechaPedido,
                FechaRequerida = FechaRequerida,
                FechaEnvio = FechaEnvio,
                Destinatario = Destinatario,
                CiudadDestino = CiudadDestino,
                PaisDestino = PaisDestino
            };

            if (PedidoID == 0)
            {
                var id = await _repo.CrearAsync(p);
                Mensaje = $"Pedido creado (ID {id}).";
            }
            else
            {
                await _repo.ActualizarAsync(p);
                Mensaje = "Pedido actualizado.";
            }
            await CargarAsync();
        }
        catch (Exception ex) { Mensaje = $"No se pudo guardar: {ex.Message}"; }
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (PedidoID == 0) return;
        if (MessageBox.Show($"¿Eliminar el pedido #{PedidoID} y su detalle?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

        try
        {
            await _repo.EliminarAsync(PedidoID);
            Mensaje = "Pedido eliminado.";
            await CargarAsync();
        }
        catch (Exception ex) { Mensaje = $"No se pudo eliminar: {ex.Message}"; }
    }
}
