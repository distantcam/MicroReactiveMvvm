using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MicroReactiveMVVM
{
    public static partial class CommandExtensions
    {
        public static void RaiseCanExecuteChanged(this ICommand command)
        {
            if (command is IRaiseCanExecuteChanged canExecuteChanged)
                canExecuteChanged.RaiseCanExecuteChanged();
        }

        public static IObservableCommand ToCommand(this IObservable<bool> canExecuteObservable, Func<object, Task> action) =>
            new ObservableCommand(canExecuteObservable, action);

        public static IObservableCommand ToCommand(this IObservable<PropertyChangedData<bool>> canExecuteObservable, Func<object, Task> action) =>
            new ObservableCommand(canExecuteObservable.Select(pc => pc.Value), action);

        public static IObservableCommand ToCommand(this IObservable<bool> canExecuteObservable, Action<object> action) =>
            new ObservableCommand(canExecuteObservable, p => { action(p); return Task.CompletedTask; });

        public static IObservableCommand ToCommand(this IObservable<PropertyChangedData<bool>> canExecuteObservable, Action<object> action) =>
            new ObservableCommand(canExecuteObservable.Select(pc => pc.Value), p => { action(p); return Task.CompletedTask; });

        public static IDisposable Execute<T>(this IObservable<T> observable, ICommand command) =>
            observable.Do(t => { if (command.CanExecute(t)) command.Execute(t); }).Subscribe();

        public static IDisposable Execute<T>(this IObservable<T> observable, ICommand<T> command) =>
            observable.Do(t => { if (command.CanExecute(t)) command.Execute(t); }).Subscribe();

        public static IDisposable ExecuteAsync<T>(this IObservable<T> observable, IAsyncCommand<T> command) =>
            observable.SelectMany(async t => { if (command.CanExecute(t)) await command.ExecuteAsync(t); return Unit.Default; }).Subscribe();
    }
}
