using Dynadimmer.Views.LampItem;
using Dynadimmer.Views.NewSchdularSelection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynadimmer.Models.Actions
{
    class DownloadAllAction : Action
    {
        public DownloadAllAction(List<LampModel> lampmodle, byte[] lampconfig, params UnitProperty[] controls):base()
        {
            foreach (var item in lampmodle)
            {
                Console.WriteLine(item.Name + ": " + item.isConfig);
            }
            if (controls != null)
            {
                foreach (var control in controls)
                {
                    if (control is NewSchedularSelectionModel)
                    {
                        foreach (var lampitem in lampmodle.Where(x=>x.isConfig))
                        {
                            foreach (var monthitem in lampitem.GetMonths())
                            {
                                Upload.Add(control);
                                Extra.Add(monthitem);
                            }
                        }
                    }
                    else
                    {
                        Upload.Add(control);
                        Extra.Add(lampconfig);
                    }
                }
            }
            Start();
        }
        public override string BeforeDone()
        {
            return "Lamp Config and All Programs Downloaded to the Unit";
        }

        public override bool BeforeNext()
        {
            return true;
        }

        public override void DoAction()
        {
            Upload[Counter].SendDownLoad(Extra[Counter]);
        }
    }
}
