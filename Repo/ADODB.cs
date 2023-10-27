using System.ComponentModel;
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

        public static User GetUserInfo(long chatId)
        {
            User userInfo;
            using (var conn = new NpgsqlConnection(SqlConnectionString))
            {
                string sql = @"select id, first_name, last_name, user_role, specialist from users where chat_id = @chatId";
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
						string userRoleStr =
                            char.ToUpper(reader["user_role"].ToString()[0])
                            + reader["user_role"].ToString().Substring(1);
						string specialist =
                            char.ToUpper(reader["specialist"].ToString()[0])
                            + reader["specialist"].ToString().Substring(1);
						userInfo = new User()
                            {
                                Id = Convert.ToInt32(reader["id"].ToString()),
                                FirstName = reader["first_Name"].ToString(),
                                LastName = reader["last_Name"].ToString(),
                                UserRole = (UserRoles)Enum.Parse(typeof(UserRoles), userRoleStr),
								Specialist = (Specialist)Enum.Parse(typeof(Specialist), specialist)
                            };
                        return userInfo;
                    }
                }
            }
            return null;
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

        public static List<Request> GetRequestUser(long userId)
        {
            var userRequests = new List<Request>();
            using (var conn = new NpgsqlConnection(SqlConnectionString))
            {
                string sql =
                    @"select number, specialist, description, status from requests where user_id = @userId";
                conn.Open();
                using (var command = new NpgsqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
						string specialist =
                            char.ToUpper(reader["specialist"].ToString()[0])
                            + reader["specialist"].ToString().Substring(1);
						string status =
							char.ToUpper(reader["status"].ToString()[0])
                            + reader["status"].ToString().Substring(1);
                        userRequests.Add(
                            new Request()
                            {
                                Number = Convert.ToInt32(reader["number"]),
                                Specialist = (Specialist)
                                    Enum.Parse(typeof(Specialist), specialist),
                                Description = reader["description"].ToString(),
                                //Urgency = reader["urgency"].ToString(),
                                Status = (RequestStatus)
                                    Enum.Parse(typeof(RequestStatus), status)
                            }
                        );
                    }
                }
            }
            return userRequests;
        }

		public static List<Request> GetRequestExecutor(Specialist specialist)
        {
            var executorRequests = new List<Request>();
            using (var conn = new NpgsqlConnection(SqlConnectionString))
            {
                string sql =
                    @"select number, description, status from requests where specialist = @specialist::specialist and status = @status::request_status" ;
                conn.Open();
                using (var command = new NpgsqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@specialist", specialist);
					command.Parameters.AddWithValue("@status", RequestStatus.New);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
						string status =
							char.ToUpper(reader["status"].ToString()[0])
                            + reader["status"].ToString().Substring(1);
                        executorRequests.Add(
                            new Request()
                            {
                                Number = Convert.ToInt32(reader["number"]),
                                Specialist = specialist,
                                //Urgency = reader["urgency"].ToString(),
                                Status = (RequestStatus)
                                    Enum.Parse(typeof(RequestStatus), status)
                            }
                        );
                    }
                }
            }
            return executorRequests;
        }

        public static void AddUsers(User user, long chatId)
        {
            using (var conn = new NpgsqlConnection(SqlConnectionString))
            {
                string sql =
                    @"insert into users (chat_id,user_role,first_name,last_name,patronymic,phone_number,city,street,house_number, specialist) values (@chatId,@userRole,@firstName,@lastName,@patronymic,@phoneNumber,@city,@street,@houseNumber, @specialist::specialist)";
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
					command.Parameters.AddWithValue("@specialist", Specialist.Other);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void AddRequestUser(Request request)
        {
            using (var conn = new NpgsqlConnection(SqlConnectionString))
            {
                string sqlRequest =
                    @"insert into requests (specialist,description, user_id, status) values (@specialist::specialist,@description, @userId, @status::request_status)";
                conn.Open();
                using (var command = new NpgsqlCommand(sqlRequest, conn))
                {
                    command.Parameters.AddWithValue("@specialist", request.Specialist.ToString());
                    command.Parameters.AddWithValue("@description", request.Description);
					command.Parameters.AddWithValue("@userId", Handlers.userInfo.Id);
					command.Parameters.AddWithValue("@status", RequestStatus.New);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteUser(int userId)
        {
            using (var conn = new NpgsqlConnection(SqlConnectionString))
            {
                string sql = @"delete from users where id = @userId";
                conn.Open();
                using (var command = new NpgsqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
