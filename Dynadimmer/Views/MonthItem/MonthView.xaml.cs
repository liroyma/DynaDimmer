using Dynadimmer.Views.NewSchdularSelection;
using Dynadimmer.Views.Schedulers.Inner;
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

namespace Dynadimmer.Views.MonthItem
{
    /// <summary>
    /// Interaction logic for MonthView.xaml
    /// </summary>
    public partial class MonthView : UserControl
    {
        public Month LampMonth
        {
            get { return (Month)GetValue(MonthProperty); }
            set { SetValueDP(MonthProperty, value); }
        }
        public static readonly DependencyProperty MonthProperty = DependencyProperty.Register("LampMonth", typeof(Month),
            typeof(MonthView), null);

        public event PropertyChangedEventHandler PropertyChanged;
        void SetValueDP(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName]string p = null)
        {
            SetValue(property, value);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
        
        public MonthModel Model { get; private set; }

        public MonthView()
        {
            Model = new MonthModel();
            InitializeComponent();
            this.DataContext = Model;
        }

        private void container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Model.UpdateView();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Model.Init(LampMonth, container);
        }

        private void Button_Error(object sender, ValidationErrorEventArgs e)
        {
            Model.IlluminanceError = true;
        }


    }
}
