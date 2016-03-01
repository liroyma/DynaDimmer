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

namespace Dynadimmer.Views.OnlineSaving
{
    /// <summary>
    /// Interaction logic for OnlineSavingView.xaml
    /// </summary>
    public partial class OnlineSavingView : UserControl
    {
        public OnlineSavingModel Model { get; set; }
        public OnlineSavingView()
        {
            Model = new OnlineSavingModel();
            InitializeComponent();
            this.DataContext = Model; 
        }
    }
}
