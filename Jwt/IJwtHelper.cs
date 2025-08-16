namespace Hirsch.Jwt
{
    public interface IJwtHelper
    {
        string GenerateToken(string username);
    }
}
