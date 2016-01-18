using Dynadimmer.Models;
using Dynadimmer.Models.Actions;
using Dynadimmer.Models.Messages;
using Dynadimmer.Views.MonthItem;
using System;
using System.Collections.Generic;
using System.Windows;

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
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.InnerException.Message);
            }
            log = (LogHandler)this.FindResource("Logger");
            log.DoneSaving += Log_DoneSaving;
            Models.Action.Log = log;
            viewer = (WindowHandler)this.FindResource("Viewer");
            connection = (IRDAHandler)this.FindResource("Connection");
            try
            {
                connection.InitWCL();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.SetHandlers(log, viewer);
            connection.Connected += Connection_Connected;
            connection.Answered += Connection_Answered;
            UnitProperty.SetConnection(connection);
            MonthModel.Perent = newschdularselectionview.Model;
            answers = new AnswerHandler(log, unitidview.Model, newschdularselectionview.Model, datetimeview.Model, summerwinterview.Model, configview.Model);
            answers.allAnswersProssed += Answers_allAnswersProssed;
            this.DataContext = viewer;
            fileloadview.Model.SetContainer(this.MainContainer);
            fileloadview.Model.ClickDownload += Model_ClickDownload;
            newschdularselectionview.Model.SetContainer(this.MainContainer);
            fileloadview.Model.WinVisibilityChanged += Model_WinVisibilityChanged;
            datetimeview.Visibility = viewer.UnitTimeVisibility;
            unitidview.Visibility = viewer.UnitIDVisibility;
            summerwinterview.Visibility = viewer.SummerWinterVisibility;
            configview.Visibility = viewer.ConfigVisibility;
            configview.Model.GotData += ConfigModel_GotData;
            viewer.WindowEnable = true;
            if (connection.IsInit)
                connection.CheckStatus();
        }

        private void Model_ClickDownload(object sender, byte e)
        {
            action = new DownloadAllAction(this.MainContainer.GetLampsModels(), e, configview.Model, newschdularselectionview.Model);
        }

        private void Model_WinVisibilityChanged(object sender, Visibility e)
        {
            if (e == Visibility.Collapsed)
            {
                viewer.ConfigChecked = true;
                unitidview.Visibility = viewer.UnitIDVisibility;
                datetimeview.Visibility = viewer.UnitTimeVisibility;
                summerwinterview.Visibility = viewer.SummerWinterVisibility;
                configview.Visibility = viewer.ConfigVisibility;
                newschdularselectionview.Visibility = Visibility.Collapsed;
                configview.IsEnabled = connection.IsConnected;
                MainContainer.IsEnabled = connection.IsConnected;
                summerwinterview.IsEnabled = connection.IsConnected;
                datetimeview.IsEnabled = connection.IsConnected;
                unitidview.IsEnabled = connection.IsConnected;
                newschdularselectionview.IsEnabled = connection.IsConnected;
                MainContainer.Model.FromFile = false;
                if (connection.IsConnected)
                {
                    action = new StartAction(configview.Model);
                }
            }
            else
            {
                datetimeview.Visibility = newschdularselectionview.Visibility = datetimeview.Visibility = summerwinterview.Visibility = configview.Visibility = Visibility.Collapsed;
                MainContainer.IsEnabled = true;
                MainContainer.Model.FromFile = true;
            }
            viewer.IsConnectedAndNotFromFile = !MainContainer.Model.FromFile && connection.IsConnected;
        }

        private void Answers_allAnswersProssed(object sender, EventArgs e)
        {
            if (action != null)
            {
                action.Next();
                if (!connection.IsConnected)
                {
                    action.Stop();
                }
                if (!action.AllDone)
                {
                    action.DoAction();
                }
                else
                {
                    viewer.WindowEnable = true;
                }
            }
            else
            {
                viewer.WindowEnable = true;
            }
        }

        private void Connection_Answered(object sender, List<GaneralMessage> e)
        {
            answers.Handle(e);
        }

        private void ConfigModel_GotData(object sender, EventArgs e)
        {
            datetimeview.UpdateTime(configview.Model.UnitTime);
            newschdularselectionview.Model.SetNumberOfLamps(configview.Model.UnitLampCount);
        }

        private void Connection_Connected(object sender, bool e)
        {
            if (!MainContainer.Model.FromFile)
            {
                configview.IsEnabled = e;
                MainContainer.IsEnabled = e;
                summerwinterview.IsEnabled = e;
                datetimeview.IsEnabled = e;
                unitidview.IsEnabled = e;
                newschdularselectionview.IsEnabled = e;
            }
            viewer.IsConnectedAndNotFromFile = !MainContainer.Model.FromFile && connection.IsConnected;
            if (fileloadview.Model.WinVisibility != Visibility.Visible && e)
                action = new StartAction(configview.Model);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            close = true;
            e.Cancel = true;
            if (connection != null)
                connection.Dispose();
            if (log != null)
                log.Save();
            else
                App.Current.Shutdown();

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
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
            unitidview.Visibility = viewer.UnitIDVisibility;
        }

        private void MenuItem_Save(object sender, RoutedEventArgs e)
        {
            action = new SaveAction(configview.Model, newschdularselectionview.Model);
        }

        private void MenuItem_Load(object sender, RoutedEventArgs e)
        {
            fileloadview.Model.ReadFromFile();
        }
    }

}
