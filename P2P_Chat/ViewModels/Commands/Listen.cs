﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace P2P_Chat.ViewModels.Commands
{
    internal class Listen : ICommand
    {
        
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public MainViewModel Parent { get; set; }

        public Listen(MainViewModel parent)
        {
            this.Parent = parent;
        }

        public bool CanExecute(object? parameter)
        {
            //if (parameter != null)
            //{
            //    var s = parameter as string;
            //    s = s.Trim();
            //    if(string.IsNullOrEmpty(s))
            //    {
            //        return false;
            //    }
            //    return true;

            //}
            return true;
        }

        public void Execute(object? parameter)
        {
            Parent.startListening();
        }
    }
}
