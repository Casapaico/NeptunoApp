using System.Windows.Input;
using NeptunoApp.Helpers;

namespace NeptunoApp.ViewModels
{
    /// <summary>Menu principal: navega entre las vistas de mantenimiento y reportes.</summary>
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public ICommand VerProductosCommand { get; }
        public ICommand VerCategoriasCommand { get; }
        public ICommand VerProveedoresCommand { get; }
        public ICommand VerPedidosCommand { get; }
        public ICommand VerReportesCommand { get; }

        public MainViewModel()
        {
            VerProductosCommand   = new RelayCommand(_ => CurrentViewModel = new ProductosViewModel());
            VerCategoriasCommand  = new RelayCommand(_ => CurrentViewModel = new CategoriasViewModel());
            VerProveedoresCommand = new RelayCommand(_ => CurrentViewModel = new ProveedoresViewModel());
            VerPedidosCommand     = new RelayCommand(_ => CurrentViewModel = new PedidosViewModel());
            VerReportesCommand    = new RelayCommand(_ => CurrentViewModel = new ReportesViewModel());

            CurrentViewModel = new ProductosViewModel();
        }
    }
}
