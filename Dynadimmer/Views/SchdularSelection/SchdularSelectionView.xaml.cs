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

namespace Dynadimmer.Views.SchdularSelection
{
    /// <summary>
    /// Interaction logic for SchdularSelectionView.xaml
    /// </summary>
    public partial class SchdularSelectionView : UserControl
    {
        SchdulerSelectionModel modol;
        public SchdularSelectionView()
        {
            modol = new SchdulerSelectionModel();
            InitializeComponent();
            this.DataContext = modol;
        }

        public void SetContainer(StackPanel containr)
        {
            modol.SetContainer(containr);
        }

        internal void UpdateNumberOfLamps(int unitLampCount)
        {
            modol.SetNumberOfLamps(unitLampCount);
        }

        internal UnitProperty UploadScadular(Lamp lamp, Month month)
        {
            return modol.Sent(lamp, month);
        }

        internal List<UnitProperty> LoadAll()
        {
            return modol.SentAll();
        }
    }
}
