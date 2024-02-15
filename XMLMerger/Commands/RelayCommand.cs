using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XMLMerger.Commands
{
    internal class RelayCommand : ICommand
    {
        Action<object> _executeMethod;
        Func<object, bool> _canexecuteMethod;

        public RelayCommand(Action<object> executeMethod, Func<object, bool> canexecuteMethod = null)
        {
            _executeMethod = executeMethod;
            _canexecuteMethod = canexecuteMethod;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove {  CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if(_canexecuteMethod != null) { return _canexecuteMethod(parameter); }
            else { return false; }
        }

        public void Execute(object parameter)
        {
            _executeMethod(parameter);
        }
    }
}
