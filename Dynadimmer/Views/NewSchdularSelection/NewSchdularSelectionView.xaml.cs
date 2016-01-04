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

namespace Dynadimmer.Views.NewSchdularSelection
{
    /// <summary>
    /// Interaction logic for NewSchdularSelectionView.xaml
    /// </summary>
    public partial class NewSchdularSelectionView : UserControl
    {
        public NewSchedularSelectionModel Model { get; private set; }

        public NewSchdularSelectionView()
        {
            Model = new NewSchedularSelectionModel();
            InitializeComponent();
            this.DataContext = Model;
        }
        
        public void SetContainer(StackPanel container)
        {
            Model.SetContainer(container);
        }

        internal void UpdateNumberOfLamps(int unitLampCount)
        {
            Model.SetNumberOfLamps(unitLampCount);
        }

        internal void AddLamp(int z)
        {
            throw new NotImplementedException();
        }
    }
}
