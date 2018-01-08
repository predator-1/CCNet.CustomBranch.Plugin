using System.Web;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace CCNet.CustomBranch.Plugin
{
    public class AllBranchesBuilder
    {
        public HtmlFragmentResponse GenerateAllBranchesView(IProjectSpecifier projectSpecifier, string sessionToken, string currentBranch, string[] branches)
        {
            string color = currentBranch == "Sourcecontrol not found" ? "#ff0000" : "#2E8A2E";
            string html = "<form method=\"POST\">";
            html += string.Format("<h2>Current branch - <font color=\"{1}\">{0}</font></h2>", currentBranch, color);
            if (branches != null)
            {
                html += GetSelect(branches);
                html += "<button>Change</button>";
            }
            else
            {
                html += "<h2><font color=\"#ff0000\">Sorry, I can't get branches.</font></h2>";
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
