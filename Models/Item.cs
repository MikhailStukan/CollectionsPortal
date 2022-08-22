namespace CollectionsPortal.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; }
        public int CollectionId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
