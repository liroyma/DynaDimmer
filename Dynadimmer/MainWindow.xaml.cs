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
using System.ComponentModel;
using Dynadimmer.Views.NewSchdularSelection;
using System.Xml;

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
        AnswerHandler answers;

        Models.Action action;
        bool close = false;

        public MainWindow()
        {
            InitializeComponent();
            log = (LogHandler)this.FindResource("Logger");
            log.DoneSaving += Log_DoneSaving;
            viewer = (WindowHandler)this.FindResource("Viewer");
            connection = (IRDAHandler)this.FindResource("Connection");
            connection.SetHandlers(log, viewer);
            connection.Connected += Connection_Connected;
            connection.Answered += Connection_Answered;
            UnitProperty.SetConnection(connection);
            answers = new AnswerHandler(log, newschdularselectionview.Model, datetimeview.Model, summerwinterview.Model, configview.Model);
            answers.allAnswersProssed += Answers_allAnswersProssed;
            this.DataContext = viewer;
            newschdularselectionview.SetContainer(this.Container);
            datetimeview.Visibility = viewer.UnitTimeVisibility;
            summerwinterview.Visibility = viewer.SummerWinterVisibility;
            configview.Visibility = viewer.ConfigVisibility;
            ((Views.Config.ConfigModel)configview.Model).GotData += ConfigModel_GotData;
            connection.CheckStatus();
        }

        private void Answers_allAnswersProssed(object sender, EventArgs e)
        {
            if (action != null)
            {
                action.Next();
            }
        }

        private void Connection_Answered(object sender, ListEventArgs e)
        {
            answers.Handle(e.Data);
        }

        private void ConfigModel_GotData(object sender, EventArgs e)
        {
            datetimeview.UpdateTime(((Views.Config.ConfigModel)configview.Model).UnitTime);
            newschdularselectionview.UpdateNumberOfLamps(((Views.Config.ConfigModel)configview.Model).UnitLampCount);
        }

        private void Connection_Connected(object sender, EventArgs e)
        {
            action = new Models.Action("", false, configview.Model);
            action.Done += Action_Done;
            action.Start();
        }

        private void Action_Done(object sender, string e)
        {
            if (e != "")
                log.AddMessage(new NotificationMessage(e, Brushes.Green));
            action = null;
        }

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

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            action = new Models.Action("Done loading", true, configview.Model);
            action.Done += Action_Done;
            foreach (var item in newschdularselectionview.Model.LampsList)
            {
                action.Add(newschdularselectionview.Model, item);
            }
            action.Start();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Filter = "Dimmer documents (.dxml)|*.dxml";

            if (dlg.ShowDialog() != true)
                return;

            XmlDocument doc = new XmlDocument();
            doc.Load(dlg.FileName);

            XmlNodeList ConfigutarionNodes = doc.DocumentElement.SelectNodes("/Dimmer/Configutarion");
            XmlNodeList LampNodes = doc.DocumentElement.SelectNodes("/Dimmer/Lamp");

            var x = ConfigutarionNodes.Item(0).Attributes["LampCount"].Value;
            foreach (XmlNode node in LampNodes)
            {
                newschdularselectionview.Model.AddLamp(node);
            }
        }
    }

}
