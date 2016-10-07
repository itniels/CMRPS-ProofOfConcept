using System;
using System.Collections.Generic;
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
            // Logic for shutdown.

            return false;
        }

        [HttpPost]
        public bool Wol(string mac)
        {
            string cleanMac = mac.Replace(":", "");
            // Logic for wake On Lan.

            return false;
        }
    }
}