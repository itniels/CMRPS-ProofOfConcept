using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;
using CMRPS_ProofOfConcept.Models;

namespace CMRPS_ProofOfConcept.Controllers
{
    public class HomeController : Controller
    {
        public static List<EventLogItem> Events = new List<EventLogItem>();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Proof Of Conecept Demo.";

            return View();
        }

        public ActionResult Logviewer()
        {
            return View(Events);
        }

        // ==================================================================

        private void AddLog(string action, string result, string exception, bool success)
        {
            EventLogItem item = new EventLogItem();
            item.Time = DateTime.Now;
            item.Action = action;
            item.Result = result;
            item.Exception = exception;
            item.Success = success;
            Events.Add(item);
        }

        public long Ping(string name)
        {
            if (name.Length > 0)
            {
                AddLog("Ping", "Starting Ping for: " + name, "none", true);
                bool pingable = false;
                Ping pinger = new Ping();
                try
                {
                    PingReply reply = pinger.Send(name);
                    pingable = reply.Status == IPStatus.Success;
                    if (pingable)
                    {
                        AddLog("Ping", "Recived ping for: " + name, "none", true);
                    }
                    else
                    {
                        AddLog("Ping", "Failed ping for: " + name, "No reply! bad address?", false);
                    }
                    return reply.RoundtripTime;
                }
                catch (PingException ex)
                {
                    AddLog("Ping", "ERROR in Ping for: " + name, ex.ToString(), false);
                }
                return -1;
            }
            else
            {
                AddLog("Ping", "ERROR ping for: " + name, "No address given!", false);
                return -1;
            }
        }

        public bool Shutdown(string name)
        {
            //return CMDTEST(name);
            return Shutdown1(name);
            //return Shutdown2(name);
        }

        public bool Wol(string mac)
        {
            //return CMDTEST(mac);
            //return Wakeup1(mac);
            return Wakeup2(mac);
        }

        // ===============================================================================
        // SHUTDOWN
        // ===============================================================================

        /// <summary>
        /// Using the CMD in windows to shutdown.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool Shutdown1(string name)
        {
            AddLog("Shutdown1", "Starting shutdown1 for: " + name, "none", true);

            try
            {
                Process.Start("shutdown", String.Format("/s /m \\\\{0} /t 30", name));

                //string cn = @"\" + name;
                //System.Diagnostics.Process proc = new System.Diagnostics.Process();
                //System.Security.SecureString ssPwd = new System.Security.SecureString();
                //proc.StartInfo.UseShellExecute = false;
                //proc.StartInfo.FileName = "cmd.exe";
                //proc.StartInfo.Arguments = "/C shutdown -s -m " + cn;
                ////proc.StartInfo.Arguments = "shutdown /s /m " + cn;
                //proc.StartInfo.CreateNoWindow = true;
                //proc.StartInfo.Domain = ConfigurationManager.AppSettings.Get("Domain");
                //proc.StartInfo.UserName = ConfigurationManager.AppSettings.Get("Username");
                //string password = ConfigurationManager.AppSettings.Get("Password");
                //foreach (char t in password)
                //{
                //    ssPwd.AppendChar(t);
                //}
                //proc.StartInfo.Password = ssPwd;
                //proc.Start();
            }
            catch (Exception ex)
            {
                AddLog("Shutdown1", "ERROR in shutdown for: " + name, ex.ToString(), false);
                return false;
            }

            AddLog("Shutdown1", "OK shutdown for: " + name, "none", true);
            return true;
        }

