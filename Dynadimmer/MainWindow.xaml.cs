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
        IRDAHandler connection;
        LogHandler log;
        WindowHandler viewer;

        public MainWindow()
        {
            InitializeComponent();
            log = (LogHandler)this.FindResource("Logger");
            log.DoneSaving += Log_DoneSaving;
            viewer = (WindowHandler)this.FindResource("Viewer");
            connection = (IRDAHandler)this.FindResource("Connection");
            connection.SetHandlers(log, viewer);
            connection.Connected += Connection_Connected;
            UnitProperty.SetConnection(connection);
            this.DataContext = viewer;
            schdularselectionview.SetContainer(this.Container);
            datetimeview.Visibility = viewer.UnitTimeVisibility;
            summerwinterview.Visibility = viewer.SummerWinterVisibility;
            configview.Visibility = viewer.ConfigVisibility;
            ((Views.Config.ConfigModel)configview.Model).GotData += ConfigModel_GotData;
            connection.CheckStatus();
        }

        private void ConfigModel_GotData(object sender, EventArgs e)
        {
            datetimeview.UpdateTime(((Views.Config.ConfigModel)configview.Model).UnitTime);
            schdularselectionview.UpdateNumberOfLamps(((Views.Config.ConfigModel)configview.Model).UnitLampCount);
        }

        private void Connection_Connected(object sender, EventArgs e)
        {
            Models.Action startaction = new Models.Action(configview.Model);
            //startaction.Add(schdularselectionview.UploadScadular(Views.SchdularSelection.Lamp.Lamp_1, Views.SchdularSelection.Month.January));
            startaction.Add(schdularselectionview.LoadAll());
            startaction.Start();
        }

        bool close = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            close = true;
            e.Cancel = true;
            connection.Dispose();
            log.Save();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Log_DoneSaving(object sender, EventArgs e)
        {
            if (close)
                App.Current.Shutdown();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            datetimeview.Visibility = viewer.UnitTimeVisibility;
            summerwinterview.Visibility = viewer.SummerWinterVisibility;
            configview.Visibility = viewer.ConfigVisibility;
        }


    }

}
