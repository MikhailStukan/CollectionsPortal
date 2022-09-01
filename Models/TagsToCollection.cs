namespace CollectionsPortal.Models
{
    public class TagsToCollection
    {
        public int Id { get; set; }

        public Collection Collection { get; set; }
        public Tag Tag { get; set; }
    }
}
