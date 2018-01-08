using System;
using Exortech.NetReflector;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.CCTray;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using System.Xml.Serialization;
using System.Threading;

namespace CCNet.CustomBranch.Plugin
{
    [ReflectorType("customBranchPlugin")]
    public class CustomBranchPlugin : ICruiseAction, IPlugin
    {
        public static readonly string ACTION_NAME = "CustomBranch";
        private static int sleep = 3000;

        private readonly IFarmService farmService;
        private readonly IVelocityViewGenerator viewGenerator;
        private readonly ILinkFactory linkFactory;
        private readonly AllBranchesBuilder viewBuilder;
        private readonly IRemoteServicesConfiguration configuration;
        private ICruiseUrlBuilder urlBuilder;
        private ICruiseManagerWrapper cruiseManagerWrapper;
        private IProjectSpecifier projectSpecifier;
        private string retrieveSessionToken;
        private IServerSpecifier serverSpecifier;

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
            serverSpecifier = cruiseRequest.ServerSpecifier;
            if (!string.IsNullOrEmpty(res))
            {
                ChangeBranch(cruiseRequest.Request.GetText(res));
                Thread.Sleep(sleep);
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
            var project = cruiseManagerWrapper.GetProject(projectSpecifier, retrieveSessionToken);
            XmlDocument projectXml = new XmlDocument();
            projectXml.LoadXml(project);
            string sourcecontrolType = "";
            var sourcecontrolTypeNodes = projectXml.GetElementsByTagName("sourcecontrol");
            if (sourcecontrolTypeNodes.Count > 0)
            {
                foreach (XmlAttribute attribute in sourcecontrolTypeNodes[0].Attributes)
                {
                    if (attribute.Name == "type")
                        sourcecontrolType = attribute.Value;
                }
                string workingDirectory = "";
                string repo = "";
                foreach (XmlNode childNode in sourcecontrolTypeNodes[0].ChildNodes)
                {
                    if (childNode.Name == "workingDirectory")
                    {
                        workingDirectory = childNode.InnerText;
                    }
                    if (childNode.Name == "repo")
                    {
                        repo = childNode.InnerText;
                    }
                }
                
                SendMassage(MessageBuildGetBranchNames("GetAllBranches", sourcecontrolType, repo, workingDirectory));
                
            }

            return new []{"1", "2", "3"};
        }

        private string GetCurrentBranch()
        {
            var project = cruiseManagerWrapper.GetProject(projectSpecifier, retrieveSessionToken);
            XmlDocument projectXml = new XmlDocument();
            projectXml.LoadXml(project);
            var nodes = projectXml.GetElementsByTagName("branch");
            if (nodes.Count > 0)
            {
                return nodes[0].InnerText;
            }
            return "Error";
        }

        private void ChangeBranch(string name)
        {
            SendMassage(MessageBuildChangeBranch("ChangeBranch", name));
        }

        private void SendMassage(CustomMessage message)
        {
            
            MessageRequest messageRequest = new MessageRequest();
            messageRequest.Kind = Message.MessageKind.NotDefined;
            messageRequest.Message = message.ToString();
            messageRequest.ProjectName = projectSpecifier.ProjectName;
            messageRequest.SessionToken = retrieveSessionToken;
            messageRequest.Identifier = "CustomBranch" + Guid.NewGuid();
            var responce = farmService.ProcessMessage(serverSpecifier, "SendMessage", messageRequest.ToString());
        }

        private CustomMessage MessageBuildChangeBranch(string command, string branch)
        {
            CustomMessage customMessage = new CustomMessage
            {
                Command = command,
                Branch = branch
            };
            return customMessage;
        }

        private CustomMessage MessageBuildGetBranchNames(string command, string sourcecontrolType,
            string repo, string workingDirectory)
        {
            CustomMessage customMessage = new CustomMessage
            {
                Command = command,
                SourcecontrolType = sourcecontrolType,
                Repo = repo,
                WorkingDirectory = workingDirectory
            };
            return customMessage;
        }
    }
 
}
