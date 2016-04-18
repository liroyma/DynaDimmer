using Dynadimmer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dynadimmer.Views.MonthItem;
using System.Xml;
using System.Windows.Controls;
using Dynadimmer.Views.NewSchdularSelection;
using Dynadimmer.Models.Messages;

namespace Dynadimmer.Views.LampItem
{
    public class LampModel : MyUIHandler
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private Visibility _itemvisibility;
        public Visibility ItemVisibility
        {
            get { return _itemvisibility; }
            set
            {
                _itemvisibility = value;
                NotifyPropertyChanged("ItemVisibility");
            }
        }
        
        public int Index { get; private set; }
        public bool isConfig { get; set; }

        private int _lamppower;
        public int LampPower
        {
            get { return _lamppower; }
            set
            {
                _lamppower = value;
                NotifyPropertyChanged("LampPower");
            }
        }

        private bool _allloaded;
        public bool AllLoaded
        {
            get { return _allloaded; }
            set
            {
                _allloaded = value;
                NotifyPropertyChanged("AllLoaded");
            }
        }

        List<MonthModel> months;
        
        public LampModel()
        {
            isConfig = false;
            months = new List<MonthModel>();
            ItemVisibility = Visibility.Collapsed;
        }

        public void Init(int index)
        {
            Name = "Luminaire " + (index + 1);
            Index = index;
        }
        
        public override string ToString()
        {
            return Name;
        }

        internal void SetMonth(UIElementCollection children)
        {
            foreach (var item in children)
            {
                if (item is MonthView)
                {
                    months.Add(((MonthView)item).Model);
                    ((MonthView)item).Model.Loaded += Model_Loaded;
                }
            }
        }

        private void Model_Loaded(object sender, EventArgs e)
        {
            foreach (var item in months)
            {
                if (!item.IsLoaded)
                {
                    AllLoaded = false;
                    return;
                }
            }
            AllLoaded = true;
        }

        internal List<MonthModel> GetMonths()
        {
            return months;
        }


    }
}
