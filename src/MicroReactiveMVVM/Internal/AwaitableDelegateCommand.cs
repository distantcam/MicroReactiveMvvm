using System;
using System.Threading.Tasks;

namespace MicroReactiveMVVM
{
    internal class AwaitableDelegateCommand : AwaitableDelegateCommand<object>, IAsyncCommand
    {
        public AwaitableDelegateCommand(Func<object, Task> executeMethod, Func<object, bool>? canExecuteMethod = null) : base(executeMethod, canExecuteMethod)
        {
        }
    }

    internal class AwaitableDelegateCommand<T> : AbstractCommand<T>, IAsyncCommand<T>
    {
        private readonly Func<T, Task> executeMethod;

        public AwaitableDelegateCommand(Func<T, Task> executeMethod, Func<T, bool>? canExecuteMethod = null) : base(canExecuteMethod)
        {
            this.executeMethod = executeMethod ??
                throw new ArgumentNullException(nameof(executeMethod), $"{nameof(executeMethod)} is null.");
        }

        public override async void Execute(T parameter) => await ExecuteAsync(parameter);

        public async Task ExecuteAsync(T parameter)
        {
            using (StartExecuting())
                await executeMethod(parameter);
        }
    }
}
