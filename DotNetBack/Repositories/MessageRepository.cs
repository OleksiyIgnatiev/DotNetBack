using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DotNetBack.Models;

namespace DotNetBack.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IConfiguration _configuration;

        public MessageRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task AddMessageAsync(Message message)
        {
            string query = "INSERT INTO Messages (user_id, message, admin_id, is_shown) " +
                "VALUES (@user_id, @message, @admin_id, @is_shown)";
            using (SqlConnection myCon = new SqlConnection(_configuration.GetConnectionString("ppDBCon")))
            {
                SqlCommand myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@user_id", message.UserId);
                myCommand.Parameters.AddWithValue("@message", message.Text);
                myCommand.Parameters.AddWithValue("@admin_id", message.AdminId);
                myCommand.Parameters.AddWithValue("@is_shown", 0); // Установка значения is_shown в 0

                myCon.Open();
                await myCommand.ExecuteNonQueryAsync();
                myCon.Close();
            }
        }

        public async Task<List<Message>> GetUnreadMessagesAsync(int userId)
        {
            string query = "SELECT message_id, message, user_id, admin_id, is_shown FROM Messages WHERE user_id = @user_id AND is_shown = 0";
            List<Message> messages = new List<Message>();

            using (SqlConnection myCon = new SqlConnection(_configuration.GetConnectionString("ppDBCon")))
            {
                SqlCommand myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@user_id", userId);

                myCon.Open();
                SqlDataReader myReader = await myCommand.ExecuteReaderAsync();
                while (await myReader.ReadAsync())
                {
                    messages.Add(Message.Create(
                        myReader.GetInt32(0),   // MessageId
                        myReader.GetString(1),  // Message (Text)
                        myReader.IsDBNull(2) ? (int?)null : myReader.GetInt32(2), // UserId
                        myReader.IsDBNull(3) ? (int?)null : myReader.GetInt32(3), // AdminId
                        myReader.GetBoolean(4)  // IsShown
                    ));
                }
                myReader.Close();
                myCon.Close();
            }
            return messages;
        }
    }
}
