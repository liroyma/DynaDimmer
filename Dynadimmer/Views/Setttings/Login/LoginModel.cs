using Dynadimmer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Dynadimmer.Views.Setttings.Login
{
    public class LoginModel : MyUIHandler
    {
        #region cont
        private const string USER = "menorah";
        private const string PASS = "liroy";
        #endregion

        public event EventHandler<bool> Loggedin;

        public string Password { get; set; }
        public bool IsLogged { get; set; }

        public MyCommand LogInCommand { get; set; }
        public MyCommand CancelCommand { get; set; }

        public string username;
        public string UserName
        {
            get { return username; }
            set
            {
                username = value;
                NotifyPropertyChanged("UserName");
            }
        }

        public Visibility errorvisibility;
        public Visibility ErrorVisibility
        {
            get { return errorvisibility; }
            set
            {
                errorvisibility = value;
                NotifyPropertyChanged("ErrorVisibility");
            }
        }

        public LoginModel()
        {
            LogInCommand = new MyCommand();
            LogInCommand.CommandSent += LogInCommand_CommandSent;
            CancelCommand = new MyCommand();
            CancelCommand.CommandSent += CancelCommand_CommandSent;
            ErrorVisibility = Visibility.Hidden;
        }

        private void CancelCommand_CommandSent(object sender, EventArgs e)
        {
            ErrorVisibility = Visibility.Hidden;
            if (sender is Window)
            {
                ((Window)sender).Close();
            }
        }

        private void LogInCommand_CommandSent(object sender, EventArgs e)
        {
            IsLogged = UserName == USER && Password == PASS;
            ErrorVisibility = Visibility.Hidden;
            if (IsLogged)
            {
                if (sender is Window)
                {
                    ((Window)sender).Close();
                }
            }
            else
            {
                ErrorVisibility = Visibility.Visible;

            }
            if (Loggedin != null)
                Loggedin(null, IsLogged);

        }

        internal void Logout()
        {
            IsLogged = false;
            UserName = "";
            Password = "";
            if (Loggedin != null)
                Loggedin(null, false);
        }
    }
}
