using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OskalaAbo.Models
{
    public class AAbonnement
    {
        public int id { get; set; } = -1;
        public string name { get; set; } = "";
        public string description { get; set; } = "";
        public string descriptionAlt { get; set; } = "";
        public string lang { get; set; } = "FR";
        public string url { get; set; } = "";
        public string embedVideo { get; set; } = "";
        public string avatar { get; set; } = "";
        public bool isSpecial { get; set; } = false;
        public string specialDescription { get; set; } = "";
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool active { get; set; } = false;
        public int maxAccounts { get; set; } = 0;
        public int maxStoragrGb { get; set; } = 0;
        public int maxFairUseTransactions { get; set; } = 0;
        public bool isRestricted { get; set; } = false;
        public string offerThemeClass { get; set; } = "";
        public bool freeTrial { get; set; } = true;
        public int tarif { get; set; } = 0;
        public int dureeEngagement { get; set; } = 0;
    }
}