using Jwe_AspCore.Models;

namespace Jwe_AspCore.Services
{
    public interface IJwtService
    {
        string Generate(User user);
    }
}
