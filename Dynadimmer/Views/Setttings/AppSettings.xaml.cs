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
        AppSettingsModel model;
        public AppSettings()
        {
            model = new AppSettingsModel();
            InitializeComponent();
            this.DataContext = model;
        }

        private void TextBox_Error(object sender, ValidationErrorEventArgs e)
        {
            model.PriceAddEnable = false;
        }
    }
}
