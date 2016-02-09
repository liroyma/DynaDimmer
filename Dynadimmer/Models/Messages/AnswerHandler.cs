using Dynadimmer.Views.Config;
using Dynadimmer.Views.DateTime;
using Dynadimmer.Views.FileLoad;
using Dynadimmer.Views.Information;
using Dynadimmer.Views.NewSchdularSelection;
using Dynadimmer.Views.SummerWinnter;
using Dynadimmer.Views.UnitID;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dynadimmer.Models.Messages
{
    class AnswerHandler
    {
        private const byte GaneralHeader = 255;
        private const byte InnerHeader = 254;

        private ConfigModel Config;
        private UnitDateTimeModel UnitDateTime;
        private NewSchedularSelectionModel NewSchedularSelection;
        private UnitSummerWinnterClockModel UnitSummerWinnterClock;
        private UnitIDModel UnitID;
        private InformationModel UnitInfo;
        private LogHandler Log;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public event EventHandler allAnswersProssed;

        public AnswerHandler(LogHandler log, params UnitProperty[] properties)
        {
            Log = log;
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            foreach (var prop in properties)
            {
                if (prop is ConfigModel)
                    Config = (ConfigModel)prop;
                else if (prop is InformationModel)
                    UnitInfo = (InformationModel)prop;
                else if (prop is UnitDateTimeModel)
                    UnitDateTime = (UnitDateTimeModel)prop;
                else if (prop is NewSchedularSelectionModel)
                    NewSchedularSelection = (NewSchedularSelectionModel)prop;
                else if (prop is UnitSummerWinnterClockModel)
                    UnitSummerWinnterClock = (UnitSummerWinnterClockModel)prop;
                else if (prop is UnitIDModel)
                    UnitID = (UnitIDModel)prop;
            }
        }

        #region BackgroundWorker
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            allAnswersProssed(null, null);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            GaneralMessage item = e.UserState as GaneralMessage;
            if (item is IncomeMessage)
            {
                SetPropety(((IncomeMessage)item));
                Log.AddMessage(item);
            }
            else if (item is JunkMessage)
            {
                Log.AddMessage(item);
            }
            else if (item is NotificationMessage)
            {
                Log.AddMessage(item);
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var item in (e.Argument as List<GaneralMessage>))
            {
                worker.ReportProgress(0, item);
            }

        }
        #endregion

        internal void Handle(List<GaneralMessage> items)
        {
            worker.RunWorkerAsync(items);

        }

        private void SetPropety(IncomeMessage item)
        {
            try
            {
                item.Info = "Recived ";
                switch (item.Header)
                {
                    case InformationModel.Header:
                        item.Info += UnitInfo.GotAnswer(item);
                        break;
                    case UnitIDModel.Header:
                        item.Info += UnitID.GotAnswer(item);
                        break;
                    case ConfigModel.Header:
                        item.Info += Config.GotAnswer(item);
                        break;
                    case UnitDateTimeModel.Header:
                        item.Info += UnitDateTime.GotAnswer(item);
                        break;
                    case NewSchedularSelectionModel.UploadHeader:
                    case NewSchedularSelectionModel.DownloadHeader:
                        item.Info += NewSchedularSelection.GotAnswer(item);
                        break;
                    case UnitSummerWinnterClockModel.Header:
                        item.Info += UnitSummerWinnterClock.GotAnswer(item);
                        break;
                    case GaneralHeader:
                        string title = GetTitle(item.DecimalData);
                        HandleGaneralMassege(title, item);
                        break;
                    case InnerHeader:
                        HandleInnerMassege(item);
                        break;
                }
            }
            catch
            {
                item.MessageColor = Brushes.Red;
                item.Info = "Error analysing the data";
            }
            finally
            {
                Log.AddMessage(new NotificationMessage(item.Info, item.MessageColor));
            }

           
        }
        
        private string GetTitle(byte[] data)
        {
            switch (data[2])
            {
                case InformationModel.Header:
                    return UnitInfo.Title;
                case ConfigModel.Header:
                    return Config.Title;
                case UnitDateTimeModel.Header:
                    return UnitDateTime.Title;
                case NewSchedularSelectionModel.UploadHeader:
                case NewSchedularSelectionModel.DownloadHeader:
                    return NewSchedularSelection.Title;
                case UnitSummerWinnterClockModel.Header:
                    return UnitSummerWinnterClock.Title;
                case UnitIDModel.Header:
                    return UnitID.Title;
                case NewSchedularSelectionModel.ResetHeader:
                    return NewSchedularSelection.ResetTitle;
                default:
                    return "Unknown header";
            }
        }

        private void HandleGaneralMassege(string title, IncomeMessage mess)
        {
            string str = "Fail to get " + title + ", ";
            var color = Brushes.Red;
            switch (mess.DecimalData[5])
            {
                case 0:
                    str = NewSchedularSelection.GotGaneralAnswer(mess);
                    switch (mess.DecimalData[2])
                    {
                        case InformationModel.Header:
                            break;
                        case UnitIDModel.Header:
                            break;
                        case ConfigModel.Header:
                            break;
                        case UnitDateTimeModel.Header:
                            break;
                        case NewSchedularSelectionModel.UploadHeader:
                        case NewSchedularSelectionModel.DownloadHeader:
                        case NewSchedularSelectionModel.ResetHeader:
                            str = NewSchedularSelection.GotGaneralAnswer(mess);
                            color = Brushes.Green;
                            break;
                        case UnitSummerWinnterClockModel.Header:
                            break;
                    }
                    break;
                case 1:
                    str += ", Number of ascii byte is odd.";
                    break;
                case 2:
                    str += ", CRC problem.";
                    break;
                case 3:
                    str += ", Luminaire don't exist.";
                    break;
                case 4:
                    str += ", Program message to short (less then 5 bytes).";
                    break;
                case 5:
                    str += ", Message length is not valid.";
                    break;
                case 6:
                    str += ", Message with wrong program number.";
                    break;
                case 7:
                    str += ", Message data is not valid.";
                    break;
                case 8:
                    str += ", Fail to save the changes.";
                    break;
                case 9:
                    str += ", Year can't be 0 in the unit clock .";
                    break;
                case 10:
                    str += ", Unit ID is wrong.";
                    break;
                case 11:
                    str += ", Unable to update UnitID to Broadcast value.";
                    break;
                case 12:
                    str += ", Unable to update UnitID withput pushing the button.";
                    break;
                case 14:
                    str += ", Broadcast is not active.";
                    break;
                case 15:
                    str += ", Dim times are not in the right order.";
                    break;
            }
            mess.Info = str;
            mess.MessageColor = color;
        }

        private void HandleInnerMassege(IncomeMessage item)
        {
            item.Info += "inner error.";
            item.MessageColor = Brushes.White;
            item.MessageBackground = Brushes.Red;
            switch(item.OnlyData[0])
            {
                case 1:
                    item.Info += "\nUnable to read unit id from the memory.";
                    break;
                case 2:
                    item.Info += "\nUnable to read unit configuration from the memory.";
                    break;
                case 3:
                    item.Info += "\nUnable to read programs from the memory.";
                    break;
                case 4:
                    item.Info += "\nUnable to read summer winter dates from the memory.";
                    break;
                case 5:
                    item.Info += "\nUnable to read summer winter update from the memory.";
                    return;
                case 6:
                    item.Info += "\nUnable to save summer winter dates to the memory.";
                    break;
            }
            switch (item.OnlyData[1])
            {
                case 1:
                    item.Info += "\nClock isn't initilazed.";
                    break;
                case 17:
                    item.Info += "\nSummer winter dates are not update.";
                    break;
                case 33:
                case 49:
                case 65:
                case 81:
                    item.Info += "\nSummer winter clock updated not in time.";
                    break;
                case 97:
                    item.Info += "\nCurrnt year is 2000.";
                    break;
            }

        }

    }
}
