namespace CollectionsPortal.Models
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public int? CollectionId { get; set; }
        public Collection collection;

        public DateTime CreatedAt { get; set; }

        public IList<Comment> Comments { get; } = new List<Comment>();
        public IList<Like> Likes { get; } = new List<Like>();
    }
}
