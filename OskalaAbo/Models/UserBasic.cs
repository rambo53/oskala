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
    public class UserBasic
    {
        [Key]
        public int Id { get; set; } = 0;

        [MinLength(2, ErrorMessage = "Le nom est obligatoire. (minimum : 2 caractères)")]
        [MaxLength(50, ErrorMessage = "Le nom est trop grand. (maximum : 50 caractères)")]
        [RegularExpression(@"^[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð, .'-]+$",
        ErrorMessage = "Le nom n'est pas au bon format.")]
        public string userLastName { get; set; } = "user";

        public string userMail { get; set; } = "";

        public RoleUserEnum role { get; set; }  = RoleUserEnum.USER;

        public bool isActive { get; set; } = false;




        const string UPDATE_ACTIVE_NEW_USER = "UPDATE user SET active = 1 WHERE id= @id";

        public AIntegerResponse updateActiveNewUser(int id)
        {
            AIntegerResponse userActiveUpdate = new AIntegerResponse();
            MySqlConnection connection = new SqlConnect().getConnection();

            using (connection)
            {

                try
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(UPDATE_ACTIVE_NEW_USER, connection);

                    command.Parameters.AddWithValue("@id", id);

                    int update = command.ExecuteNonQuery();

                    if (update > 0)
                    {
                        userActiveUpdate.result = update;
                        userActiveUpdate.isSuccess = true;
                    }
                    else
                    {
                        userActiveUpdate.result = update;
                    }

                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                }

            }

            return userActiveUpdate;
        }


        const string GET_MAIL = "SELECT mail FROM contact WHERE mail = @mail LIMIT 1";

        public AResponse getMail(string mail)
        {
            AResponse result = new AResponse();
            MySqlConnection connection = new SqlConnect().getConnection();

            using (connection)
            {
                try
                {
                    connection.Open();

                    MySqlParameter[] command = new MySqlParameter[1];

                    command[0] = new MySqlParameter("@mail", mail);

                    DataSet ds = MySqlHelper.ExecuteDataset(connection, GET_MAIL, command);
                    DataTable table = ds.Tables[0];

                    if (table.Rows.Count > 0)
                    {
                        result.isSuccess = true;
                        result.errorMessage = "Cette adresse mail est déjà utilisé.";
                    }
                    else
                    {
                        result.isSuccess = false;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                }
            }

            return result;
        }
    }
}