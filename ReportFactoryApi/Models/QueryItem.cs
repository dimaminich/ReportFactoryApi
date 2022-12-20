using System.ComponentModel.DataAnnotations;

namespace ReportFactoryApi.Models
{
    public class QueryItem
    {
        [Required]
        public string? DbConnection { get; set; }
        [Required]
        public string? DataSetName { get; set; }
        [Required]
        public string? QueryName { get; set; }
        public List<QueryParameter>? QueryParameters { get; set; }
        [Required]
        public List<ResultMapping>? ResultMappings { get; set; }
    }
}
