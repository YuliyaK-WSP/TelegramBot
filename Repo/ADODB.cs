using System.Runtime.InteropServices.ComTypes;
using System.Reflection.PortableExecutable;
using System.Buffers.Text;
using System.Reflection.Metadata;
using System.Data;
using System.Configuration;
using System;
using TelegramBot.Models;
using TelegramBot.Enums;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegramBot.Repo
{
    public static class ADODB
    {
        static string SqlConnectionString = ConfigurationManager.AppSettings["SqlConnect"];

        public static List<User> GetUsers()
        {
            var users = new List<User>();
            using (var conn = new NpgsqlConnection(SqlConnectionString))
            {
                string sql = @"select id,first_name,last_name,user_role from users";
                conn.Open();

                using (var command = new NpgsqlCommand(sql, conn))
                {
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string userRoleStr =
                            char.ToUpper(reader["user_role"].ToString()[0])
                            + reader["user_role"].ToString().Substring(1);
                        Console.WriteLine(reader["user_role"].ToString());
                        users.Add(
                            new User()
                            {
                                Id = Convert.ToInt32(reader["id"].ToString()),
                                FirstName = reader["first_Name"].ToString(),
                                LastName = reader["last_Name"].ToString(),
                                UserRole = (UserRoles)Enum.Parse(typeof(UserRoles), userRoleStr)
                            }
                        );
                    }
                }
            }
            return users;
        }

        public static string GetUserRole(long chatId)
        {
            string userRole = null;
            using (var conn = new NpgsqlConnection(SqlConnectionString))
            {
                string sql = @"select user_role from users where chat_id = @chatId";
                conn.Open();
                using (var command = new NpgsqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@chatId", chatId);
                    /*var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        userRole = result.ToString();
                    }*/
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        return reader["user_role"].ToString();
                    }
                }
            }
            return userRole;
        }

        public static string GetUserSpecialict(long chatId)
        {
            string userSpecialict = null;
            using (var conn = new NpgsqlConnection(SqlConnectionString))
            {
                string sql = @"select specialict from requests where chat_id = @chatId";
                conn.Open();
                using (var command = new NpgsqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@chatId", chatId);
                    /*var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        userRole = result.ToString();
                    }*/
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        return reader["specialict"].ToString();
                    }
                }
            }
            return userSpecialict;
        }

        public static string GetRequestUser(long chatId)
        {
            string userRequests = null;
            using (var conn = new NpgsqlConnection(SqlConnectionString))
            {
                string sql =
                    @"select number,specialist,description from requests where chat_id = @chatId";
                conn.Open();
                using (var command = new NpgsqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@chatId", chatId);
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        userRequests = result.ToString();
                    }
                }
            }
            return userRequests;
        }

        public static void AddUsers(User user, long chatId)
        {
            using (var conn = new NpgsqlConnection(SqlConnectionString))
            {
                string sql =
                    @"insert into users (chat_id,user_role,first_name,last_name,patronymic,phone_number,city,street,house_number) values (@chatId,@userRole,@firstName,@lastName,@patronymic,@phoneNumber,@city,@street,@houseNumber)";
                conn.Open();
                using (var command = new NpgsqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@chatId", chatId);
                    command.Parameters.AddWithValue("@userRole", user.UserRole);
                    command.Parameters.AddWithValue("@firstName", user.FirstName);
                    command.Parameters.AddWithValue("@lastName", user.LastName);
                    command.Parameters.AddWithValue(
                        "@patronymic",
                        user.Patronymic ?? (object)DBNull.Value
                    );
                    command.Parameters.AddWithValue("@phoneNumber", user.PhoneNumber);
                    command.Parameters.AddWithValue("@city", user.City);
                    command.Parameters.AddWithValue("@street", user.Street);
                    command.Parameters.AddWithValue("@houseNumber", user.HouseNumber);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void AddRequestUser(Request request)
        {
            using (var conn = new NpgsqlConnection(SqlConnectionString))
            {
                string sqlRequest =
                    @"insert into requests (specialist,description) values (@specialist::specialist,@description)";
                conn.Open();
                using (var command = new NpgsqlCommand(sqlRequest, conn))
                {
                    command.Parameters.AddWithValue("@specialist", request.Specialist);
                    command.Parameters.AddWithValue("@description", request.Description);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteUser(long chatId)
        {
            using (var conn = new NpgsqlConnection(SqlConnectionString))
            {
                string sql = @"delete from users where chat_id = @chatId";
                conn.Open();
                using (var command = new NpgsqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@chatId", chatId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
