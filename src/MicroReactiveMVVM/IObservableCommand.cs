using System;

namespace MicroReactiveMVVM
{
    public interface IObservableCommand : System.Windows.Input.ICommand, IRaiseCanExecuteChanged, IDisposable
    {
    }
}
