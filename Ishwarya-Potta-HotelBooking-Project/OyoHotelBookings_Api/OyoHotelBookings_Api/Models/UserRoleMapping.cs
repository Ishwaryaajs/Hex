using System;
using System.Collections.Generic;

namespace OyoHotelBookings_Api.Models;

public partial class UserRoleMapping
{
    public int? Id { get; set; }

    public int? UserId { get; set; }

    public int? RoleId { get; set; }

    public virtual UserRole IdNavigation { get; set; }
}
