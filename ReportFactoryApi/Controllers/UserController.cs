using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReportFactoryApi.Data;
using ReportFactoryApi.Interfaces;
using ReportFactoryApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace ReportFactoryApi.Controllers
{
    public class UserController : ReportController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public UserController(DataContext context, ITokenService tokenService)
        {
            _context = context;
           _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<DtoUser>> Login([FromBody] DtoAccount dtoAccount)
        {
            var users = await _context.Users!.ToListAsync();

            var user = await _context.Users!.SingleOrDefaultAsync(x => x.Username == dtoAccount.Username);
            if (user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.Credential2!);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dtoAccount.Password!));

             for (int i = 0; i < computedHash.Length; i++)
             {
                if (computedHash[i] != user.Credential1![i]) return Unauthorized("Invalid password");
             }

            return new DtoUser
            {
                Username = user.Username,
                Token = _tokenService.CreateToken(user)
            };
        }

        [AllowAnonymous]
        [HttpPost("init")]
        public async Task<ActionResult> Initialize([FromBody] DtoAccount dtoAccount)
        {
            if (await _context.Users!.AnyAsync()) return BadRequest("Initialization superfluous!");

            using var hmac = new HMACSHA512();

            var user = new User
            {
                Username = dtoAccount.Username,
                Credential1 = hmac.ComputeHash(Encoding.UTF8.GetBytes(dtoAccount.Password!)),
                Credential2 = hmac.Key
            };

            await _context.Users!.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok("User initialized.");
        }
    }
}
