namespace CollectionsPortal.Models
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public Collection Collection;

        public DateTime CreatedAt { get; set; }

        public List<Comment> Comments { get; set; } = new();
        public List<Like> Likes { get; set; } = new();

        public List<Field> Fields { get; set; } = new();
    }
}
