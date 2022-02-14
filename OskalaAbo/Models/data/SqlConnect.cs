using Microsoft.Azure;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace OskalaAbo.Models.data
{
    public class SqlConnect
    {
      
        string databaseName = CloudConfigurationManager.GetSetting("databaseSoftena");

        public MySqlConnection getConnection()
        {
            return new MySqlConnection(databaseName);
        }

    }

}