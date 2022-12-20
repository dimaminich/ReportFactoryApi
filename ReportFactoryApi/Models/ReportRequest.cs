using System.ComponentModel.DataAnnotations;

namespace ReportFactoryApi.Models
{
    public class ReportRequest
    {
        [Required]
        public DtoDevice? device { get; set; }
        [Required]
        public string? RDLCTemplate { get; set; }
        [Required]
        public string? Printer { get; set; }
        [Required]
        public string? ReportFilename { get; set; }
        [Required]
        public List<QueryItem>? QueryItems { get; set; }
        public List<UserValueItem>? UserValueItems { get; set; }
    }
}
