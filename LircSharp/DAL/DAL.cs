using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using LircSharp.Models;
using System;

namespace LircSharp.DAL
{
    public class DAL
    {
        private static List<Remote> remoteList;
        
        public DAL()
        {
        }
        
        public Remote GetRemote(string name)
        {
            if (remoteList == null)
            {
                try
                {
                    remoteList = GetRemotes();
                }
                catch(NotSupportedException e)
                {
                    throw e;
                }
            }
            
            /* Loop through remotes until we have a match */
            foreach (Remote remote in remoteList)
            {
                if (remote.Name == name)
                {
                    return remote;
                }
            }
            return null;
        }
        
        public List<Remote> GetRemotes()
        {
            remoteList = new List<Remote>(); 
            
            /* Build process info to call LIRC */
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.FileName = "irsend";
            procInfo.UseShellExecute = false;
            procInfo.RedirectStandardOutput = true;
            
            /* Ask LIRC for a list of remotes */
            procInfo.Arguments = "list \"\" \"\"";
            try
            {
                Process proc = Process.Start(procInfo);
                string strOut = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();

                /* Extract the remote names from the LIRC return*/
                using StringReader reader = new StringReader(strOut);
                string line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        Remote remote = new Remote();
                        remote.Name = line;
                        remote.Codes = GetRemoteCodes(remote);
                        remoteList.Add(remote);
                    }
                } while (line != null);
            }
            catch(Exception)
            {
                throw new NotSupportedException("LIRQ interface cannot be found");
            }
            
            
            return remoteList;            
        }
        
        public List<RemoteCode> GetRemoteCodes(Remote remote)
        {
            List<RemoteCode> remoteCodeList = new List<RemoteCode>(); 
            
            /* Build process info to call LIRC */
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.FileName = "irsend";
            procInfo.UseShellExecute = false;
            procInfo.RedirectStandardOutput = true;
            
            /* Ask LIRC for a list of remotes */
            procInfo.Arguments = "list " + remote.Name + " \"\"";
            try
            {
                Process proc = Process.Start(procInfo);
                string strOut = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
            
                /* Extract the remote names from the LIRC return*/
                using (StringReader reader = new StringReader(strOut))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            RemoteCode code = new RemoteCode();
                            code.Name = line.Split(' ')[1];
                            code.Command = line.Split(' ')[0];
                            code.RemoteName = remote.Name;
                            remoteCodeList.Add(code);
                        }
                    } while (line != null);
                }
            }
            catch(Exception)
            {
                throw new NotSupportedException("LIRQ interface cannot be found");
            }
            
            return remoteCodeList;
        }
        
        public void SendRemoteCode(RemoteCode code)
        {
            /* Build process info to call LIRC */
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.FileName = "irsend";
            procInfo.UseShellExecute = false;
            procInfo.RedirectStandardOutput = true;
            
            /* Ask LIRC for a list of remotes */
            procInfo.Arguments = "SEND_ONCE " + code.RemoteName + " " + code.Name;

            Process proc = Process.Start(procInfo);
            string strOut = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            if(strOut.Contains("hardware does not support"))
            {
                throw new NotSupportedException(strOut);
            }
        }
    }
}
