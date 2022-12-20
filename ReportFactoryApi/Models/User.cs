namespace ReportFactoryApi.Models
{
    public class User
    {
        public int? Id { get; set; }
        public string? Username { get; set; }
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
