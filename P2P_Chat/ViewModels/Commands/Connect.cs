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
        public event EventHandler? CanExecuteChanged;
        private MainViewModel _parent;

        public MainViewModel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public Connect(MainViewModel parent)
        {
            this.Parent = parent;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Parent.establishConnection();
        }
    }
}
