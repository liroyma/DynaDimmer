using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Dynadimmer.Models
{
    class Action
    {
        List<UnitProperty> Upload = new List<UnitProperty>();
        int Counter = 0;

        public Action(params UnitProperty[] controls)
        {
            if(controls!=null)
                Upload = controls.ToList();

            foreach (var item in Upload)
            {
                if(item!=null)
                    item.AllDone += Item_AllDone; 
            }
        }

        public void Add(UnitProperty control)
        {
            control.AllDone += Item_AllDone;
            Upload.Add(control);
        }

        private void Item_AllDone(object sender, EventArgs e)
        {
            Counter++;
            if(Counter < Upload.Count)
            {
                Upload[Counter].SendUpload(null);
            }
        }

        public void Start()
        {
            Upload[Counter].SendUpload(null);
        }
    }
}
