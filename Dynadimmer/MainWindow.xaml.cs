using Dynadimmer.Models;
using Dynadimmer.Models.Actions;
using Dynadimmer.Models.Messages;
using Dynadimmer.Views.Calc;
using Dynadimmer.Views.LampItem;
using Dynadimmer.Views.MonthItem;
using Dynadimmer.Views.Setttings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Dynadimmer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CalcWindow calc;
        IRDAHandler connection;
        LogHandler log;
        WindowHandler viewer;
        AnswerHandler answers;
        Models.Action action;
        AppSettings settings = new AppSettings();
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
            calc = new CalcWindow();
            //calc.Owner = this;
            //calc.WindowStartupLocation = WindowStartupLocation.CenterOwner;
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
            UnitProperty.SetConnection(connection, viewer);
            MonthModel.Perent = newschdularselectionview.Model;
            answers = new AnswerHandler(log, infoview.Model, unitidview.Model, newschdularselectionview.Model, datetimeview.Model, summerwinterview.Model, configview.Model);
            answers.allAnswersProssed += Answers_allAnswersProssed;
            this.DataContext = viewer;
            fileloadview.Model.SetContainer(this.MainContainer);
            newschdularselectionview.Model.SetContainer(this.MainContainer);

            datetimeview.Visibility = viewer.UnitTimeVisibility;
            unitidview.Visibility = viewer.UnitIDVisibility;
            summerwinterview.Visibility = viewer.SummerWinterVisibility;
            configview.Visibility = viewer.ConfigVisibility;
            infoview.Visibility = viewer.UnitInfoVisibility;

            fileloadview.Model.ClickDownload += FileLoad_ClickDownload;
            fileloadview.Model.WinVisibilityChanged += FileLoad_WinVisibilityChanged;

            infoview.Model.GotData += Model_GotData;
            unitidview.Model.GotData += Model_GotData;
            datetimeview.Model.GotData += Model_GotData;
            configview.Model.GotData += Model_GotData;
            summerwinterview.Model.GotData += Model_GotData;
            settings.Model.LoginWin.Model.Loggedin += Model_Loggedin;
            viewer.WindowEnable = true;
            if (connection.IsInit)
                connection.CheckStatus();
        }

        private void Model_Loggedin(object sender, bool e)
        {
            unitidview.Model.DownloadVisibility = e ? Visibility.Visible : Visibility.Hidden;
        }

        private void Model_GotData(object sender, Views.Information.UnitInfo info)
        {
            if (sender.GetType() == infoview.Model.GetType())
            {
                newschdularselectionview.Model.UpdateData(info);
                unitidview.Model.UpdateData(info);
                datetimeview.Model.UpdateData(info);
                configview.Model.UpdateData(info);
                if (viewer.RemoteID != info.UnitID)
                {
                    viewer.RemoteID = info.UnitID;
                    MainContainer.Reset();
                }
            }
            else if (sender.GetType() == unitidview.Model.GetType())
            {
                infoview.Model.UpdateData(info);
                viewer.RemoteID = info.UnitID;
            }
            else if (sender.GetType() == datetimeview.Model.GetType())
            {
                infoview.Model.UpdateData(info);
            }
            else if (sender.GetType() == configview.Model.GetType())
            {
                infoview.Model.UpdateData(info);
                newschdularselectionview.Model.UpdateData(info);
            }

        }

        #region Models Events
        private void FileLoad_ClickDownload(object sender, byte[] e)
        {
            action = new DownloadAllAction(this.MainContainer.GetLampsModels(), e, configview.Model, newschdularselectionview.Model);
        }

        private void FileLoad_WinVisibilityChanged(object sender, Visibility e)
        {
            if (e == Visibility.Collapsed)
            {
                viewer.UnitInfoChecked = true;
                unitidview.Visibility = viewer.UnitIDVisibility;
                datetimeview.Visibility = viewer.UnitTimeVisibility;
                summerwinterview.Visibility = viewer.SummerWinterVisibility;
                configview.Visibility = viewer.ConfigVisibility;
                infoview.Visibility = viewer.UnitInfoVisibility;
                newschdularselectionview.Visibility = Visibility.Collapsed;
                configview.IsEnabled = connection.IsConnected;
                MainContainer.IsEnabled = connection.IsConnected;
                summerwinterview.IsEnabled = connection.IsConnected;
                datetimeview.IsEnabled = connection.IsConnected;
                unitidview.IsEnabled = connection.IsConnected;
                newschdularselectionview.IsEnabled = connection.IsConnected;
                MainContainer.Model.FromFile = false;
                infoview.Model.Info = new Views.Information.UnitInfo();
                infoview.Model.NoDataVisibility = Visibility.Visible;
                if (connection.IsConnected)
                {
                    action = new StartAction(infoview.Model);
                }
            }
            else
            {
                infoview.Visibility = datetimeview.Visibility = newschdularselectionview.Visibility = datetimeview.Visibility = summerwinterview.Visibility = configview.Visibility = Visibility.Collapsed;
                MainContainer.IsEnabled = true;
                MainContainer.Model.FromFile = true;
            }
            viewer.IsConnectedAndNotFromFile = !MainContainer.Model.FromFile && connection.IsConnected;
        }
        #endregion

        #region Connection Events
        private void Connection_Answered(object sender, List<GaneralMessage> e)
        {
            answers.Handle(e);
        }

        private void Connection_Connected(object sender, bool e)
        {
            datetimeview.Model.SendingClock(false);
            if (!MainContainer.Model.FromFile)
            {
                configview.IsEnabled = e;
                MainContainer.IsEnabled = e;
                summerwinterview.IsEnabled = e;
                datetimeview.IsEnabled = e;
                unitidview.IsEnabled = e;
                infoview.IsEnabled = e;
                newschdularselectionview.IsEnabled = e;
                datetimeview.Model.SendingClock(e);
            }
            viewer.IsConnectedAndNotFromFile = !MainContainer.Model.FromFile && connection.IsConnected;
            fileloadview.Model.DownLoadEnable = e;
            if (fileloadview.Model.WinVisibility != Visibility.Visible && e)
                action = new StartAction(infoview.Model);
        }

        #endregion

        #region UI Events
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            datetimeview.Visibility = viewer.UnitTimeVisibility;
            summerwinterview.Visibility = viewer.SummerWinterVisibility;
            configview.Visibility = viewer.ConfigVisibility;
            unitidview.Visibility = viewer.UnitIDVisibility;
            infoview.Visibility = viewer.UnitInfoVisibility;
            if (viewer.SummerWinterChecked)
                summerwinterview.Model.SendUpload(null);
            datetimeview.Model.SendingClock(viewer.UnitClockChecked || viewer.UnitInfoChecked);
        }

        private void MenuItem_Save(object sender, RoutedEventArgs e)
        {
            action = new SaveAction(infoview.Model, newschdularselectionview.Model);
        }

        private void MenuItem_Load(object sender, RoutedEventArgs e)
        {
            fileloadview.Model.ReadFromFile();
        }

        private void MenuItem_Settings(object sender, RoutedEventArgs e)
        {
            settings.Owner = this;
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.ShowDialog();
        }

        private void MenuItem_Calc(object sender, RoutedEventArgs e)
        {
            if (MainContainer.Model.FromFile)
            {
                calc.Model.Read(fileloadview.Model.FilePath);
            }
            else
            {
                List<LampModel> models = this.MainContainer.GetLampsModels().Where(x => x.isConfig && x.AllLoaded).ToList();
                if (models.Count == 0)
                {

                    if (MessageBoxResult.No == MessageBox.Show("Not all data is avilable", "Data error", MessageBoxButton.YesNo))
                        return;
                }
                calc.SetList(models, viewer.RemoteIDString);
            }
            calc.Show();
        }
        #endregion

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
                    action = null;
                }
            }
            else
            {
                viewer.WindowEnable = true;
            }
        }

        private void Log_DoneSaving(object sender, EventArgs e)
        {
            if (close)
                App.Current.Shutdown();
        }
    }

}
