using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DotNetBack.Models;
using Microsoft.Extensions.Configuration;

namespace DotNetBack.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IConfiguration _configuration;

        public MessageRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Response> CreateMessageAsync(Message message)
        {
            Response response = new Response();
            try
            {
                string connectionString = _configuration.GetConnectionString("ppDBCon");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Message (user_id, message, admin_id, is_shown) VALUES (@userId, @message, @adminId, @isShown)", connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", message.UserId);
                        cmd.Parameters.AddWithValue("@message", message.Text);
                        cmd.Parameters.AddWithValue("@adminId", message.AdminId);
                        cmd.Parameters.AddWithValue("@isShown", false);

                        response.Data = await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;
        }


        public async Task<Response> GetUnreadMessagesAsync(int userId)
        {
            Response response = new Response();
            try
            {
            List<Message> messages = new List<Message>();
            string connectionString = _configuration.GetConnectionString("ppDBCon");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Message WHERE user_id = @userId AND is_shown = 0", connection))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            messages.Add(new Message
                            {
                                UserId = userId,
                                IsShown = (bool)reader["is_shown"],
                                Text = reader["message"].ToString(),
                                AdminId = (int)reader["admin_id"]
                            });
                        }
                    }
                }
                response.Data = messages;
            }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
