using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Events;
using ThoughtWorks.CruiseControl.Remote.Messages;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace ccnet.custombranchcore.plugin
{
    public class CustomBranchPlugin : ICruiseServerExtension
    {
        private ICruiseServer server;
        private static string ccNetConfigFileName = "ccnet.config";
        private static string branchSectionName = "BranchName";
        private static string projectNameSectionName = "TargetEnvironment";

        public void Initialise(ICruiseServer server,
            ExtensionConfiguration extensionConfig)
        {
            this.server = server;
            this.server.SendMessageReceived += SendMessageReceived;
        }
        public void Start()
        {
           
        }
        public void Stop()
        {
            
        }
        public void Abort()
        {
            
        }

        public void SendMessageReceived(object obj, CancelProjectEventArgs<Message> message)
        {
            string messageTxt = message.Data.Text;
            CustomMessage customMessage = GetCustomMessage(messageTxt);
            if (customMessage != null)
            {
                var fileSystem = server.RetrieveService(typeof(IFileSystem)) as IFileSystem ??
                                 new SystemIoFileSystem();
                if (customMessage.Command == "ChangeBranch")
                {
                    ChangeBranch(fileSystem, customMessage.Branch, message.ProjectName);
                }
                if (customMessage.Command == "GetAllBranches")
                {
                    if (!string.IsNullOrEmpty(customMessage.Repo) &&
                        !string.IsNullOrEmpty(customMessage.WorkingDirectory)
                        && !string.IsNullOrEmpty(customMessage.SourcecontrolType))
                    {
                        GetAllBranches(fileSystem, message.ProjectName, customMessage);
                    }
                }
            }

        }

        private CustomMessage GetCustomMessage(string message)
        {
            if (!string.IsNullOrEmpty(message) && message.StartsWith("<?xml"))
            {
                var serializer = new XmlSerializer(typeof(CustomMessage));
                CustomMessage result;
                using (TextReader reader = new StringReader(message))
                {
                    result = (CustomMessage)serializer.Deserialize(reader);
                }
                return result;
            }
            return null;
        }

        private void ChangeBranch(IFileSystem fileSystem, string branchName, string projectName)
        {
            CustomConfig cfg = GetProjectCfg(fileSystem, projectName);
            if (cfg != null)
            {
                fileSystem.Save(cfg.Href, ChangeBranch(cfg.CurrentConfigTxt, branchName));
            }
        }

        private void GetAllBranches(IFileSystem fileSystem, string projectName, CustomMessage customMessage)
        {
            CustomConfig cfg = GetProjectCfg(fileSystem, projectName);
            if (cfg != null)
            {
                string changedCfg = SetAllBranches(cfg.CurrentConfigTxt, customMessage);
                if (!string.IsNullOrEmpty(changedCfg))
                {
                    fileSystem.Save(cfg.Href, changedCfg);
                }
            }
        }


        private bool IsCurrentProjectCfg(string xml, string projectName)
        {
            return xml.Contains($"{projectNameSectionName}=\"{projectName}\"");
        }

        private string ChangeBranch(string xml, string branchName)
        {
            string currentBranch = new Regex(branchSectionName + "=\"(?<name>.*?)\"").Match(xml).Groups["name"].Value;
            return xml.Replace($"{branchSectionName}=\"{currentBranch}\"", $"{branchSectionName}=\"{branchName}\"");
        }

        private string SetAllBranches(string xml, CustomMessage customMessage)
        {
            if (customMessage.SourcecontrolType == "hg")
            {
                HgService hgService = new HgService(customMessage.Repo, customMessage.WorkingDirectory);
                var branches = hgService.GetAllBranches();
                if(branches.Any())
                    return SetBranchesToLabler(branches, xml);
            }
            return "";
        }

        private CustomConfig GetProjectCfg(IFileSystem fileSystem, string projectName)
        {
            var ccNetCnfig = fileSystem.Load(ccNetConfigFileName);
            string ccNetCnfigTxt;
            using (ccNetCnfig)
            {
                ccNetCnfigTxt = ccNetCnfig.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(ccNetCnfigTxt))
            {
                XDocument doc = XDocument.Parse(ccNetCnfigTxt);
                var nodes = doc.Root?.Nodes().Where(x => x.NodeType == XmlNodeType.Element).Select(x => x as XElement)
                    .Where(v => v.Name.LocalName == "include");
                if (nodes != null)
                {
                    foreach (XElement node in nodes)
                    {
                        var href = node.Attributes().FirstOrDefault(b => b.Name.LocalName == "href");
                        if (href != null)
                        {
                            var currentConfig = fileSystem.Load(href.Value);
                            string currentConfigTxt;
                            using (currentConfig)
                            {
                                currentConfigTxt = currentConfig.ReadToEnd();
                            }
                            if (!string.IsNullOrEmpty(currentConfigTxt))
                            {
                                if (IsCurrentProjectCfg(currentConfigTxt, projectName))
                                {
                                    CustomConfig cfg = new CustomConfig();
                                    cfg.CurrentConfigTxt = currentConfigTxt;
                                    cfg.Href = href.Value;
                                    return cfg;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private string SetBranchesToLabler(List<string> branches, string xml)
        {
            string oldBranches = xml;
            int startLabel = oldBranches.IndexOf("<labeller type=\"getBranches\">", StringComparison.Ordinal) + "<labeller type=\"getBranches\">".Length;
            oldBranches = oldBranches.Remove(0, startLabel);
            int endLabel = oldBranches.IndexOf("</labeller>", StringComparison.Ordinal);
            oldBranches = oldBranches.Remove(endLabel, oldBranches.Length - endLabel);
            string newBranches = "\r\n<branches>\r\n";
            foreach (var branch in branches)
            {
                newBranches += $"<value>{branch}</value>\r\n";
            }
            newBranches += "\r\n</branches>\r\n";
            return xml.Replace(oldBranches, newBranches);
        }

    }
}
