﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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
            //return Shutdown1(name);
            return Shutdown2(name);
        }

        [HttpPost]
        public bool Wol(string mac)
        {
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

            string cn = @"\" + name;
            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                System.Security.SecureString ssPwd = new System.Security.SecureString();
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.Arguments = "/C shutdown /s /f /m " + cn + " /c CMRPS is shutting this device down.";
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Domain = "skole.local";
                proc.StartInfo.UserName = "USER";
                string password = "PASS";
                foreach (char t in password)
                {
                    ssPwd.AppendChar(t);
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

        /// <summary>
        /// Using internal methods to shutdown computer.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool Shutdown2(string name)
        {
            try
            {
                //string computerName = name; // computer name or IP address

                ConnectionOptions options = new ConnectionOptions();
                options.EnablePrivileges = true;

                options.Username = "USERNAME";
                options.Password = "PASSWORD";
                options.Authority = "skole.local";

                ManagementScope scope = new ManagementScope(
                    "\\\\" + name + "\\root\\CIMV2", options);
                scope.Connect();

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
                return true;
            }
            catch (Exception)
            {
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
            public WOLClass()
                : base()
            { }
            //this is needed to send broadcast packet
            public void SetClientToBrodcastMode()
            {
                if (this.Active)
                    this.Client.SetSocketOption(SocketOptionLevel.Socket,
                                              SocketOptionName.Broadcast, 0);
            }
        }


        /// <summary>
        /// Using WINWAKE.exe to start a computer.
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        private bool Wakeup1(string mac)
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
                foreach (char t in password)
                {
                    ssPwd.AppendChar(t);
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

        /// <summary>
        /// Using internal methods to start computer.
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        public bool Wakeup2(string mac)
        {
            //MAC_ADDRESS should  look like '013FA049'
            if (mac.Length > 0)
            {
                string macAdr = mac.Replace(":", "");
                try
                {
                    WOLClass client = new WOLClass();
                    client.Connect(new
                        IPAddress(0xffffffff), //255.255.255.255  i.e broadcast
                        0x2fff); // port=12287 let's use this one 
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

                    //now send wake up packet
                    int reterned_value = client.Send(bytes, 1024);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }


}