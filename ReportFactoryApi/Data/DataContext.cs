using Microsoft.EntityFrameworkCore;
using ReportFactoryApi.Models;

namespace ReportFactoryApi.Data
{
    public class DataContext: DbContext
    {

        public DataContext(DbContextOptions options): base(options)
        {

        }

        public DbSet<User>? Users { get; set; }
        public DbSet<Device>? Devices { get; set; }

    }
}
