using MySql.Data.MySqlClient;
using OskalaAbo.Models.data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace OskalaAbo.Models
{
    public class UserLogin : UserBasic
    {

        public string userPassword { get; set; } = "";

        public AuthentificationMethodeEnum authentificationMethode { get; set; } = AuthentificationMethodeEnum.SOFTENA;




        const string GET_USER_BY_MAIL_LOGIN = "SELECT id, last_name, active, mail FROM user WHERE mail = @mail AND password = @password LIMIT 1";

        public AObjectTResponse<UserBasic> getUserByMailLogin(string mail, string password)
        {
            MySqlConnection connection = new SqlConnect().getConnection();
            AObjectTResponse<UserBasic> user = null;

            using (connection)
            {

                try
                {
                    connection.Open();

                    MySqlParameter[] command = new MySqlParameter[2];

                    command[0] = new MySqlParameter("@password", password);
                    command[1] = new MySqlParameter("@mail", mail);

                    DataSet ds = MySqlHelper.ExecuteDataset(connection, GET_USER_BY_MAIL_LOGIN, command);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var a = from u in ds.Tables[0].AsEnumerable() select new UserBasic(){
                        Id = u.Field<int>("id"),
                        userLastName = u.Field<string>("last_name"),
                        isActive = u.Field<byte>("active")==1,
                        userMail = mail,
                        };

                        user = new AObjectTResponse<UserBasic>();

                        user.result = a.ToArray()[0];

                        user.isSuccess = true;

                    }

                }
                catch (Exception e)
                {
                    user = new AObjectTResponse<UserBasic>();
                    user.errorMessage = "Ce compte n'existe pas.";
                }

            }

            return user;
        }
    }
}