namespace CollectionsPortal.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public User User { get; set; }

        public Item Item { get; set; }

        public DateTime CreatedDate { get; set; }
        public string Content { get; set; }
    }
}
