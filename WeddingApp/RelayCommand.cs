using System;
using System.Windows.Input;

namespace WeddingApp
{

    /// <summary>
    /// RelayCommand
    /// 
    /// General purpose command implementation wrapper. This is an alternative 
    /// to multiple command classes, it is a single class that encapsulates different
    /// business logic using delegates accepted as constructor arguments.
    /// 
    /// Courtesy of Jim Kniest
    /// http://stackoverflow.com/questions/32725973/wpf-mvvm-treeview-commands-enabled-based-on-selected-item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelayCommand<T> : ICommand
    {
        private static bool CanExecute(T paramz)
        {
            return true;
        }

        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        /// <summary>
        /// Relay Command
        ///
        /// Stores the Action to be executed in the instance field variable. Also Stores the
        /// information about IF it canexecute in the instance field variable. These executing
        /// commands can be sent from other methods in other classes. Hence the lambda expressions.
        /// Tries to be as generic as possible T type as parameter.
        /// </summary>
        /// <param name="execute">Holds the method body about what it does when it executes</param>
        /// <param name="canExecute">Holds the method body conditions about what needs to happen for the ACTION
        /// Execute to execute. If it fails it cannot execute. </param>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute ?? CanExecute;

        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(TranslateParameter(parameter));
        }

        // allows for constant updating if the event can execute or not.
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;

            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            _execute(TranslateParameter(parameter));
        }

        private T TranslateParameter(object parameter)
        {
            T value = default(T);
            if (parameter != null && typeof(T).IsEnum)
                value = (T)Enum.Parse(typeof(T), (string)parameter);
            else
                value = (T)parameter;
            return value;
        }

        public void RaiseCanExecuteChanged()
        {
            // we should not have to reevaluate every can execute.  
            // but since there are too many places in product code to verify
            // we will settle for all or nothing.
            CommandManager.InvalidateRequerySuggested();
        }
    }


    /// <summary>
    /// Class is based on two delegates; one for executing the command and another for returning the validity of the command.
    /// The non-generic version is just a special case for the first, in case the command has no parameter.
    /// </summary>
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute, Func<bool> canExecute = null)
            : base(obj => execute(),
                (canExecute == null ?
                null : new Func<object, bool>(obj => canExecute())))
        {

        }
    }
}