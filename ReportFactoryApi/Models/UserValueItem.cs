using System.ComponentModel.DataAnnotations;

namespace ReportFactoryApi.Models
{
    public class UserValueItem
    {
        [Required]
        public string? DataSetName { get; set; }
        [Required]
        public List<ValueItem>? ValueItems { get; set; }
    }
}