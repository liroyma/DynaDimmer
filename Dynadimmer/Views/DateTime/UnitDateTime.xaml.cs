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
using Dynadimmer.Models;

namespace Dynadimmer.Views.DateTime
{
    /// <summary>
    /// Interaction logic for UnitDateTime.xaml
    /// </summary>
    public partial class UnitDateTime : UserControl
    {
        public UnitProperty Model { get; private set; }
        public UnitDateTime()
        {
            Model = new UnitDateTimeModel();
            InitializeComponent();
            this.DataContext = Model;
        }
    }
}
