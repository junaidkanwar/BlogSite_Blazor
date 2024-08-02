using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BlogSite.Models;
using Microsoft.Extensions.Configuration;

public class BlogService
{
    private readonly string _connectionString;

    // Constructor accepting IConfiguration to get connection string
    public BlogService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public bool SubmitBlog(BlogModel model)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new SqlCommand("INSERT INTO Blogs (Title, Content, UserId, SubmittedBy) VALUES (@Title, @Content, @UserId, @SubmittedBy)", connection))
            {
                command.Parameters.AddWithValue("@Title", model.Title);
                command.Parameters.AddWithValue("@Content", model.Content);
                command.Parameters.AddWithValue("@UserId", model.UserId);
                command.Parameters.AddWithValue("@SubmittedBy", model.SubmittedBy);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }

    public List<BlogModel> GetAllBlogs()
    {
        var blogs = new List<BlogModel>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            // Update the query to select all blogs
            using (var command = new SqlCommand("SELECT * FROM Blogs ORDER BY SubmittedBy DESC", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        blogs.Add(new BlogModel
                        {
                            BlogId = reader.GetInt32(reader.GetOrdinal("BlogId")),
                            Title = reader.GetString(reader.GetOrdinal("title")),
                            Content = reader.GetString(reader.GetOrdinal("content")),
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            SubmittedBy = reader.GetString(reader.GetOrdinal("SubmittedBy")) // Use CreatedAt column
                        });
                    }
                }
            }
        }

        return blogs;
    }


    public BlogModel GetBlogById(int blogId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new SqlCommand("SELECT * FROM Blogs WHERE BlogId = @BlogId", connection))
            {
                command.Parameters.AddWithValue("@BlogId", blogId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new BlogModel
                        {
                            BlogId = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            Title = reader.GetString(2),
                            Content = reader.GetString(3),
                            SubmittedBy = reader.GetString(4),
                        };
                    }
                    else
                    {
                        // Blog with the specified ID not found
                        return null;
                    }
                }
            }
        }
    }

    public List<BlogModel> GetBlogs()
    {
        var blogs = new List<BlogModel>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new SqlCommand("SELECT * FROM Blogs", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        blogs.Add(new BlogModel
                        {
                            BlogId = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Content = reader.GetString(2),
                            UserId = reader.GetInt32(3),
                            SubmittedBy = reader.GetString(4)
                        });
                    }
                }
            }
        }

        return blogs;
    }

    public bool RemoveBlog(int blogId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Delete comments related to the blog
                    using (var deleteCommentsCommand = new SqlCommand("DELETE FROM Comments WHERE BlogId = @BlogId", connection, transaction))
                    {
                        deleteCommentsCommand.Parameters.AddWithValue("@BlogId", blogId);
                        deleteCommentsCommand.ExecuteNonQuery();
                    }

                    // Delete the blog
                    using (var deleteBlogCommand = new SqlCommand("DELETE FROM Blogs WHERE BlogId = @BlogId", connection, transaction))
                    {
                        deleteBlogCommand.Parameters.AddWithValue("@BlogId", blogId);
                        int rowsAffected = deleteBlogCommand.ExecuteNonQuery();

                        transaction.Commit();
                        return rowsAffected > 0;
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }


    public List<BlogModel> GetBlogsByUserId(int userId)
    {
        var blogs = new List<BlogModel>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new SqlCommand("SELECT * FROM Blogs WHERE UserId = @UserId", connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        blogs.Add(new BlogModel
                        {
                            BlogId = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            Title = reader.GetString(2),
                            Content = reader.GetString(3),
                            SubmittedBy = reader.GetString(4) // Assuming SubmittedBy is stored as a string in the database
                        });
                    }
                }
            }
        }

        return blogs;
    }
}
