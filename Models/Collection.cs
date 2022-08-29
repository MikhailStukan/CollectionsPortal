namespace CollectionsPortal.Models
{
    public class Collection
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public int TopicId { get; set; }


        public string? UserId { get; set; }
        public User User { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int CountItems { get; set; }

        public IList<Item> Items { get; } = new List<Item>();


    }
}
