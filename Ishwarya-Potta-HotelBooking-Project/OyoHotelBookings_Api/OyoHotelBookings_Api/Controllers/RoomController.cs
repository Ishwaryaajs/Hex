using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OyoHotelBookings_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OyoHotelBookings_Api.Controllers
{
    /// <summary>
    /// Provides APIs related to hotel rooms
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly OyoHotelBookingContext dbContext;
        private readonly ILogger<RoomController> logger;
        public RoomController(OyoHotelBookingContext dbContext, ILogger<RoomController> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        [HttpGet("GetAvailableRooms")]
        [Authorize]
        public IActionResult GetAvailableRooms(int hotelId, DateTime checkInDate, DateTime checkOutDate)
        {
            try
            {
                var availableRooms = dbContext.Rooms
                    .Where(room =>
                        room.HotelId == hotelId &&
                        !dbContext.Bookings.Any(booking =>
                            booking.RoomId == room.RoomId &&
                            (checkInDate < booking.CheckoutDate && checkOutDate > booking.CheckinDate)))
                    .ToList();

                return Ok(availableRooms);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "An error occurred while fetching available rooms.");
            }
        }

        /// <summary>
        /// gets all the rooms
        /// </summary>
        /// <returns>returns result</returns>
        /// <exception cref="System.Exception"></exception>
        [HttpGet("GetRooms")]

        public IActionResult GetRooms()
        {
            logger.Log(LogLevel.Information, "Method Started : GetRooms");
            IActionResult result = null;
            if (dbContext.Rooms.ToList() == null)
            {
                throw new System.Exception("Data not available");
            }
            List<Room> room = dbContext.Rooms.ToList();
            if (room.Count == 0)
            {
                result = NoContent();
            }
            else
            {
                logger.LogInformation("you opened room controller");
                result = Ok(room);
            }
            logger.Log(LogLevel.Information, "Method end  : GetRooms");
            return result;
        }
        /// <summary>
        /// gets all the rooms by its hotelid
        /// </summary>
        [HttpGet("{hotelId}")]
        public IActionResult GetRoomsByHotelId(int hotelId)
        {
            List<Room> room = dbContext.Rooms.ToList();
            var rooms = room.Where(r => r.HotelId == hotelId).ToList();
            return Ok(rooms);
        }

        /// <summary>
        /// Get the number of available rooms.
        /// </summary>
        /// <returns></returns>
        [HttpGet("RoomAvailability")]
     
        public IActionResult GetRoomAvailability()
        {
            logger.Log(LogLevel.Information, "Method Started: GetRoomAvailability");

          
            int totalRooms = dbContext.Rooms.Count();
            int bookedRooms = dbContext.Bookings.Count();
            int availableRooms = totalRooms - bookedRooms;

            
            var availabilityResponse = new
            {
                TotalRooms = totalRooms,
                BookedRooms = bookedRooms,
                AvailableRooms = availableRooms
            };

            logger.LogInformation($"Total Rooms: {totalRooms}, Booked Rooms: {bookedRooms}, Available Rooms: {availableRooms}");

            logger.Log(LogLevel.Information, "Method end: GetRoomAvailability");

            return Ok(availabilityResponse);
        }


        /// <summary>
        /// allows to post the new room
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] Room room)
        {
            logger.Log(LogLevel.Information, "Method Started : PostRooms");
            dbContext.Rooms.Add(room);
            dbContext.SaveChanges();
            logger.Log(LogLevel.Information, "Method end : PostRooms");
            return Created("User Added", room);
        }

        /// <summary>
        /// Deletes the room by it's Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            logger.Log(LogLevel.Information, "Method Started : deleteRooms");
            IActionResult result = null;
            Room room = dbContext.Rooms.Find(id);
            if (room == null)
            {
                result = StatusCode(404, "Room not available");
            }
            else
            {
                dbContext.Rooms.Remove(room);
                dbContext.SaveChanges();
                result = Ok();
            }
            logger.Log(LogLevel.Information, "Method end : deleteRooms");
            return result;
        }
        /// <summary>
        /// creates a room based on the hotelName
        /// </summary>
        /// <param name="hotelName"></param>
        /// <returns>new Room</returns>
        [HttpPost("CreateRoom/{hotelName}")]
        public IActionResult Post(string hotelName, [FromBody] Room room)
        {
            logger.Log(LogLevel.Information, "Method Started : PostRooms");

            var hotel = dbContext.Hotels.SingleOrDefault(h => h.HotelName == hotelName);

            if (hotel == null)
            {
                return NotFound($"Hotel '{hotelName}' not found.");
            }

            room.HotelId = hotel.HotelId;

            
            dbContext.Rooms.Add(room);
            dbContext.SaveChanges();

            logger.Log(LogLevel.Information, "Method end : PostRooms");

            return Created("Room Added", room);
        }



        /// <summary>
        /// updates the details of the exitising room
        /// </summary>
        /// <param name="room"></param>
        /// <returns>created room</returns>
        [HttpPut("Update")]
        [Authorize]
        public IActionResult UpdateRoom([FromBody] Room room)
        {
            logger.Log(LogLevel.Information, "Method Started : updateRooms");
            dbContext.Entry(room).State = EntityState.Modified;
            dbContext.SaveChanges();
            logger.Log(LogLevel.Information, "Method end : updateRooms");
            return Created("User Updated", room);
        }
    }
}