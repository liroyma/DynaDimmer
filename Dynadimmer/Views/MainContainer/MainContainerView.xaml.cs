using Dynadimmer.Views.LampItem;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Dynadimmer.Views.MainContainer
{
    /// <summary>
    /// Interaction logic for MainContainerView.xaml
    /// </summary>
    public partial class MainContainerView : UserControl
    {
        private List<LampModel> lampsmodels = new List<LampModel>();
        private List<LampView> lampsViews = new List<LampView>();
        
        public MainContainerModel Model { get; private set; }

        public MainContainerView()
        {
            Model = new MainContainerModel();
            InitializeComponent();
            this.DataContext = Model;
            foreach (var item in lampContainer.Children)
            {
                if (item is LampView)
                {
                    lampsmodels.Add(((LampView)item).Model);
                    lampsViews.Add(((LampView)item));
                }
            }
        }

        public LampView FindLamp(int index)
        {
            return lampsViews.Where(x => x.LampIndex == index).FirstOrDefault();
        }

        public LampView FindLamp(LampModel lamp)
        {
            return lampsViews.Where(x => x.Model == lamp).FirstOrDefault();
        }

        public List<LampModel> GetLampsModels()
        {
            return lampsmodels;
        }

        public List<LampView> GetLampsViews()
        {
            return lampsViews;
        }

        internal void Reset()
        {
            foreach (var item in lampsViews)
            {
                item.ResetAll();
            }
        }
    }
}
