using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OyoHotelBookings_Api.Models;

public partial class OyoHotelBookingContext : DbContext
{
    public OyoHotelBookingContext()
    {
    }

    public OyoHotelBookingContext(DbContextOptions<OyoHotelBookingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserRoleMapping> UserRoleMappings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=Desktop-vuuf228\\sparta;Database=oyo_hotel_booking;integrated security=true; Encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__bookings__5DE3A5B1118E3186");

            entity.ToTable("bookings");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.BookingDate)
                .HasColumnType("datetime")
                .HasColumnName("booking_date");
            entity.Property(e => e.BookingPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("booking_price");
            entity.Property(e => e.CheckinDate)
                .HasColumnType("datetime")
                .HasColumnName("checkin_date");
            entity.Property(e => e.CheckoutDate)
                .HasColumnType("datetime")
                .HasColumnName("checkout_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("isActive");
            entity.Property(e => e.IsApproved)
                .HasDefaultValueSql("((0))")
                .HasColumnName("isApproved");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValueSql("((0))")
                .HasColumnName("user_name");

            entity.HasOne(d => d.Room).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bookings__room_i__2B3F6F97");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bookings__user_i__2C3393D0");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.HotelId).HasName("PK__hotels__45FE7E265D5570DC");

            entity.ToTable("hotels");

            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.HotelAddress)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("hotel_address");
            entity.Property(e => e.HotelCity)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("hotel_city");
            entity.Property(e => e.HotelCountry)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("hotel_country");
            entity.Property(e => e.HotelName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("hotel_name");
            entity.Property(e => e.HotelRating).HasColumnName("hotel_rating");
            entity.Property(e => e.HotelState)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("hotel_state");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("isActive");
            entity.Property(e => e.RoomAvailable)
                .HasDefaultValueSql("((0))")
                .HasColumnName("room_available");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__rooms__19675A8A9E728D90");

            entity.ToTable("rooms");

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.HotelName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("hotel_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("isActive");
            entity.Property(e => e.RoomAvailable).HasColumnName("room_available");
            entity.Property(e => e.RoomCapacity).HasColumnName("room_capacity");
            entity.Property(e => e.RoomPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("room_price");
            entity.Property(e => e.RoomType)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("room_type");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__rooms__hotel_id__286302EC");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370F38972B10");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("isActive");
            entity.Property(e => e.IsAdmin)
                .HasDefaultValueSql("((0))")
                .HasColumnName("isAdmin");
            entity.Property(e => e.UserEmail)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_email");
            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_name");
            entity.Property(e => e.UserPassword)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("user_password");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC2721D40200");

            entity.ToTable("UserRole");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserRoleMapping>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("UserRoleMapping");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.IdNavigation).WithMany()
                .HasForeignKey(d => d.Id)
                .HasConstraintName("FK_UserRoleMapping");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
