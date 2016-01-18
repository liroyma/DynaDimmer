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

namespace Dynadimmer.Views.UnitID
{
    /// <summary>
    /// Interaction logic for UnitIDView.xaml
    /// </summary>
    public partial class UnitIDView : UserControl
    {
        public UnitIDModel Model { get; set; }

        public UnitIDView()
        {
            Model = new UnitIDModel();
            InitializeComponent();
            this.DataContext = Model;
        }
    }
}
