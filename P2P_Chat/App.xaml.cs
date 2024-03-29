﻿using P2P_Chat.Models;
using P2P_Chat.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using P2P_Chat.Views;

namespace P2P_Chat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Main(Object Sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(new MainViewModel(new ConnectionHandler()));
            mainWindow.Title = "Message Sender";
            mainWindow.Show();
        }
    }
}
