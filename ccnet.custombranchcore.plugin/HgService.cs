using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ccnet.custombranchcore.plugin
{
    public class HgService
    {
        private string _repo;
        private string _workingPath;
        private string sourceControl = "hg";

        public HgService(string repo, string workingPath)
        {
            _repo = repo;
            _workingPath = workingPath;
        }

        public List<string> GetAllBranches()
        {
            Pull();
            string branchesReq = Branches();
            return BranchesPrs(branchesReq);
        }
        
        private string RunCommand(string command)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = _workingPath,
                    FileName = sourceControl,
                    Arguments = command,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,

                }
            };

            proc.Start();
            proc.WaitForExit(30000);
            while (!proc.StandardOutput.EndOfStream)
            {
                return proc.StandardOutput.ReadToEnd();
            }
            return "";
        }

        private void Pull()
        {
            string command = "pull " + _repo;
            RunCommand(command);
        }


        private string Branches()
        {
            string command = "branches";
            return RunCommand(command);
        }

        private List<string> BranchesPrs(string lines)
        {
            List<string> branches = new List<string>();
            string[] linesSplited = lines.Split('\n');
            foreach (var lineSplited in linesSplited)
            {
                string techInfo = new Regex(" [0-9]+:[0-9a-z() ]+$").Match(lineSplited).Value;
                if (!string.IsNullOrEmpty(techInfo))
                {
                    branches.Add(lineSplited.Replace(techInfo, "").Trim());
                }
            }
            return branches;
        }

    }
}
