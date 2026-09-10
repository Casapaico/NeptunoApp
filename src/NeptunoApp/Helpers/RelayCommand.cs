using System;
using System.Windows.Input;

namespace NeptunoApp.Helpers
{
    /// <summary>ICommand generico para enlazar acciones de los ViewModels a los botones.</summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _ejecutar;
        private readonly Predicate<object> _puedeEjecutar;

        public RelayCommand(Action<object> ejecutar, Predicate<object> puedeEjecutar = null)
        {
            _ejecutar = ejecutar ?? throw new ArgumentNullException(nameof(ejecutar));
            _puedeEjecutar = puedeEjecutar;
        }

        public bool CanExecute(object parameter) => _puedeEjecutar == null || _puedeEjecutar(parameter);

        public void Execute(object parameter) => _ejecutar(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
