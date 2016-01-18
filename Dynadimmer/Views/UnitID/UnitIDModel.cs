using Dynadimmer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynadimmer.Models.Messages;
using System.Xml;

namespace Dynadimmer.Views.UnitID
{
    public class UnitIDModel : UnitProperty
    {
        public const int Header = 250;

        public UnitIDModel()
        {
            Title = "Unit ID";
        }
        public override void DidntGotAnswer()
        {

        }

        public override string GotAnswer(IncomeMessage messase)
        {
            return "";
        }

        public override void SaveData(XmlWriter writer, object extra)
        {

        }

        public override void SendDownLoad(object sender)
        {

        }

        public override void SendUpload(object sender)
        {

        }
    }
}
