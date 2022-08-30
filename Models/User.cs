using Microsoft.AspNetCore.Identity;

namespace CollectionsPortal.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Description { get; set; }
        public bool isAdmin { get; set; }

        public List<Collection> collections { get; set; } = new();
        public List<Like> likes { get; set; } = new();
        public List<Comment> comments { get; set; } = new(); 

    }
}
