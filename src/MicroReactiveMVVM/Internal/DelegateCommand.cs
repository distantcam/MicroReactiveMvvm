using System;

namespace MicroReactiveMVVM
{
    internal class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action<object> executeMethod, Func<object, bool>? canExecuteMethod = null) :
            base(executeMethod, canExecuteMethod)
        {
        }
    }

    internal class DelegateCommand<T> : AbstractCommand<T>
    {
        private readonly Action<T> executeMethod;

        public DelegateCommand(Action<T> executeMethod, Func<T, bool>? canExecuteMethod = null) : base(canExecuteMethod)
        {
            this.executeMethod = executeMethod ??
                throw new ArgumentNullException(nameof(executeMethod), $"{nameof(executeMethod)} is null.");
        }

        public override void Execute(T parameter)
        {
            using (StartExecuting())
                executeMethod(parameter);
        }
    }
}
