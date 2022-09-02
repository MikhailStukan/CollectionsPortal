

namespace CollectionsPortal.Models
{
    public class Collection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string? imageUrl { get; set; }
        public Topic Topic { get; set; }

        public User User { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<Item> Items { get; set; } = new();
        public List<FieldTemplate> FieldTemplates { get; set; } = new();

    }
}
