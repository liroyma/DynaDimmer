using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;

namespace Dynadimmer.Models
{
    class Action
    {
        List<UnitProperty> Upload = new List<UnitProperty>();
        List<object> Extra = new List<object>();
        int Counter = 0;
        XmlWriter writer;
        bool Save;
        string EndMessage;

        public event EventHandler<string> Done;

        public Action(string endmessage,bool save,params UnitProperty[] controls)
        {
            Counter = 0;
            Save = save;
            EndMessage = endmessage;
            if (controls != null)
            {
                Upload = controls.ToList();
                foreach (var item in controls)
                {
                    Extra.Add(0);
                }
            }
        }

        public void Add(UnitProperty control, Views.NewSchdularSelection.Lamp extra)
        {
            Upload.Add(control);
            Extra.Add(extra);
        }

        public void Add(UnitProperty control)
        {
            Upload.Add(control);
            Extra.Add(0);
        }

        public void Start()
        {
            if (Save)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                writer = XmlWriter.Create(@"C:\Test\employees.xml", settings);
                writer.WriteStartDocument();
                writer.WriteStartElement("Dimmer");
            }

            Upload[Counter].SendUpload(Extra[Counter]);
        }

        public void Next()
        {
            if(Save)
                Upload[Counter].SaveData(writer, Extra[Counter]);
            Counter++;
            if (Counter < Upload.Count)
            {
                if (!Upload[Counter].IsLoaded)
                    Upload[Counter].SendUpload(Extra[Counter]);
                else
                    Next();
            }
            else
            {
                if (Save)
                {
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Close();
                    writer.Dispose();
                }
                Save = false;
                Counter = 0;
                if (Done!=null)
                    Done(null, EndMessage);
            }
        }
    }

}
