using Microsoft.AspNetCore.Identity;

namespace PantryManagementSystem.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
