using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dynadimmer.Models
{
    public class MyCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public event EventHandler CommandSent;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            CommandSent(parameter, null);
        }

        private void Check()
        {
            CanExecuteChanged(null,null);
        }
        
    }
}
