using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json;

namespace ReportFactoryApi.Models
{
    public class QueryParameter
    {
        [Required]
        public string? ParameterPlaceholder { get; set; }
        [Required]
        public JsonElement ParameterValue { get; set; }
        [Required]
        public SqlDbType ParameterType { get; set; }
    }
}
