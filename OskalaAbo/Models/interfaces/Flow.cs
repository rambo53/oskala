using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OskalaAbo.Models.interfaces
{
    interface Flow
    {
        string from { get; set; }
        string fromName { get; set; }
        string to { get; set; }
        string template { get; set; }
        string password { get; set; }

        

        bool send(string address, string subject, string body);
        bool verifdroit(UserSubscriber userSubscriber);
    }
}