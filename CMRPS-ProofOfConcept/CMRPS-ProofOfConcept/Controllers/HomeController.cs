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
using System.Security;
using System.Web;
using System.Web.Mvc;
using CMRPS_ProofOfConcept.Models;
using Microsoft.Owin.Security.Provider;

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
            //List<EventLogItem> events = Events.OrderByDescending(x => x.Time);
            return View(Events.OrderByDescending(x => x.Time));
        }

        // ==================================================================

        private void AddLog(string action, List<string> listOfData, string exception, bool success)
        {
            EventLogItem item = new EventLogItem();
            item.Time = DateTime.Now;
            item.Action = action;
            item.listOfData = listOfData;
            item.Exception = exception;
            item.Success = success;
            Events.Add(item);
        }

        public long Ping(string name)
        {
            List<string> data = new List<string>();
            data.Add("Starting");
            data.Add("Name: " + name);
            if (name.Length > 0)
            {
                bool pingable = false;
                Ping pinger = new Ping();
                try
                {
                    PingReply reply = pinger.Send(name);
                    pingable = reply.Status == IPStatus.Success;
                    data.Add("Pingable: " + pingable);
                    AddLog("Ping", data, "none", true);
                    return reply.RoundtripTime;
                }
                catch (PingException ex)
                {
                    AddLog("Ping", data, ex.ToString(), false);
                }
                return -1;
            }
            else
            {
                AddLog("Ping", data, "none", true);
                return -1;
            }
        }

        public bool ShutdownCMD(string name, bool credentials)
        {
            return Shutdown1(name, credentials);
        }

        public bool ShutdownWMI(string name)
        {
            return Shutdown2(name);
        }

        public bool ShutdownCancel(string name)
        {
            return Shutdown3(name);
        }

        public bool WolCMD(string mac)
        {
            return Wakeup1(mac);
        }

        public bool WolPacket(string mac)
        {
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
        private bool Shutdown1(string name, bool credentials)
        {
            List<string> data = new List<string>();
            data.Add("Starting");
            data.Add("Name: " + name);
            data.Add("Use Credentials: " + credentials);
            if (credentials)
            {
                try
                {
                    // Grab credentials from config file
                    string username = ConfigurationManager.AppSettings.Get("Username");
                    string password = ConfigurationManager.AppSettings.Get("Password");
                    string domain = ConfigurationManager.AppSettings.Get("Domain");
                    data.Add("Username: " + username);
                    data.Add("Password: " + password);
                    data.Add("Domain: " + domain);

                    // Make secure password
                    SecureString securePassword = new SecureString();
                    foreach (char c in password)
                    {
                        securePassword.AppendChar(c);
                    }

                    // Create arguments
                    string args = String.Format("/c shutdown -s -m \\\\{0} -t 30 -f", name);
                    data.Add("Arguments: " + args);

                    // Create the process
                    Process psi = new Process();
                    psi.StartInfo.UseShellExecute = false;
                    psi.StartInfo.RedirectStandardOutput = true;
                    psi.StartInfo.UserName = username;
                    psi.StartInfo.Password = securePassword;
                    psi.StartInfo.Domain = domain;
                    psi.StartInfo.FileName = "cmd.exe";
                    psi.StartInfo.Arguments = args;

                    // Start the process and get output
                    psi.Start();
                    string output = psi.StandardOutput.ReadToEnd();
                    psi.WaitForExit();

                    data.Add("OUTPUT: " + output);
                    AddLog("Shutdown CMD (With credetials)", data, "none", true);
                    return true;
                }
                catch (Exception ex)
                {
                    AddLog("Shutdown CMD (With credetials)", data, ex.ToString(), false);
                    return false;
                }
            }
            else
            {
                try
                {
                    string args = String.Format("-s -m \\\\{0} -t 30 -f", name);
                    data.Add("Arguments: " + args);
                    Process.Start("shutdown", args);
                }
                catch (Exception ex)
                {
                    AddLog("Shutdown CMD", data, ex.ToString(), false);
                    return false;
                }
            }
            AddLog("Shutdown CMD", data, "none", true);
            return true;
        }

        /// <summary>
        /// Using internal methods to shutdown computer.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool Shutdown2(string name)
        {
            List<string> data = new List<string>();
            data.Add("Starting");
            data.Add("Name: " + name);
            try
            {
                ConnectionOptions options = new ConnectionOptions();
                options.EnablePrivileges = true;

                options.Username = ConfigurationManager.AppSettings.Get("Username");
                options.Password = ConfigurationManager.AppSettings.Get("Password");
                options.Authority = "ntlmdomain:" + ConfigurationManager.AppSettings.Get("Domain");
                data.Add("Option Username: " + options.Username);
                data.Add("Option Password: " + ConfigurationManager.AppSettings.Get("Password"));
                data.Add("Option Authority: " + options.Authority);

                ManagementScope scope = new ManagementScope("\\\\" + name + "\\root\\CIMV2", options);
                data.Add("Scope Path: " + scope.Path);
                scope.Connect();
                data.Add("Scope Connected: " + scope.IsConnected);
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
                    inParams["Flags"] = 5;


                    // Execute the method and obtain the return values.
                    //ManagementBaseObject outParams = os.InvokeMethod("Win32Shutdown", inParams, null);
                    ManagementBaseObject outParams = os.InvokeMethod("Win32Shutdown", inParams, null);
                    //data.Add("Out Params: " + outParams);
                }
                AddLog("Shutdown WMI", data, "none", true);
                return true;
            }
            catch (Exception ex)
            {
                AddLog("Shutdown WMI", data, ex.ToString(), false);
                return false;
            }
        }

        private bool Shutdown3(string name)
        {
            List<string> data = new List<string>();
            data.Add("Starting");
            data.Add("Name: " + name);
            try
            {
                string args = String.Format("-a -m \\\\{0}", name);
                data.Add("Arguments: " + args);
                Process.Start("shutdown", args);
            }
            catch (Exception ex)
            {
                AddLog("Shutdown Cancel", data, ex.ToString(), false);
                return false;
            }

            AddLog("Shutdown Cancel", data, "none", true);
            return true;
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
            List<string> data = new List<string>();
            data.Add("Starting");
            data.Add("MAC: " + mac);

            string cleanMac = mac.Replace(":", "");
            data.Add("Clean MAC: " + cleanMac);
            try
            {
                string args = String.Format("{0}", cleanMac);
                data.Add("Arguments: " + args);
                string path = @"c:\WINWAKE.exe";
                data.Add("Path: " + path);
                Process.Start(path, args);
            }
            catch (Exception ex)
            {
                AddLog("Wakeup Winwake", data, ex.ToString(), false);
                return false;
            }
            AddLog("Wakeup Winwake", data, "none", true);
            return true;
        }

        /// <summary>
        /// Using internal methods to start computer.
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        public bool Wakeup2(string mac)
        {
            List<string> data = new List<string>();
            data.Add("Starting");
            data.Add("MAC: " + mac);

            //IPAddress broadcast = new IPAddress(0xffffffff); //255.255.255.255  i.e broadcast
            IPAddress broadcast = IPAddress.Parse("255.255.255.255");
            Int32 port = 0x2fff; // port=12287 let's use this one 
            data.Add("IPAdress: " + broadcast);
            data.Add("Port: " + port);
            //MAC_ADDRESS should  look like '013FA049'
            if (mac.Length > 0)
            {
                // Clean mac of ":".
                string macAdr = mac.Replace(":", "");
                data.Add("Clean mac: " + macAdr);
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
                    data.Add("Return value: " + returnValue);
                    data.Add("bytes: " + bytes);

                    // Check if all bytes were sent OK.
                    bool length = returnValue == bytes.Length;
                    data.Add("Bytes length: " + length);
                    AddLog("Wakeup2", data, "none", false);
                    return true;
                }
                catch (Exception ex)
                {
                    AddLog("Wakeup Packet", data, ex.ToString(), false);
                    return false;
                }
            }
            else
            {
                AddLog("Wakeup Packet", data, "No mac found!", false);
                return false;
            }
        }
    }


}
