namespace CollectionsPortal.Models
{
    public class Field
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public Item Item { get; set; }

        public FieldTemplate FieldTemplates { get; set; }
    }
}
