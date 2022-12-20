namespace ReportFactoryApi.Models
{
    public class Device
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        /// <summary>
        /// PasswordHash
        /// </summary>
        public byte[]? Credential1 { get; set; }
        /// <summary>
        /// PasswordSalt
        /// </summary>
        public byte[]? Credential2 { get; set; }
    }
}
