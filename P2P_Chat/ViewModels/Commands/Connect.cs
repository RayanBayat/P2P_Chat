using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace P2P_Chat.ViewModels.Commands
{
    internal class Connect:ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public MainViewModel Parent { get; set; }

        public Connect(MainViewModel parent)
        {
            this.Parent = parent;
        }
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public bool CanExecute(object ? parameter)
        {
            //bool isconnected = true;
            //if (parameter is not null)
            //{
            //    isconnected = (bool)parameter;
            //}

            //return !isconnected;
            return true;
        }

        public void Execute(object? parameter)
        {
            Parent.establishConnection();
            
        }
    }
}
