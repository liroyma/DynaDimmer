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

namespace Dynadimmer.Views.FileLoad
{
    /// <summary>
    /// Interaction logic for FileLoadView.xaml
    /// </summary>
    public partial class FileLoadView : UserControl
    {
        public FileLoadModel Model { get; private set; }
        public FileLoadView()
        {
            Model = new FileLoadModel();
            InitializeComponent();
            this.DataContext = Model;
        }
    }
}
