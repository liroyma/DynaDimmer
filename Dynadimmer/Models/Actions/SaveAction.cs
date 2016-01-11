using Dynadimmer.Views.NewSchdularSelection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xceed.Wpf.Toolkit;

namespace Dynadimmer.Models
{
    class SaveAction : Action
    {
        XmlWriter writer;
        bool Save;

        public SaveAction(params UnitProperty[] controls) : base(controls)
        {
            System.Windows.MessageBoxResult result = MessageBox.Show("Do you whans to save?","",
                button:System.Windows.MessageBoxButton.YesNoCancel,
                icon:System.Windows.MessageBoxImage.Question,
                defaultResult:System.Windows.MessageBoxResult.Cancel);

            switch(result)
            {
                case System.Windows.MessageBoxResult.Yes:
                    Save = true;
                    break;
                case System.Windows.MessageBoxResult.No:
                    Save = false;
                    break;
                case System.Windows.MessageBoxResult.Cancel:
                    return;
            }
            if (Save)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Dimmer documents (.dxml)|*.dxml";
                if (saveFileDialog.ShowDialog() != true)
                    return;
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                writer = XmlWriter.Create(saveFileDialog.FileName, settings);
                writer.WriteStartDocument();
                writer.WriteStartElement("Dimmer");
            }
            Start();
        }

        public override void BeforeNext()
        {
            if (Save)
                Upload[Counter].SaveData(writer, Extra[Counter]);
        }

        public override string BeforeDone()
        {
            if (Save)
            {
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                writer.Dispose();
                return "All Data Save to File";
            }
            return "All Data Loaded";
        }
    }
}
