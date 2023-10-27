using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    /// Provides APIs related to hotels
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HotelController : ControllerBase
    {
        private readonly OyoHotelBookingContext dbContext;
        private readonly ILogger<HotelController> logger;
        public HotelController(OyoHotelBookingContext dbContext, ILogger<HotelController> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        /// <summary>
        /// Gets all the Hotels 
        /// </summary>
        /// <returns> returns Result</returns>
        /// <exception cref="System.Exception"></exception>
        [HttpGet("GetHotels")]

        public IActionResult GetHotels()
        {
            logger.Log(LogLevel.Information, "Method Started : GetHotels");
            IActionResult result = null;
            if (dbContext.Hotels.ToList() == null)
            {
                throw new System.Exception("No Data Available");
            }
            List<Hotel> hotel = dbContext.Hotels.ToList();
            if (hotel.Count == 0)
            {
                result = NoContent();
            }
            else
            {
                logger.LogInformation("you opened hotel controller");
                result = Ok(hotel);
            }

            logger.Log(LogLevel.Information, "Method end : GetHotels");
            return result;
        }
        /// <summary>
        /// Gets all the Rooms based on same hotelId
        /// </summary>
        [HttpGet("{hotelId}/GetRooms")]
        public IActionResult GetRoomsByHotelId(int hotelId)
        {
            logger.Log(LogLevel.Information, "Method Started: GetRoomsByHotelId");

            try
            {
                var rooms = dbContext.Rooms
                    .Where(r => r.HotelId == hotelId)
                    .ToList();

                if (rooms.Count == 0)
                {
                    logger.LogInformation("No rooms available for this hotel");
                    return NoContent();
                }

                logger.LogInformation("Rooms for the hotel retrieved successfully");
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the request");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request");
            }
        }
        /// <summary>
        /// Gets all the Rooms based on same hotelId
        /// </summary>
        [HttpGet("JoinHotelsAndRooms/{hotelId}")]
        public IActionResult JoinHotelsAndRooms(int hotelId)
        {
            logger.Log(LogLevel.Information, "Method Started: JoinHotelsAndRooms");

            try
            {
                var result = dbContext.Hotels
                    .Where(h => h.HotelId == hotelId)
                    .Join(
                        dbContext.Rooms,
                        hotel => hotel.HotelId,
                        room => room.HotelId,
                        (hotel, room) => new
                        {


                            RoomType = room.RoomType,
                            RoomPrice = room.RoomPrice,
                            RoomCapacity = room.RoomCapacity,
                            RoomAvailability = room.RoomAvailable

                        }
                    )
                    .ToList();

                if (result.Count == 0)
                {
                    logger.LogInformation("No data available for the provided hotelId");
                    return NoContent();
                }

                logger.LogInformation("Hotels and rooms joined successfully");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the request");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request");
            }
        }
        /// <summary>
        /// Gets all the names of the hotel
        /// </summary>
        [HttpGet("names")]
        public ActionResult<IEnumerable<string>> GetHotelNames()
        {
            try
            {
                var hotelNames = dbContext.Hotels.Select(h => h.HotelName).ToList();
                if (hotelNames.Count == 0)
                {
                    return NotFound("No hotel names found.");
                }

                return Ok(hotelNames);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return StatusCode(500, "An error occurred while fetching hotel names.");
            }
        }
        /// <summary>
        /// allows to post the new hotel
        /// </summary>
        /// <param name="hotel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] Hotel hotel)
        {
            logger.Log(LogLevel.Information, "Method started : postHotels");
            dbContext.Hotels.Add(hotel);
            dbContext.SaveChanges();
            logger.Log(LogLevel.Information, "Method end : postHotels");
            return Created("Hotel Info Added", hotel);
        }

        /// <summary>
        /// Deletes the hotel by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            logger.Log(LogLevel.Information, "Method started : deleteHotels");
            IActionResult result;
            Hotel hotel = dbContext.Hotels.Find(id);
            if (hotel == null)
            {
                result = StatusCode((int)HttpStatusCode.NotFound, "Hotels not available");
            }
            else
            {
                dbContext.Hotels.Remove(hotel);
                dbContext.SaveChanges();

                result = Ok();

            }
            logger.Log(LogLevel.Information, "Method end : deleteHotels");
            return result;
        }
        /// <summary>
        /// allows to update the hotel details
        /// </summary>
        /// <param name="hotel"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public IActionResult UpdateHotel([FromBody] Hotel hotel)
        {
            logger.Log(LogLevel.Information, "Method started : updateHotels");
            dbContext.Entry(hotel).State = EntityState.Modified;
            dbContext.SaveChanges();
            logger.Log(LogLevel.Information, "Method end : updateHotels");
            return Created("Hotel Updated", hotel);

        }
    }
}
