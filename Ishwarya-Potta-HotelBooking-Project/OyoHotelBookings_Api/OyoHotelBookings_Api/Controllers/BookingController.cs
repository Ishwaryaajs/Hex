using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OyoHotelBookings_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace OyoHotelBookings_Api.Controllers
{
    /// <summary>
    /// Provides APIs related to hotel bookings
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly OyoHotelBookingContext dbContext;
        private readonly ILogger<BookingController> logger;
        public BookingController(OyoHotelBookingContext dbContext, ILogger<BookingController> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        /// <summary>
        /// Gets all the bookings 
        /// </summary>
        /// <returns>returns Bookings</returns>
        [HttpGet("GetBookings")]

        public IActionResult GetBookings(int userId, bool isAdmin)
        {
            logger.Log(LogLevel.Information, "Method Started : GetBookings");
            IActionResult result = null;
            try
            {
                if (dbContext.Bookings.ToList() == null)
                {
                    throw new Exception("No Bookings Data Available");
                }
                List<Booking> bookings = new List<Booking>();
                if (isAdmin)
                {
                    bookings = dbContext.Bookings.Where(x => !(bool)x.IsApproved).ToList();
                }
                else
                {
                    bookings = dbContext.Bookings.Where(x => x.UserId == userId).ToList();
                }
                var finalBookings = from b in bookings
                                    join u in dbContext.Users.ToList()
                                    on b.UserId equals u.UserId
                                    join r in dbContext.Rooms.ToList()
                                   on b.RoomId equals r.RoomId
                                    join h in dbContext.Hotels.ToList()
                                   on r.HotelId equals h.HotelId

                                    select new
                                    {
                                        IsApproved = b.IsApproved,
                                        UserId = b.UserId,
                                        BookingDate = b.BookingDate,
                                        BookingId = b.BookingId,
                                        BookingPrice = b.BookingPrice,
                                        CheckinDate = b.CheckinDate,
                                        CheckoutDate = b.CheckoutDate,
                                        UserName = u.UserName,
                                        RoomId = b.RoomId,
                                        RoomType = r.RoomType,
                                        HotelName = h.HotelName,
                                        HotelAddress = h.HotelAddress,
                                        RoomPrice = r.RoomPrice





                                    };
                if (bookings.Count == 0)
                {
                    result = NoContent();
                }
                else
                {
                    result = Ok(finalBookings);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
            logger.Log(LogLevel.Information, "Method End : GetBookings");
            return result;
        }
        /// <summary>
        /// Saves a Hotel bookings based on provided details
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] Booking booking)
        {
            logger.Log(LogLevel.Information, "Method Started : PostBookings");
            var isRoomBooked = dbContext.Bookings.Where(x => x.RoomId == booking.RoomId
                               && (x.CheckinDate.Month == booking.CheckinDate.Month
                                   && x.CheckinDate.Year == booking.CheckinDate.Year
                                   )
                               && (x.CheckoutDate.Month == booking.CheckoutDate.Month
                                   && x.CheckoutDate.Year == booking.CheckoutDate.Year
                                   )
                               && !(
                                   (
                                   x.CheckinDate.Day > booking.CheckoutDate.Day
                                   && x.CheckoutDate.Day > booking.CheckoutDate.Day
                                   )
                                  ||
                                  (
                                   x.CheckinDate.Day < booking.CheckinDate.Day
                                   && x.CheckoutDate.Day < booking.CheckinDate.Day
                                   )
                                 )
                               ).Count() > 0;
            if (isRoomBooked)
            {
                return StatusCode(404, "Selected room is already booked");
            }
            else
            {
                booking.BookingDate = DateTime.Now;
                dbContext.Bookings.Add(booking);
                dbContext.SaveChanges();
                logger.Log(LogLevel.Information, "Method end : PostBookings");
                return Created("Booking Added successfully", booking);
            }
        }



        /// <summary>
        /// Deletes a hotel booking base on booking id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>returns </returns>
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            logger.Log(LogLevel.Information, "Method Started : DeleteBookings");
            IActionResult result;
            Booking bookings = dbContext.Bookings.Find(id);
            if (bookings == null)
            {
                result = StatusCode((int)HttpStatusCode.NotFound, "Booking not available");
            }
            else
            {
                dbContext.Bookings.Remove(bookings);
                dbContext.SaveChanges();
                result = Ok();

            }
            logger.Log(LogLevel.Information, "Method end : DeleteBookings");
            return result;
        }

        /// <summary>
        /// Updates hotel booking details based on provided booking details
        /// </summary>
        /// <param name="booking"></param>
        /// <returns>returns result</returns>
        [HttpPut("Update")]
        public IActionResult UpdateBooking([FromBody] Booking booking)
        {
            logger.Log(LogLevel.Information, "Method Started : updateBookings");

            dbContext.Entry(booking).State = EntityState.Modified;
            dbContext.SaveChanges();

            logger.Log(LogLevel.Information, "Method Started : updateBookings");
            return Created("booking Updated", booking);
        }

        /// <summary>
        /// Updates hotel booking status based on admin's approval and rejection
        /// </summary>
        /// <param name="booking"></param>
        /// <returns>returns result</returns>
        [HttpPut("UpdateStatus")]
        public IActionResult UpdateBookingStatus([FromBody] Booking booking)
        {

            Booking bookings = dbContext.Bookings.Find(booking.BookingId);


            if (bookings == null)
            {
                return NotFound();
            }

            bookings.IsApproved = booking.IsApproved; 

            try
            {
                dbContext.SaveChanges();
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error updating the booking status.");
            }
        }



    }
}

