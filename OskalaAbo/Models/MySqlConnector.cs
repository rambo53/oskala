using Microsoft.Azure;
using MySql.Data.MySqlClient;
using OskalaAbo.Models.interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace OskalaAbo.Models
{

    public class MySqlConnector 
    {
        private MySqlConnection connectionSoftena { get; set ; }
        private MySqlConnection connectionDatabaseClient { get; set; }
        private MySqlParameter parameter { get; set; }
        private MySqlCommand command { get; set; }
        private MySqlTransaction transaction { get; set; }


        public MySqlConnector(bool withTransac) 
        {
            connectionSoftena = new MySqlConnection(CloudConfigurationManager.GetSetting("databaseSoftena"));
            
            if (withTransac)
            {
                connectionSoftena.Open();
                transaction = connectionSoftena.BeginTransaction();
            }
        }

        public MySqlConnection GetConnection()
        {
            return connectionSoftena;
        }

        public MySqlTransaction GetTransaction()
        {
            return transaction;
        }



        public void Commit()
        {
            if(transaction != null)
            {
                transaction.Commit();
            }
        }

        private bool OpenConnection()
        {
            if(connectionSoftena == null)
            {
                try
                {
                    connectionSoftena.Open();
                    return connectionSoftena.State == ConnectionState.Open || connectionSoftena.State == ConnectionState.Connecting;     
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    return false;
                }  
                
            }
            return true;
        }


        public void CloseConnection()
        {
            if (connectionSoftena != null)
            {
                connectionSoftena.Close();
            }
        }



        private void closeTransaction()
        {
            try
            {
                transaction.Rollback();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Rollback Exception Type: {0}", ex.GetType());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
        }
            
        public ALongResponse Insert(string tableName, Dictionary<string, object> dic, bool returnID)
        {
            ALongResponse result = new ALongResponse();
            result.result = -1;
            result.comments = "Insert sur table " + tableName;

            try
            {
                if (OpenConnection())
                {

                    //command = connectionSoftena.CreateCommand();

                    var dresult = Insert(connectionSoftena, tableName, dic, returnID);

                    if (dresult.isSuccess)
                    {
                        result = dresult;
                    }

                }

            }
            catch (MySqlException ex)
            {

                result.errorMessage = ex.Message;
                result.errorCode = ex.Number;
                this.closeTransaction();
                this.CloseConnection();
                
            }
            catch (Exception ex)
            {
                result.errorMessage = ex.Message;
                result.errorCode = (int)HttpStatusCode.InternalServerError;
                this.closeTransaction();
                this.CloseConnection();
            }
            return result;
        }



        public static ALongResponse Insert(MySqlConnection context, string tableName, Dictionary<string, object> dic, bool returnID)
        {
            MySqlCommand cmd = null;
            ALongResponse result = new ALongResponse();
            try
            {
                if (context != null)
                {
                    if (!(context.State == ConnectionState.Open || context.State == ConnectionState.Connecting)) context.Open();

                    cmd = context.CreateCommand();

                    return Insert(cmd, tableName, dic, returnID);
                }
                else
                {
                    result.errorMessage = "DB Insert FAILED No context";
                    result.errorCode = (int)HttpStatusCode.InternalServerError;
                }
            }
            catch (Exception ex)
            {
                result.errorMessage = "DB Insert FAILED " + ex.Message + " " + cmd.CommandText;
                result.errorCode = (int)HttpStatusCode.InternalServerError;
            }
            return result;
        }

        public static ALongResponse Insert(MySqlCommand cmd, string tableName, Dictionary<string, object> dic, bool returnID)
        {
            ALongResponse result = new ALongResponse();
            try
            {
                if (cmd != null)
                {
                    StringBuilder sbCol = new StringBuilder();
                    StringBuilder sbVal = new StringBuilder(); 

                    foreach (KeyValuePair<string, object> kv in dic)
                    {
                        if (sbCol.Length == 0)
                        {
                            sbCol.Append("insert into ");
                            sbCol.Append(tableName);
                            sbCol.Append("(");
                        }
                        else
                        {
                            sbCol.Append(",");
                        }
                        sbCol.Append("`");
                        sbCol.Append(kv.Key);
                        sbCol.Append("`"); 
                        
                        if (sbVal.Length == 0)
                        {
                            sbVal.Append(" values(");
                        }
                        else
                        {
                            sbVal.Append(", ");
                        }
                        sbVal.Append("@v");
                        sbVal.Append(kv.Key);
                    }
                    sbCol.Append(") ");
                    sbVal.Append(");"); 

                    cmd.CommandText = sbCol.ToString() + sbVal.ToString(); 
                    
                    foreach (KeyValuePair<string, object> kv in dic)
                    {

                        CoreExtensions.AddParameterWithValue(cmd, "@v" + kv.Key, kv.Value);
                    }
                    long inserts = cmd.ExecuteNonQuery(); 
                    
                    if (inserts > 0 && returnID)
                    {
                        result.result = cmd.LastInsertedId;
                        result.isSuccess = true;
                    }
                    else
                    {
                        result.result = inserts;
                        result.isSuccess = inserts > 0;
                    }
                }
                else
                {
                    result.errorMessage = "DB Insert FAILED No command";
                    result.errorCode = (int)HttpStatusCode.InternalServerError;
                }
            }
            catch (MySqlException ex)
            {
                result.errorMessage = ex.Message;
                result.errorCode = ex.Number;
            }
            catch (Exception ex)
            {
                result.errorMessage = "DB Insert Exception" + ex.Message + " " + cmd.CommandText;
                result.errorCode = (int)HttpStatusCode.InternalServerError;
            }
            return result;
        }



    }
}