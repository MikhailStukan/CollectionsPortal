namespace CollectionsPortal.Models
{
    public class TagsToItems
    {
        public int Id { get; set; }

        public Item Item { get; set; }
        public Tag Tag { get; set; }
    }
}
