using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BlogSite.Models;
using Microsoft.Extensions.Configuration;

public class CommentService
{
    private readonly string _connectionString;

    // Constructor accepting IConfiguration to get connection string
    public CommentService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public bool SubmitComment(CommentModel model)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new SqlCommand("INSERT INTO Comments (BlogId, UserId, CommentContent, Author) VALUES (@BlogId, @UserId, @CommentContent, @Author)", connection))
            {
                command.Parameters.AddWithValue("@BlogId", model.BlogId);
                command.Parameters.AddWithValue("@UserId", model.UserId);
                command.Parameters.AddWithValue("@CommentContent", model.CommentContent);
                command.Parameters.AddWithValue("@Author", model.Author);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }

    public List<CommentModel> GetCommentsForBlog(int blogId)
    {
        var comments = new List<CommentModel>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new SqlCommand("SELECT CommentId, BlogId, UserId, CommentContent, Author FROM Comments WHERE BlogId = @BlogId", connection))
            {
                command.Parameters.AddWithValue("@BlogId", blogId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comments.Add(new CommentModel
                        {
                            CommentId = reader.GetInt32(reader.GetOrdinal("CommentId")),
                            BlogId = reader.GetInt32(reader.GetOrdinal("BlogId")),
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            CommentContent = reader.GetString(reader.GetOrdinal("CommentContent")),
                            Author = reader.GetString(reader.GetOrdinal("Author")) // Retrieve 'Author' from the Comments table
                        });
                    }
                }
            }
        }

        return comments;
    }

}
