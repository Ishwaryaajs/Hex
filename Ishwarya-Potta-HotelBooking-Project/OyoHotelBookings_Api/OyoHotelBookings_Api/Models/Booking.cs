using System;
using System.Collections.Generic;

namespace OyoHotelBookings_Api.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int RoomId { get; set; }

    public int UserId { get; set; }

    public DateTime CheckinDate { get; set; }

    public DateTime CheckoutDate { get; set; }

    public decimal BookingPrice { get; set; }

    public DateTime BookingDate { get; set; }

    public bool? IsActive { get; set; }

    public string UserName { get; set; }

    public bool? IsApproved { get; set; }

    public virtual Room Room { get; set; }

    public virtual User User { get; set; }
}
