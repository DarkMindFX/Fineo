using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Fineo.AzureEmulatorHelper
{
    public class AzureStorageEmulator
    {
        public bool Start()
        {
            bool result = false;

            ProcessStartInfo psi = CreateCommand("start");

            using (var p = Process.Start(psi))
            {
                p.WaitForExit();
                result = true;
            }


            return result;
        }
        public async Task<bool> StartAsync()
        {
            bool result = false;

            await Task.Run(() =>
                {
                    result = Start();
                }
            );

            return result;
        }

        public bool Stop()
        {
            bool result = false;
            ProcessStartInfo psi = CreateCommand("stop");

            using (var p = System.Diagnostics.Process.Start(psi))
            {
                p.WaitForExit();
                result = true;
            }

            return result;
        }

        public async Task<bool> StopAsync()
        {
            bool result = false;

            await Task.Run(() =>
                {
                    Stop();
                }
            );

            return result;
        }

        public Dictionary<string, string> Status()
        {
            Dictionary<string, string> status = null;
            ProcessStartInfo psi = CreateCommand("status");

            using (Process p = System.Diagnostics.Process.Start(psi))
            {
                var reader = p.StandardOutput;

                status = ReadStatus(reader);
            }

            return status;
        }

        public async Task<Dictionary<string, string>> StatusAsync()
        {
            Dictionary<string, string> status = null;
            await Task.Run(() =>
            {
                status = Status();
            }
            );

            return status;

        }

        #region Support method

        private Dictionary<string, string> ReadStatus(StreamReader reader)
        {
            Dictionary<string, string> status = new Dictionary<string, string>();
            while (!reader.EndOfStream)
            {
                string l = reader.ReadLine();
                string[] parts = l.Split(": ");
                if (parts.Length >= 2)
                {
                    status.Add(parts[0].Trim(), parts[1].Trim());
                }
            }

            return status;
        }

        private ProcessStartInfo CreateCommand(string command)
        {
            string dir = Environment.GetEnvironmentVariable("AZURE_EMULATOR_HOME");
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = $"{dir}\\AzureStorageEmulator.exe";
            psi.ArgumentList.Add(command.ToLower());
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            return psi;
        }
        #endregion
    }
}
