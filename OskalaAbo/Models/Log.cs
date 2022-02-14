using Grpc.Core.Logging;
using MySql.Data.MySqlClient;
using OskalaAbo.Models.data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace OskalaAbo.Models
{
    public class Log
    {
        public DateTime dateLog { get; set; } = DateTime.Now;
        public TypeLogEnum typeLog { get; set; }
        public UserSubscriber idUsersubscriber { get; set; }
        public UserSubscriber idUserCompany { get; set; }
        public LogLevel logLevel { get; set; }
        public string url { get; set; }
        public string shortMessage { get; set; }
        public string longMessage { get; set; }
        public int errorCode { get; set; }
        public string ipUser { get; set; }


        public static void log(LogLevel logLevel, string url, AResponse result)
        {
            Log log = new Log();

            log.logLevel = logLevel;
            log.url = url;
            log.shortMessage = result.comments;
            log.longMessage = result.errorMessage;
            log.errorCode = result.errorCode;

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    log.ipUser = ip.ToString();
                }
            }

            log.logRegister(log);
            
        }


        const string INSERT_LOG = @"INSERT INTO log VALUES(null, 
                                @logLevel,
                                @url, 
                                @shortMessage, 
                                @longMessage, 
                                @errorCode, 
                                @ipUser, 
                                @date)";

        public AIntegerResponse logRegister(Log log)
        {
            AIntegerResponse result = new AIntegerResponse();

            MySqlConnection connection = new SqlConnect().getConnection();

            using (connection)
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand(INSERT_LOG, connection);

                try
                {

                    command.Parameters.AddWithValue("@logLevel", log.logLevel);
                    command.Parameters.AddWithValue("@url", log.url);
                    command.Parameters.AddWithValue("@shortMessage", log.shortMessage);
                    command.Parameters.AddWithValue("@longMessage", log.longMessage);
                    command.Parameters.AddWithValue("@errorCode", log.errorCode);
                    command.Parameters.AddWithValue("@ipUser", log.ipUser);
                    command.Parameters.AddWithValue("@date", DateTime.Now);

                    command.ExecuteNonQuery();

                    result.isSuccess = true;
                }
                catch (Exception e)
                {
                    result.isSuccess = false;

                }

            }
            return result;
        }

    }
}