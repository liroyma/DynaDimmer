using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Dynadimmer.Views.Setttings
{
    /// <summary>
    /// Interaction logic for AppSettings.xaml
    /// </summary>
    public partial class AppSettings : Window
    {
        public AppSettingsModel Model { get; private set; }
        public AppSettings()
        {
            Model = new AppSettingsModel();
            InitializeComponent();
            this.DataContext = Model;
        }

        private void TextBox_Error(object sender, ValidationErrorEventArgs e)
        {
            Model.PriceAddEnable = false;
        }

        private void win_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            e.Cancel = true;
        }
        
    }
}
