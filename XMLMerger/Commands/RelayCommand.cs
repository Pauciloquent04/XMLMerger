using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XMLMerger.Commands
{
    public class RelayCommand : ICommand
    {
        private Action<object> executeMethod;
        private Func<object, bool> canexecuteMethod;

        public RelayCommand(Action<object> executeMethod, Func<object, bool> canexecuteMethod = null)
        {
            this.executeMethod = executeMethod;
            this.canexecuteMethod = canexecuteMethod;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove {  CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return this.canexecuteMethod == null || this.canexecuteMethod(parameter);
        }

        public void Execute(object parameter)
        {
            this.executeMethod(parameter);
        }
    }
}
