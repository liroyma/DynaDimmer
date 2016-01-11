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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dynadimmer.Views.Config
{
    /// <summary>
    /// Interaction logic for ConfigView.xaml
    /// </summary>
    public partial class ConfigView : UserControl
    {
        public ConfigModel Model { get; private set; }
        public ConfigView()
        {
            Model = new ConfigModel();
            InitializeComponent();
            this.DataContext = Model;
        }
    }
}
