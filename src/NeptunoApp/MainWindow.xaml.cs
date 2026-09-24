using System.Configuration;
using System.Windows;
using NeptunoApp.Data;
using NeptunoApp.ViewModels;

namespace NeptunoApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var cs = ConfigurationManager.ConnectionStrings["NeptunoDB"].ConnectionString;

        var vm = new MainViewModel(
            new ProductoRepository(cs),
            new CategoriaRepository(cs),
            new ProveedorRepository(cs),
            new PedidoRepository(cs),
            new ReporteRepository(cs),
            new CatalogoRepository(cs));

        DataContext = vm;
    }
}
