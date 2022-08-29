using Microsoft.AspNetCore.Identity;

namespace CollectionsPortal.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Description { get; set; }
        public bool isAdmin { get; set; }

        public IList<Collection> collections { get; } = new List<Collection>();
        public IList<Like> likes { get; } = new List<Like>();
        public IList<Comment> comments { get; } = new List<Comment>();  

    }
}
