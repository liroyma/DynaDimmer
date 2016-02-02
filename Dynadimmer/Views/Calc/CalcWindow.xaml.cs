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
using Dynadimmer.Views.LampItem;
using Dynadimmer.Views.Schedulers;

namespace Dynadimmer.Views.Calc
{
    /// <summary>
    /// Interaction logic for CalcWindow.xaml
    /// </summary>
    public partial class CalcWindow : Window
    {
        public CalcModel Model { get; private set; }
        public CalcWindow(List<LampModel> list)
        {
            Model = new CalcModel();
            InitializeComponent();
            this.DataContext = Model;
            Model.SetLampListAndCalc(list);
        }

    }
}
