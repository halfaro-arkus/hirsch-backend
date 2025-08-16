using System.Collections.Concurrent;
using System.Text.Json;
using Hirsch.Models;

namespace Hirsch.Services
{
    public class UserService : IUserService
    {
        private readonly ConcurrentDictionary<string, User> _users = new();
        private readonly HttpClient _httpClient = new();

        public UserService() {
            RegisterUser(new UserRequest { Email = "halfaro@arkusnexus.com", Password = "halfaro" });
        }
        

        public User Login(string username, string password)
        {
            username = username.ToLower();
            User user = _users.GetValueOrDefault(username);
            if (user != null && user.Password == password)
            {
                return user;
            }
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        public async Task RegisterUser(UserRequest userRequest)
        {
            User user = new User
            {
                Email = userRequest.Email,
                Password = userRequest.Password              
            };
            user.Email = user.Email.ToLower();
            if (IsUsernameTaken(user.Email))
            {
                throw new InvalidOperationException("El nombre de usuario ya está en uso.");
            }
            user.Id = _users.Count + 1;
            user.Created = DateTime.UtcNow;
            assignData(user);           

            _users.TryAdd(user.Email, user);
        }

        public bool IsUsernameTaken(string username)
        {
            return _users.ContainsKey(username);
        }

        public List<UserResponse> GetAllUsers(string username)
        {
            List<User> users = _users.Values.ToList();
            List<UserResponse> result = new List<UserResponse>();            
            User userSession = _users.GetValueOrDefault(username);            
            users.ForEach(user =>
            {
                result.Add(new UserResponse
                {
                    Id = user.Id,
                    FullName = user.Data?.Name ?? "-",
                    isFavorite = userSession.Favorites.Find(ul => ul.Id == user.Id) != null,
                    CreatedDate = user.Created,
                    LocationOrigin = user.Data?.Location?.Name ?? "Desconocido",
                    HirschLabel = false
                });
            });

            return result;
    }

        public void AddFavorite(string username, int userId)
        {
            if (_users.TryGetValue(username, out var user))
            {
                bool exists = user.Favorites.Any(f => f.Id == userId);
                if (!exists)
                {
                    user.Favorites.Add(new Favorite { Id = userId });
                }
                else {
                    RemoveFavorite(username, userId);
                }

            }
        }

        public void RemoveFavorite(string username, int favoriteId)
        {
            if (_users.TryGetValue(username, out var user))
            {
                var favorite = user.Favorites.FirstOrDefault(f => f.Id == favoriteId);
                if (favorite != null)
                {
                    user.Favorites.Remove(favorite);
                }
            }
        }

        public List<Favorite> GetFavorites(string username)
        {
            return _users.TryGetValue(username, out var user) ? user.Favorites : new List<Favorite>();
        }

        private async Task assignData(User user) {            
            var apiUrl = $"https://rickandmortyapi.com/api/character/{user.Id}";
            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            user.Data = JsonSerializer.Deserialize<UserData>(json, options);
        }

        public UserDetail GetUserById(int Id, string username)
        {
            List<User> users = _users.Values.ToList();
            User user = users.Find(u => u.Id == Id);            
            User userSession = _users.GetValueOrDefault(username);            
            if (user != null)
            {
                return new UserDetail
                {
                    Id = user.Id,
                    FullName = user.Data.Name,
                    isFavorite = userSession.Favorites.Find(ul => ul.Id == Id) != null,
                    CreatedDate = user.Created,
                    LocationOrigin = user.Data.Location?.Name ?? "Desconocido",
                    HirschLabel = false,
                    Species = user.Data.Species,
                    EpisodesCount = user.Data.Episode?.Count ?? 0,
                    ImageUrl = user.Data.Image,
                    Status = user.Data.Status
                };
            }
            else
            {
                throw new KeyNotFoundException("Usuario no encontrado.");

            }
        }

        public User GetUserEntityById(int Id)
        {
            return _users.Values.ToList().Find(u => u.Id == Id);            
        }
    }
}
