using Microsoft.EntityFrameworkCore;
using ReportFactoryApi.Data;
using ReportFactoryApi.Models;
using ReportFactoryApi.Utilities;
using System.Security.Cryptography;
using System.Text;

namespace ReportFactoryApi.Services
{
    public class VerificationService
    {

        private readonly ILogger<VerificationService> _logger;

        public VerificationService(ILogger<VerificationService> logger)
        {
            _logger = logger;
        }

        public async Task<(int status, string message)> Verify(DtoDevice dtoDevice, DataContext context)
        {
            var device = await context.Devices!.FirstOrDefaultAsync(device => device.Name == dtoDevice.Name);
            if (device == null) return (StatusCodes.Status401Unauthorized, "Device not exist!");

            using var hmac = new HMACSHA512(device.Credential2!);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dtoDevice.ApiKey!));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != device.Credential1![i]) return (StatusCodes.Status401Unauthorized, "Wrong device ApiKey!");
            }

            return (StatusCodes.Status200OK, "Device verified.");
        }

    }
}
