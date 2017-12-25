using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace CCNet.CustomBranch.Plugin
{
    public class CustomBranchAction : INamedAction
    {
        private readonly string actionName;
        private readonly ICruiseAction action;

        public CustomBranchAction(string actionName, ICruiseAction action)
        {
            this.actionName = actionName;
            this.action = action;
        }

        public string ActionName
        {
            get { return actionName; }
        }

        public ICruiseAction Action
        {
            get { return action; }
        }
    }
}
