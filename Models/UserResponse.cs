namespace Hirsch.Models
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string LocationOrigin { get; set; }
        public bool isFavorite { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool HirschLabel { get; set; } = false;
        
        
        
        
    }
}
