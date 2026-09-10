using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NeptunoApp.Helpers
{
    /// <summary>Base MVVM: implementa INotifyPropertyChanged.</summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string name = null)
        {
            if (Equals(campo, valor)) return false;
            campo = valor;
            OnPropertyChanged(name);
            return true;
        }
    }
}
