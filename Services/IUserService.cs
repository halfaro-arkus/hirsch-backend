using Hirsch.Models;

namespace Hirsch.Services
{
    public interface IUserService
    {
        User Login(string username, string password);
        Task RegisterUser(UserRequest userRequest);
        bool IsUsernameTaken(string username);
        List<UserResponse> GetAllUsers(string username);
        void AddFavorite(string username, int userId);
        void RemoveFavorite(string username, int favoriteId);
        List<Favorite> GetFavorites(string username);
        UserDetail GetUserById(int Id, string username);
        User GetUserEntityById(int Id);
    }
}
