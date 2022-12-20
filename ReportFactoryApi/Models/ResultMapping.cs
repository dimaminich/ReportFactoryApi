using System.ComponentModel.DataAnnotations;

namespace ReportFactoryApi.Models
{
    public class ResultMapping
    {
        [Required]
        public string? Source { get; set; }
        [Required]
        public string? Target { get; set; }
    }
}
