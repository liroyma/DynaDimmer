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
        public CalcWindow()
        {
            Model = new CalcModel();
            InitializeComponent();
            this.DataContext = Model;
        }
        
        public void SetList(List<LampModel> list,string unitid)
        {
            Model.SetLampListAndCalc(list, unitid);
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    Model.Read(file);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

      
    }
}
