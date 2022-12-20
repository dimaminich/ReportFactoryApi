using System.ComponentModel.DataAnnotations;

namespace ReportFactoryApi.Models
{
    public class DtoAccount
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
