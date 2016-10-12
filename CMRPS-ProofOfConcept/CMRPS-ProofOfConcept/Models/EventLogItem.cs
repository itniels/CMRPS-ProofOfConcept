using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMRPS_ProofOfConcept.Models
{
    public class EventLogItem
    {
        public DateTime Time { get; set; }
        public string Action { get; set; }
        public List<string> listOfData { get; set; }
        public string Exception { get; set; }
        public bool Success { get; set; }
    }
}