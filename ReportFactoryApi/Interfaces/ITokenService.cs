using ReportFactoryApi.Models;

namespace ReportFactoryApi.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
