using Dynadimmer.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dynadimmer.Models.Actions
{
    class StartAction : Action
    {
        public StartAction(params UnitProperty[] controls):base(controls)
        {
            Start();
        }

        public override string BeforeDone()
        {
            return "";
        }

        public override void BeforeNext()
        {
        }
    }
}
