using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Dynadimmer.Models.Messages;

namespace Dynadimmer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IRDACummunication connection;

        public MainWindow()
        {
            connection = new IRDACummunication();
            connection.Connected += Connection_Connected;
            InitializeComponent();
            UnitProperty.SetConnection(connection);
            this.DataContext = connection;
            schdularselectionview.SetContainer(this.Container);
            datetimeview.Visibility = connection.UnitTimeVisibility;
            summerwinterview.Visibility = connection.SummerWinterVisibility;
            configview.Visibility = connection.ConfigVisibility;
            ((Views.Config.ConfigModel)configview.Model).GotData += ConfigModel_GotData;

        }

        private void ConfigModel_GotData(object sender, EventArgs e)
        {
            datetimeview.UpdateTime(((Views.Config.ConfigModel)configview.Model).UnitTime);
            schdularselectionview.UpdateNumberOfLamps(((Views.Config.ConfigModel)configview.Model).UnitLampCount);
        }

        private void Connection_Connected(object sender, EventArgs e)
        {
            Models.Action startaction = new Models.Action(configview.Model);
            startaction.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connection.Dispose();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            datetimeview.Visibility = connection.UnitTimeVisibility;
            summerwinterview.Visibility = connection.SummerWinterVisibility;
            configview.Visibility = connection.ConfigVisibility;
        }
        
    }

}
