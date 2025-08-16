using Hirsch.Models;

namespace Hirsch.Models
{
    public class UserDetail : UserResponse
    {
        public string ImageUrl { get; set; } = "";
        public string Species { get; set; } = "";
        public string Status { get; set; } = "";
        public int EpisodesCount { get; set; } = 0;        
    }
}
