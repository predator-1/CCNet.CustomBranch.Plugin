using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core;

namespace ccnet.custombranchcore.plugin
{
    [ReflectorType("getBranches")]
    public class SourceControlExt : ILabeller
    {
        [ReflectorProperty("branches", Required = false)]
        public string[] Branches { get; set; }

        public string Generate(IIntegrationResult integrationResult)
        {
            return "";
        }

        public void Run(IIntegrationResult result)
        {
            
        }
    }
}
