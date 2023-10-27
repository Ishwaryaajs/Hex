using System;
using System.Collections.Generic;

namespace OyoHotelBookings_Api.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public int HotelId { get; set; }

    public string RoomType { get; set; }

    public decimal RoomPrice { get; set; }

    public int RoomCapacity { get; set; }

    public bool RoomAvailable { get; set; }

    public bool? IsActive { get; set; }

    public string HotelName { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Hotel Hotel { get; set; }
}
