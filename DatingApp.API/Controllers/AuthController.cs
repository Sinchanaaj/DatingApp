using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]  //attribute type routing
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //validate request from user

            //    if(!ModelState.IsValid)
            //    return BadRequest(ModelState); because we are using [apiController]

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();


            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username already exists");

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201); //createdAtRoot should be used instead status code

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            //creation of token here
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
               new Claim(ClaimTypes.NameIdentifier , userFromRepo.Id.ToString()),
               new Claim(ClaimTypes.Name ,userFromRepo.Username)
            };
            //key to sign our token ,it will be hash
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            
            //signing credential key or encryting
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //security token descripter;contains claims, exp date ,signing crede
            var tokenDescripter = new SecurityTokenDescriptor
            {
               Subject=  new ClaimsIdentity(claims),
               Expires = DateTime.Now.AddDays(1), //24 hrs
               SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescripter); 

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });


        }
    }
}