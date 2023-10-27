using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OyoHotelBookings_Api.Dto;
using OyoHotelBookings_Api.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace OyoHotelBookings_Api.Controllers
{
    /// <summary>
    /// Provides APIs related to users
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        public static UserInfo userInfo = new UserInfo();
        private readonly OyoHotelBookingContext dbContext;
        private readonly ILogger<UserController> logger;
        private readonly IConfiguration configuration;
        public UserController(OyoHotelBookingContext dbContext, ILogger<UserController> logger, IConfiguration config)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            configuration = config;
        }
        /// <summary>
        /// gets the users list to admin and gets the specific user based on their userid
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("GetUser")]
        [Authorize]
        public IActionResult GetUser(int userId, bool isAdmin)
        {
            logger.Log(LogLevel.Information, "Method Started :GetUsers");
            IActionResult response = null;
            if (dbContext.Users.ToList() == null)
            {
                throw new Exception("Data Not Available");
            }

            List<User> user = dbContext.Users.ToList();

            if (isAdmin)
            {
                user = dbContext.Users.ToList();
            }
            else
            {
                user = dbContext.Users.Where(x => x.UserId == userId).ToList();
            }
            if (user.Count == 0)
            {
                response = NoContent();
            }
            else
            {
                logger.LogInformation("you opened user controller");
                response = Ok(user);
            }
            logger.Log(LogLevel.Information, "Method end :GetUsers");
            return response;
        }


        /// <summary>
        /// allows to post the user details to login
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("LoginUser")]
        public IActionResult LoginUser(UserInfo user)
        {
            logger.Log(LogLevel.Information, "Method Started :LoginUsers");
            IActionResult response = null;

            User existingUser = dbContext.Users.SingleOrDefault(u => u.UserEmail.Equals(user.UserEmail) && u.UserPassword.Equals(user.Password));
            if (existingUser == null)
            {
                response = BadRequest();
            }
            else
            {
                userInfo.UserName = existingUser.UserName;
                userInfo.UserEmail = existingUser.UserEmail;
                userInfo.UserId = existingUser.UserId;
                userInfo.IsAdmin = (bool)existingUser.IsAdmin;
                string token = CreateToken(user);
                var refreshToken = GenerateRefreshToken(token);
                SetRefreshToken(refreshToken);
                response = Ok(userInfo);
            }
            logger.Log(LogLevel.Information, "Method end :LoginUsers");
            return response;
        }
        /// <summary>
        /// allows the new user to enter their details
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("SignUp")]
        public IActionResult SignUp([FromBody] User user)
        {
            logger.Log(LogLevel.Information, "Method Started :SignUpUsers");
            IActionResult result = null;

            User user3 = dbContext.Users.SingleOrDefault(u => u.UserEmail == user.UserEmail);
            if (user3 == null)
            {
                user.IsAdmin = false;
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
                result = Created("User Added", user);
            }
            else
            {
                result = BadRequest(user);
            }
            User user4 = dbContext.Users.SingleOrDefault(u => u.UserEmail == user.UserEmail);
            if (user4 == null)
            {
                result = StatusCode(404);

            }
            else
            {
                result = Ok();

            }
            logger.Log(LogLevel.Information, "Method end :SignUpUsers");
            return result;
        }

        /// <summary>
        /// allows the users to post their details
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {

            logger.Log(LogLevel.Information, "Method Started : postUsers");
            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            logger.Log(LogLevel.Information, "Method end :postUsers");
            return Created("User Added", user);
        }

        /// <summary>
        /// provides to delete a specific user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            logger.Log(LogLevel.Information, "Method Started :deleteUsers");
            IActionResult result;
            User user = dbContext.Users.Find(id);
            if (user == null)
            {
                result = StatusCode(404, "User not available");
            }
            else
            {
                dbContext.Users.Remove(user);
                dbContext.SaveChanges();

                result = Ok();
            }

            logger.Log(LogLevel.Information, "Method end : postUsers");
            return result;
        }

        /// <summary>
        /// allows to update the details of the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public IActionResult UpdateUser([FromBody] User user)
        {
            logger.Log(LogLevel.Information, "Method Started : updateUsers");
            dbContext.Entry(user).State = EntityState.Modified;
            dbContext.SaveChanges();
            logger.Log(LogLevel.Information, "Method end : updateUsers");
            return Created("User Updated", user);
        }


        /// <summary>
        /// provides the user data based on their email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("GetUserData")]
        [Authorize]
        public IActionResult GetUserData(string email)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.UserEmail == email);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(user);

        }
        private RefreshToken GenerateRefreshToken(string token)
        {
            var refreshToken = new RefreshToken
            {
                Token = token,
                Expires = DateTime.Now.AddDays(30),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires,
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            userInfo.Token = newRefreshToken.Token;
            userInfo.TokenCreated = newRefreshToken.Created;
            userInfo.TokenExpires = newRefreshToken.Expires;
        }

        private string CreateToken(UserInfo user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserEmail),
                new Claim(ClaimTypes.Role, "Admin" )
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}



