#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookstoreAPI.Models;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace BookstoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BookStoreContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(BookStoreContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Users
        [HttpGet, Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // PUT: api/Users/5
        [HttpPut("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> PutUser(int id, UserDto user)
        {

            var selectedUser = await (from u in _context.Users
                                      where u.Id == user.Id
                                      select u).FirstOrDefaultAsync();

            if (id != user.Id || selectedUser == null)
            {
                return BadRequest();
            }

            selectedUser.Id = user.Id;
            selectedUser.Email = user.Email;
            selectedUser.FirstName = user.FirstName;
            selectedUser.LastName = user.LastName;
            selectedUser.Rol = user.Rol;

            try
            {
                _context.Entry(selectedUser).State = EntityState.Modified;

            }
            catch (Exception)
            {
                return NoContent();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        // POST: api/Users
        [HttpPost("signin"), AllowAnonymous]
        public async Task<ActionResult<User>> Signin(RegistrationRequest registration)
        {

            var userExist = await _context.Users.Where(user => user.Email.Equals(registration.Email)).CountAsync();

            if (userExist >= 1)
            {
                return BadRequest("User already exist");
            }

            CreatePasswordHash(registration.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User user = new User()
            {
                Id = 0,
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                PasswordHash = passwordHash,
                Email = registration.Email,
                Rol = "user",
                PasswordSalt = passwordSalt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult<UserDto>> Login(LoginRequest login)
        {
            if (login == null)
            {
                return BadRequest();
            }

            User user = await _context.Users.Where(user => user.Email.Equals(login.Email)).FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            if (!VerifyPasswordHash(login.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong credentials.");
            }

            string token = CreateToken(user);

            UserDto userDto = new UserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Rol = user.Rol,
                Token = token
            };

            return Ok(userDto);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Rol)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }

        // DELETE: api/Users/5
        [HttpDelete("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
