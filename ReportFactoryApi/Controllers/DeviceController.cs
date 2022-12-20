using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReportFactoryApi.Data;
using ReportFactoryApi.Interfaces;
using ReportFactoryApi.Models;
using ReportFactoryApi.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace ReportFactoryApi.Controllers
{
    public class DeviceController : ReportController
    {
        private readonly DataContext _context;
        private readonly VerificationService _verificationService;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(
            DataContext context,
            VerificationService verificationService,
            ILogger<DeviceController> logger)
        {
            _context = context;
            _verificationService = verificationService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevices()
        {
            var devices = await _context.Devices!.ToListAsync();
            return devices;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Device>> GetDevice(int id)
        {
            var device = await _context.Devices!.FindAsync(id);
            return device!;
        }

        [Authorize]
        [HttpPost("import")]
        public async Task<ActionResult> ImportDevices([FromBody] IEnumerable<Device> devices)
        {
            await _context.Devices!.AddRangeAsync(devices);
            await _context.SaveChangesAsync();
            return Ok("Devices imported.");

        }

        [Authorize]
        [HttpPost("register/{devicename}")]
        public async Task<ActionResult<DtoDevice>> Register([FromRoute][Required] string devicename)
        {
            if (await _context.Devices!.AnyAsync(x => x.Name == devicename.ToLower()))
                return BadRequest("Device name is taken");

            string hashstring = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            using var hmac = new HMACSHA512();

            var device = new Device()
            {
                Name = devicename,
                Credential1 = hmac.ComputeHash(Encoding.UTF8.GetBytes(hashstring)),
                Credential2 = hmac.Key
            };

            _context.Devices!.Add(device);
            await _context.SaveChangesAsync();

            return new DtoDevice
            {
                Name = devicename,
                ApiKey = hashstring
            };
        }

        [Authorize]
        [HttpPost("verify")]
        public async Task<ActionResult> VerifyDevice([FromBody] DtoDevice dtoDevice)
        {
            (int statuscode, string message) status = await _verificationService.Verify(dtoDevice, _context);
            return StatusCode(status.statuscode, status.message);
        }
    }
}
