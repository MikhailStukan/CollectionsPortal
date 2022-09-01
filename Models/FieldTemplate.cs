namespace CollectionsPortal.Models
{
    public enum Type
    {
        integer,
        text,
        boolean,
        textarea,
        dateTime

    }

    public class FieldTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Collection Collection { get; set; }

        public Type DataType { get; set; }
    }
}
