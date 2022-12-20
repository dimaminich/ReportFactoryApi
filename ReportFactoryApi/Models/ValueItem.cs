using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ReportFactoryApi.Models
{
    public class ValueItem
    {
        [Required]
        public string? Target { get; set; }
        [Required]
        public JsonElement TargetValue { get; set; }
    }
}