using Dynadimmer.Views.Information;
using Dynadimmer.Views.NewSchdularSelection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        string filename;
        uint unitid;

        public SaveAction(params UnitProperty[] controls) : base(controls)
        {
            System.Windows.MessageBoxResult result = MessageBox.Show("Do you want to save?","Save to file",
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
                if (File.Exists(saveFileDialog.FileName))
                    File.Delete(saveFileDialog.FileName);
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                filename = saveFileDialog.FileName.Split('.')[0];
                writer = XmlWriter.Create(filename, settings);
                writer.WriteStartDocument();
                writer.WriteStartElement("Dimmer");
            }
            Start();
        }

        public override bool BeforeNext()
        {
            if (Save)
            {
                Upload[Counter].SaveData(writer, Extra[Counter]);
                if (Upload[Counter] is InformationModel)
                    unitid = ((InformationModel)Upload[Counter]).Info.UnitID;
            }
            return true;

        }
        public override string BeforeDone()
        {
            if (Save)
            {
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                writer.Dispose();
                File.Move(filename, Path.ChangeExtension(filename, "."+unitid+".dxml"));
                return "All Data Save to File";
            }
            return "All Data Loaded";
        }
    }
}
