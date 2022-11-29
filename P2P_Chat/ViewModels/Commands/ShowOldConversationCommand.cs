using P2P_Chat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace P2P_Chat.ViewModels.Commands
{
    internal class ShowOldConversationCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        private MainViewModel _parent;

        public MainViewModel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public ShowOldConversationCommand(MainViewModel parent)
        {
            this.Parent = parent;
        }

        public bool CanExecute(object? parameter)
        {

            return true;
        }

        public void Execute(object? parameter)
        {
            List<Message> obj = parameter as List<Message>;
            MessageBox.Show(obj.ToString());
            MessageBox.Show("hello");
            this.Parent.ShowOldConversationMethod(obj);
        }
    }
}
