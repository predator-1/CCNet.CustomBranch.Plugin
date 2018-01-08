using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ccnet.custombranchcore.plugin
{
    [Serializable]
    public class CustomMessage
    {
        public string Command { get; set; }

        public string Branch { get; set; }

        public string SourcecontrolType { get; set; }

        public string Repo { get; set; }

        public string WorkingDirectory { get; set; }
    }
}
