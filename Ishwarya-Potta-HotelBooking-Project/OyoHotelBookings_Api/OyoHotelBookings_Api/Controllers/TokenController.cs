using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.AspNetCore.Http;

namespace OyoHotelBookings_Api.Controllers
{
    public class TokenController : Controller
    {
        public ActionResult GenerateJwtToken(string username, string password)
        {
            // Generate a JWT token for the user.
            var token = "";
            //var token = JwtSecurityTokenHandler.CreateJwtSecurityToken(
            //    issuer: "https://example.com",
            //    audience: "https://example.com",
            //    ClaimsIdentity: new Claim[] { new Claim("username", username) },
            //    expires: DateTime.Now.AddDays(1),
            //     signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secret")),"SHA")
            //);

            // Return the JWT token.
            return Ok(new { token = token.ToString() });
        }
    }
}
