using System;

namespace OyoHotelBookings_Api.Dto
{
    public class UserInfo
    {
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Password { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }

        public string Token { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}
