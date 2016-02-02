using Dynadimmer.Models;
using System.Windows;

namespace Dynadimmer.Views.MainContainer
{
    public class MainContainerModel: MyUIHandler
    {
        private Visibility _itemvisable;
        public Visibility ItemVisable
        {
            get { return _itemvisable; }
            set
            {
                _itemvisable = value;
                NotifyPropertyChanged("ItemVisable");
            }
        }

        private bool _fromfile;
        public bool FromFile
        {
            get { return _fromfile; }
            set
            {
                _fromfile = value;
                ItemVisable = value ? Visibility.Visible : Visibility.Collapsed;
                NotifyPropertyChanged("FromFile");
            }
        }

        private int _lampcount;
        public int LampCount
        {
            get { return _lampcount; }
            set
            {
                _lampcount = value;
                NotifyPropertyChanged("LampCount");
            }
        }

        private int _lamp1power;
        public int Lamp1Power
        {
            get { return _lamp1power; }
            set
            {
                _lamp1power = value;
                Lamp1Visibility = value != 0 ? Visibility.Visible : Visibility.Collapsed;
                NotifyPropertyChanged("Lamp1Power");
            }
        }

        private int _lamp2power;
        public int Lamp2Power
        {
            get { return _lamp2power; }
            set
            {
                _lamp2power = value;
                Lamp2Visibility = value != 0 ? Visibility.Visible : Visibility.Collapsed;
                NotifyPropertyChanged("Lamp2Power");
            }
        }

        private Visibility lamp1visibility;
        public Visibility Lamp1Visibility
        {
            get { return lamp1visibility; }
            set
            {
                lamp1visibility = value;
                NotifyPropertyChanged("Lamp1Visibility");
            }
        }

        private Visibility lamp2visibility;
        public Visibility Lamp2Visibility
        {
            get { return lamp2visibility; }
            set
            {
                lamp2visibility = value;
                NotifyPropertyChanged("Lamp2Visibility");
            }
        }

        public MainContainerModel()
        {
            FromFile = false;
        }
    }
}
