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

namespace Dynadimmer.Views.SummerWinnter
{
    /// <summary>
    /// Interaction logic for UnitSummerWinnterClock.xaml
    /// </summary>
    public partial class UnitSummerWinnterClock : UserControl
    {
        public UnitProperty Model { get; private set; }

        public UnitSummerWinnterClock()
        {
            Model = new UnitSummerWinnterClockModel();
            InitializeComponent();
            this.DataContext = Model;
        }
    }
}
