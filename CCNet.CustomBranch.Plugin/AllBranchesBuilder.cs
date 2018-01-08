using System.Web;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace CCNet.CustomBranch.Plugin
{
    public class AllBranchesBuilder
    {
        public HtmlFragmentResponse GenerateAllBranchesView(IProjectSpecifier projectSpecifier, string sessionToken, string currentBranch, string[] branches)
        {
            string html = "<form method=\"POST\">";
            html += string.Format("<h2>Current branch - {0}</h2>", currentBranch);
            if (branches != null)
            {
                html += GetSelect(branches);
                html += "<button>Change</button>";
            }
            else
            {
                html += "<h2>Sorry, I can't get branches.</h2>";
            }
            html += "</form>";
            return new HtmlFragmentResponse(html);
        }


        private string GetSelect(string[] branches)
        {
            string select = "<p><p><select name=\"Branch\"><option selected value=\"\">Select branch please</option>";
            foreach (var branch in branches)
            {
                if (!string.IsNullOrEmpty(branch))
                {
                    select+= string.Format("<option value=\"{0}\">{1}</option>", HttpUtility.HtmlEncode(branch), branch);
                }
            }
            return select + "</select><p><p><p>";
        }



    }
}
