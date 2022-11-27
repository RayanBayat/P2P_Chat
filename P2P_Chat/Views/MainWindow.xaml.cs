using P2P_Chat.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace P2P_Chat.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            this.DataContext = mainViewModel;
            Closing += mainViewModel.OnWindowClosing;

        }


        private void key_KeyDown(object sender, KeyEventArgs e)
        {


            if (sendbtn.IsEnabled)
            {

            

            if (e.Key == Key.Return)
            {
                ButtonAutomationPeer peer = new ButtonAutomationPeer(sendbtn);
                IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                invokeProv.Invoke();

            }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Statusupdater(object sender, DataTransferEventArgs e)
        {
            var label = sender as Label;
          //  something.Background = new SolidColorBrush(Colors.White);
            if (label.Content.ToString() == "Listening")
            {
               
            }
            MessageBox.Show(label.Content.ToString());
        }
    }
}