        /// <summary>
        /// Using internal methods to shutdown computer.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool Shutdown2(string name)
        {
            AddLog("Shutdown2", "Started shutdown for: " + name, "none", true);
            try
            {
                ConnectionOptions options = new ConnectionOptions();
                options.EnablePrivileges = true;

                options.Username = ConfigurationManager.AppSettings.Get("Username");
                options.Password = ConfigurationManager.AppSettings.Get("Password");
                options.Authority = "ntlmdomain:" + ConfigurationManager.AppSettings.Get("Domain");

                ManagementScope scope = new ManagementScope("\\\\" + name + "\\root\\CIMV2", options);
                scope.Connect();

                //if (!scope.IsConnected)
                //{
                //    return false;
                //}

                SelectQuery query = new SelectQuery("Win32_OperatingSystem");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                foreach (ManagementObject os in searcher.Get())
                {
                    // Obtain in-parameters for the method
                    ManagementBaseObject inParams = os.GetMethodParameters("Win32Shutdown");

                    // LogOff = 0,
                    // ForcedLogOff = 4,
                    // Shutdown = 1,
                    // ForcedShutdown = 5,
                    // Reboot = 2,
                    // ForcedReboot = 6,
                    // PowerOff = 8,
                    // ForcedPowerOff = 12
                    // Add the input parameters.
                    inParams["Flags"] = 5;


                    // Execute the method and obtain the return values.
                    //ManagementBaseObject outParams = os.InvokeMethod("Win32Shutdown", inParams, null);
                    ManagementBaseObject outParams = os.InvokeMethod("Win32Shutdown", inParams, null);
                }
                AddLog("Shutdown2", "OK shutdown for: " + name, "none", true);
                return true;
            }
            catch (Exception ex)
            {
                AddLog("Shutdown2", "ERROR shutdown for: " + name, ex.ToString(), false);
                return false;
            }
        }


        // ===============================================================================
        // WAKEUP
        // ===============================================================================

        /// <summary>
        /// Helper class for WAKEUP.
        /// </summary>
        public class WOLClass : UdpClient
        {
            public WOLClass() : base() { }

            //this is needed to send broadcast packet
            public void SetClientToBrodcastMode()
            {
                if (this.Active)
                {
                    this.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 0);
                }
            }
        }


        /// <summary>
        /// Using WINWAKE.exe to start a computer.
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        private bool Wakeup1(string mac)
        {
            AddLog("Wakeup1", "Started wakeup for: " + mac, "none", true);

            string cleanMac = mac.Replace(":", "");
            try
            {
                Process.Start(@"WINWAKE.EXE", String.Format("{0}", cleanMac));
            }
            catch (Exception ex)
            {
                AddLog("Wakeup1", "Started wakeup for: " + mac, ex.ToString(), false);
                return false;
            }
            AddLog("Wakeup1", "OK wakeup for: " + mac, "none", true);
            return true;
        }

        /// <summary>
        /// Using internal methods to start computer.
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        public bool Wakeup2(string mac)
        {
            AddLog("Wakeup2", "Started wakeup for: " + mac, "none", true);

            IPAddress broadcast = new IPAddress(0xffffffff); //255.255.255.255  i.e broadcast
            Int32 port = 0x2fff; // port=12287 let's use this one 

            //MAC_ADDRESS should  look like '013FA049'
            if (mac.Length > 0)
            {
                // Clean mac of ":".
                string macAdr = mac.Replace(":", "");

                try
                {
                    WOLClass client = new WOLClass();
                    client.Connect(broadcast, port);
                    client.SetClientToBrodcastMode();
                    //set sending bites
                    int counter = 0;
                    //buffer to be send
                    byte[] bytes = new byte[1024]; // more than enough :-)
                    //first 6 bytes should be 0xFF
                    for (int y = 0; y < 6; y++)
                        bytes[counter++] = 0xFF;
                    //now repeate MAC 16 times
                    for (int y = 0; y < 16; y++)
                    {
                        int i = 0;
                        for (int z = 0; z < 6; z++)
                        {
                            bytes[counter++] =
                                byte.Parse(macAdr.Substring(i, 2),
                                    NumberStyles.HexNumber);
                            i += 2;
                        }
                    }

                    // Now send wake up packet
                    int returnValue = client.Send(bytes, 1024);

                    // Check if all bytes were sent OK.
                    if (returnValue == bytes.Length)
                    {
                        AddLog("Wakeup2", "Started wakeup for: " + mac, "none", true);
                        return true;
                    }

                    AddLog("Wakeup2", "ERROR wakeup for: " + mac, "Not all bytes sent!", false);
                    return false;
                }
                catch (Exception ex)
                {
                    AddLog("Wakeup2", "ERROR wakeup for: " + mac, ex.ToString(), false);
                    return false;
                }
            }
            else
            {
                AddLog("Wakeup2", "ERROR wakeup for: " + mac, "No mac found!", false);
                return false;
            }
        }
    }


}
