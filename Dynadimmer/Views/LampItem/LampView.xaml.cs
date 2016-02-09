using Dynadimmer.Views.MonthItem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Dynadimmer.Views.NewSchdularSelection;

namespace Dynadimmer.Views.LampItem
{
    /// <summary>
    /// Interaction logic for LampView.xaml
    /// </summary>
    public partial class LampView : UserControl
    {
        int visableCounter = 0;

        public int LampIndex
        {
            get { return (int)GetValue(LampNameProperty); }
            set { SetValueDP(LampNameProperty, value); }
        }

        public static readonly DependencyProperty LampNameProperty = DependencyProperty.Register("LampIndex", typeof(int),
            typeof(LampView), null);

        public event PropertyChangedEventHandler PropertyChanged;

        void SetValueDP(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName]string p = null)
        {
            SetValue(property, value);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        public LampModel Model { get; private set; }

        public void SetModel(LampModel model)
        {
            Model = model;
        }

        public LampView()
        {
            Model = new LampModel();
            InitializeComponent();
            this.DataContext = Model;
            Model.SetMonth(monthcontainer.Children);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Model.Init(LampIndex);
            foreach (var item in monthcontainer.Children)
            {
                if (item is MonthView)
                {
                    ((MonthView)item).Model.SetLamp(Model);
                    ((MonthView)item).Model.IsVisibleChanged += LampView_IsVisibleChanged;
                }
            }
        }

        internal void ResetAll()
        {
            foreach (var item in monthcontainer.Children)
            {
                if (item is MonthView)
                {
                    ((MonthView)item).Model.ItemVisablility = Visibility.Collapsed;               }
            }
        }

        private void LampView_IsVisibleChanged(object sender, Visibility e)
        {
            if (e == Visibility.Visible)
                visableCounter++;
            else
                visableCounter--;

            if (visableCounter < 0)
                visableCounter = 0;
            Model.ItemVisibility = visableCounter > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        internal MonthView FindMonth(Month month)
        {
            foreach (var item in monthcontainer.Children)
            {
                if (item is MonthView)
                {
                    if (((MonthView)item).LampMonth == month)
                        return ((MonthView)item);
                }
            }
            return null;
        }

        internal void SetAllPatse(bool value)
        {
            foreach (var item in monthcontainer.Children)
            {
                if (item is MonthView)
                {
                    ((MonthView)item).Model.CanPaste = value;
                }
            }
        }
    }
}
