using Exortech.NetReflector;
using System.Collections;
using System.IO;
using System.Text;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.CCTray;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace CCNet.CustomBranch.Plugin
{
    [ReflectorType("customBranchPlugin")]
    public class CustomBranchPlugin : ICruiseAction, IPlugin
    {
        public static readonly string ACTION_NAME = "CustomBranch";

        private readonly IFarmService farmService;
        private readonly IVelocityViewGenerator viewGenerator;
        private readonly ILinkFactory linkFactory;
        private readonly AllBranchesBuilder viewBuilder;
        private readonly IRemoteServicesConfiguration configuration;
        private ICruiseUrlBuilder urlBuilder;
        private ICruiseManagerWrapper cruiseManagerWrapper;
        private IProjectSpecifier projectSpecifier;
        private string retrieveSessionToken;

        public CustomBranchPlugin(AllBranchesBuilder viewBuilder, IFarmService farmService, IVelocityViewGenerator viewGenerator, ILinkFactory linkFactory,
            IRemoteServicesConfiguration configuration, ICruiseUrlBuilder urlBuilder, ICruiseManagerWrapper cruiseManagerWrapper)
        {
            this.viewBuilder = viewBuilder;
            this.farmService = farmService;
            this.viewGenerator = viewGenerator;
            this.linkFactory = linkFactory;
            this.configuration = configuration;
            this.urlBuilder = urlBuilder;
            this.cruiseManagerWrapper = cruiseManagerWrapper;
        }

        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            var res = cruiseRequest.Request.FindParameterStartingWith("Branch");
            projectSpecifier = cruiseRequest.ProjectSpecifier;
            retrieveSessionToken = cruiseRequest.RetrieveSessionToken();
            if (!string.IsNullOrEmpty(res))
            {
                ChangeBranch(cruiseRequest.Request.GetText(res));
            }

            return viewBuilder.GenerateAllBranchesView(projectSpecifier, retrieveSessionToken, GetCurrentBranch(), GetBranchNames());
        }

        public string LinkDescription
        {
            get { return "Change Branch"; }
        }

        public INamedAction[] NamedActions
        {
            get { return new INamedAction[] { new CustomBranchAction(ACTION_NAME, this) }; }
        }

        public string[] GetBranchNames()
        {
            return new[] { "1", "2", "3" };
        }

        public string GetCurrentBranch()
        {
            return "1";
        }

        public void ChangeBranch(string name)
        {
            string config = cruiseManagerWrapper.GetProject(projectSpecifier, retrieveSessionToken);
            
        }
    }
 
}
