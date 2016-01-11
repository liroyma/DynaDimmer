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

        public MainContainerModel()
        {
            FromFile = false;
        }
    }
}
