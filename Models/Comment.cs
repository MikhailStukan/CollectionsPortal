namespace CollectionsPortal.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        public int? ItemId { get; set; }

        public DateTime CreatedDate { get; set; }
        public string Content { get; set; }
    }
}
