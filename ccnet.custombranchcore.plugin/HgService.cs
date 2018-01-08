using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ccnet.custombranchcore.plugin
{
    public class HgService
    {
        private string _repo;
        private string _workingPath;

        public HgService(string repo, string workingPath)
        {
            _repo = repo;
            _workingPath = workingPath;
        }

        public List<string> GetAllBranches()
        {
            List<string> branches = new List<string>();

            return branches;
        }
        
        private string RunCommand(string command, string workingPath)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = workingPath,
                    Arguments = command,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,

                }
            };

            proc.Start();
            proc.WaitForExit(5000);
            while (!proc.StandardOutput.EndOfStream)
            {
                return proc.StandardOutput.ReadToEnd();
            }
            return "";
        }

        private void Pull()
        {
            
        }
        
    }
}
