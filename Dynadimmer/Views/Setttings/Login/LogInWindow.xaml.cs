using Dynadimmer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dynadimmer.Views.Setttings.Login
{
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        public LoginModel Model { get; set; }
        public LogInWindow()
        {
            Model = new LoginModel();
            InitializeComponent();
            this.DataContext = Model;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            e.Cancel = true;
        }
        
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Model.Password = ((PasswordBox)sender).Password;
        }
    }
}
