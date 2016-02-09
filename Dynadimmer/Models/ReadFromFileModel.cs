using Dynadimmer.Views.LampItem;
using Dynadimmer.Views.MonthItem;
using Dynadimmer.Views.NewSchdularSelection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Dynadimmer.Models
{
    public class ReadFromFileModel
    {
        public static List<LampModel> Read(string filename)
        {
            List<LampModel> lamps = new List<LampModel>();
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            XmlNodeList LampNodes = doc.DocumentElement.SelectNodes("/Dimmer/Lamp");
            foreach (XmlNode lampitem in LampNodes)
            {
                LampView templamp = new LampView();
                templamp.Model.Init(int.Parse(lampitem.Attributes["LampIndex"].Value));
                templamp.Model.LampPower = int.Parse(lampitem.Attributes["LampPower"].Value);

                foreach (XmlNode monthitem in lampitem.ChildNodes)
                {
                    Month month = (Month)Enum.Parse(typeof(Month), monthitem.Attributes["Month"].Value);
                    MonthView tempmonth = templamp.FindMonth(month);
                    tempmonth.Model.SetTimes(monthitem.ChildNodes);
                }
                lamps.Add(templamp.Model);
            }
            return lamps;
        }
    }
}
