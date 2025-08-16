using System.ComponentModel.DataAnnotations;

namespace Hirsch.Models
{
    public class User
    {        
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Created { get; set; }
        public List<Favorite> Favorites { get; set; } = new List<Favorite>();
        public UserData Data { get; set; }
    }
}
