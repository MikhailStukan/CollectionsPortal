using CollectionsPortal.Models;

namespace CollectionsPortal.ViewModels
{
    public class CreateItemViewModel
    {
        public int collectionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public string Tags { get; set; }

        public List<Field> Fields { get; set; } = new();
    }
}
