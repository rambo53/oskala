using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace OskalaAbo.Models.interfaces
{
    interface SqlConnector 
    {
        bool openConnnection();
        void CloseConnection();
    }
}