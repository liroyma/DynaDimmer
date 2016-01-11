using Dynadimmer.Models.Messages;
using Dynadimmer.Views.LampItem;
using Dynadimmer.Views.NewSchdularSelection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace Dynadimmer.Models
{
    public abstract class Action
    {
        public static LogHandler Log;

        public List<UnitProperty> Upload { get; private set; }
        public List<object> Extra { get; private set; }
        public int Counter { get; private set; }
        public bool AllDone { get; private set; }

        public Action(params UnitProperty[] controls)
        {
            Counter = 0;
            Upload = new List<UnitProperty>();
            Extra = new List<object>();
            if (controls != null)
            {
                foreach (var control in controls)
                {
                    if (control is NewSchedularSelectionModel)
                    {
                        foreach (LampModel item in ((NewSchedularSelectionModel)control).LampsList)
                        {
                            Upload.Add(control);
                            Extra.Add(item);
                        }
                    }
                    else
                    {
                        Upload.Add(control);
                        Extra.Add(0);
                    }
                }
            }
        }

        public void Start()
        {
            DoAction();
        }

        public void Next()
        {
            BeforeNext();
            Counter++;
            if (Counter < Upload.Count)
            {
                //if (!Upload[Counter].IsLoaded)
                    //DoAction();
                //else
                  //  Next();
            }
            else
            {
                Counter = 0;
                AllDone = true;
                Log.AddMessage(new NotificationMessage(BeforeDone(), Brushes.Green));
            }
        }

        public virtual void DoAction()
        {
            Upload[Counter].SendUpload(Extra[Counter]);
        }

        public abstract void BeforeNext();
        public abstract string BeforeDone();
    }

}
