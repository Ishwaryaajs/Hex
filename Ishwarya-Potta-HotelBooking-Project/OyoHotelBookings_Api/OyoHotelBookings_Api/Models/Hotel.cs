using System;
using System.Collections.Generic;

namespace OyoHotelBookings_Api.Models;

public partial class Hotel
{
    public int HotelId { get; set; }

    public string HotelName { get; set; }

    public string HotelAddress { get; set; }

    public string HotelCity { get; set; }

    public string HotelState { get; set; }

    public string HotelCountry { get; set; }

    public double HotelRating { get; set; }

    public bool? IsActive { get; set; }

    public bool? RoomAvailable { get; set; }

    public int? RoomId { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
