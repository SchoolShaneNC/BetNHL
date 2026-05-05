namespace BetNHL_Web_Api.Services
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user);
    }
}
