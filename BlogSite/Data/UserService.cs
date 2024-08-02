using System;
using System.Data.SqlClient;
using BlogSite.Models;
using Microsoft.Extensions.Configuration;

namespace BlogSite.Data
{
    public class UserService
    {
        private readonly string _connectionString;

        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private int GenerateUserId()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode());
        }

        public bool RegisterUser(UserModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Users (Username, Email, Password) VALUES (@Username, @Email, @Password)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", model.Username);
                    command.Parameters.AddWithValue("@Email", model.Email);
                    command.Parameters.AddWithValue("@Password", model.Password);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool Authenticate(string email, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Construct the SQL command with parameters
                string sql = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password";
                using (var command = new SqlCommand(sql, connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    // Execute the query and retrieve the count
                    int count = (int)command.ExecuteScalar();

                    // If count > 0, user credentials are correct
                    return count > 0;
                }
            }
        }


        public int GetUserIdByUsername(string username)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT UserId FROM Users WHERE Username = @Username";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return (int)result;
                    }
                }
            }

            return -1; // Return -1 if user with the given username is not found
        }


        public string GetUsernameByEmail(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT Username FROM Users WHERE Email = @Email";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    string result = (string)command.ExecuteScalar();
                    return result;
                }
            }
        }

        public UserModel GetUserByUsername(string username)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("SELECT UserId, Username FROM Users WHERE Username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserModel
                            {
                                UserId = reader.GetInt32(0),
                                Username = reader.GetString(1)
                            };
                        }
                    }
                }
            }

            return null;
        }

        public UserModel GetUserById(int userId)
        {
            UserModel user = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Users WHERE UserId = @UserId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new UserModel
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString()
                                // Add other properties as needed
                            };
                        }
                    }
                }
            }

            return user;
        }
    }


}
