using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;

namespace CMRPS_ProofOfConcept.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        // ==================================================================

        [HttpPost]
        public long Ping(string name)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(name);
                pingable = reply.Status == IPStatus.Success;
                return reply.RoundtripTime;
            }
            catch (PingException) { }
            return -1;
        }

        [HttpPost]
        public bool Shutdown(string name)
        {
            string cn = @"\" + name;
            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                System.Security.SecureString ssPwd = new System.Security.SecureString();
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.Arguments = "/C ping 8.8.8.8";
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Domain = "skole.local";
                proc.StartInfo.UserName = "USER";
                string password = "PASS";
                for (int x = 0; x < password.Length; x++)
                {
                    ssPwd.AppendChar(password[x]);
                }
                proc.StartInfo.Password = ssPwd;
                proc.Start();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        [HttpPost]
        public bool Wol(string mac)
        {
            string cleanMac = mac.Replace(":", "");
            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                System.Security.SecureString ssPwd = new System.Security.SecureString();
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.FileName = @"c:\WINWAKE.exe";
                proc.StartInfo.Arguments = "/C" + cleanMac;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Domain = "skole.local";
                proc.StartInfo.UserName = "USER";
                string password = "PASS";
                for (int x = 0; x < password.Length; x++)
                {
                    ssPwd.AppendChar(password[x]);
                }
                proc.StartInfo.Password = ssPwd;
                proc.Start();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}