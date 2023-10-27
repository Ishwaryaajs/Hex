using System;
using System.Collections.Generic;

namespace OyoHotelBookings_Api.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; }

    public string UserEmail { get; set; }

    public string UserPassword { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsAdmin { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
