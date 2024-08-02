using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BlogSite.Models;
using Microsoft.Extensions.Configuration;

namespace BlogSite.Data
{
    public class ContactMessageService
    {
        private readonly string _connectionString;

        public ContactMessageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public bool SubmitContactMessage(ContactMessageModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("INSERT INTO ContactMessages (Name, Email, Message, CreatedAt) VALUES (@Name, @Email, @Message, @CreatedAt)", connection))
                {
                    command.Parameters.AddWithValue("@Name", model.Name);
                    command.Parameters.AddWithValue("@Email", model.Email);
                    command.Parameters.AddWithValue("@Message", model.Message);
                    command.Parameters.AddWithValue("@CreatedAt", model.CreatedAt);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public List<ContactMessageModel> GetAllContactMessages()
        {
            var messages = new List<ContactMessageModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("SELECT * FROM ContactMessages", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add(new ContactMessageModel
                            {
                                ContactMessageId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Message = reader.GetString(3),
                                CreatedAt = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }

            return messages;
        }
    }
}
