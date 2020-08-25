using System;
using System.Collections.Generic;
using LircSharp.DAL;
using LircSharp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Diagnostics;

namespace PiIRRemote.Controllers
{
    [Route("api/[controller]")]
    public class AmbilightController : Controller
    {
        [Route("on")]
        [HttpPost]
        public ActionResult StartAmbilight()
        {
            var enableGrabberCommand = "hyperion-remote -E GRABBER";
            var enableLedCommand = "hyperion-remote -E LEDDEVICE";

            var enableGrabberResult = ExecuteCommand(enableGrabberCommand);
            var enableLedResult = ExecuteCommand(enableLedCommand);

            if(!(enableGrabberResult && enableLedResult))
            {
                return StatusCode(501);
            }
            else
            {
                return StatusCode(200);
            }
        }

        [Route("off")]
        [HttpPost]
        public ActionResult StopAmbilight()
        {
            var enableGrabberCommand = "hyperion-remote -D GRABBER";
            var enableLedCommand = "hyperion-remote -D LEDDEVICE";

            var enableGrabberResult = ExecuteCommand(enableGrabberCommand);
            var enableLedResult = ExecuteCommand(enableLedCommand);

            if (!(enableGrabberResult && enableLedResult))
            {
                return StatusCode(501);
            }
            else
            {
                return StatusCode(200);
            }
        }

        private bool ExecuteCommand(string command)
        {
            var startInfos = new ProcessStartInfo()
            {
                FileName = command.Split(' ').First(),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = command.Substring(command.IndexOf(" "))
            };

            var proc = Process.Start(startInfos);
            var strOut = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            return !strOut.Contains("Error");
        }
    }
}
