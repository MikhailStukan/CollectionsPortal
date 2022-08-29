using Microsoft.AspNetCore.Identity;

namespace CollectionsPortal.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Description { get; set; }
        public bool isAdmin { get; set; }

    }
}
