namespace CollectionsPortal.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Collection> Collection { get; set; }
    }
}
