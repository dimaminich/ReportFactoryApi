using Microsoft.EntityFrameworkCore;
using ReportFactoryApi.Data;
using ReportFactoryApi.Interfaces;
using ReportFactoryApi.Models;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace ReportFactoryApi.Services
{
    public class UserInitService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public UserInitService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            int status = default;
            string username = "...";
            string password = "...";

            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                DataContext context = scope.ServiceProvider.GetRequiredService<DataContext>();

                if (context.Users!.Any(x => x.Username == username)) return Task.FromResult(status);

                
                using var hmac = new HMACSHA512();

                var user = new User
                {
                    Username = username,
                    Credential1 = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                    Credential2 = hmac.Key
                };

                context.Users!.Add(user);
                status = context.SaveChanges();
            }

            return Task.FromResult(status);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult("Service stopped");
        }
    }
}
