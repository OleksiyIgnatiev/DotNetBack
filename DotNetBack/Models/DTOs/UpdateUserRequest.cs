﻿namespace DotNetBack.Models.DTOs
{
    public class UpdateUserRequest
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
