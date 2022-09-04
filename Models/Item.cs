namespace CollectionsPortal.Models
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? imageUrl { get; set; }

        public Collection Collection { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<Comment> Comments { get; set; } = new();
        public List<Like> Likes { get; set; } = new();

        public List<Field> Fields { get; set; } = new();
    }
}
