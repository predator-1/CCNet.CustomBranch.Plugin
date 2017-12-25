using Exortech.NetReflector;
using System.Collections;
using System.IO;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace CCNet.CustomBranch.Plugin
{
    public class AllBranchesBuilder
    {
        public HtmlFragmentResponse GenerateAllBranchesView(IProjectSpecifier projectSpecifier, string sessionToken, string currentBranch, string[] branches)
        {
            string html = "<form method=\"POST\">";
            html += string.Format("<h2>{0}</h2>", currentBranch);
            html += GetSelect(branches);
            html += "<button>Change</button>";
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
                    select+= string.Format("<option value=\"{0}\">{0}</option>", branch);
                }
            }
            return select + "</select><p><p><p>";
        }



    }
}
