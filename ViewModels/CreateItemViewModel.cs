using CollectionsPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace CollectionsPortal.ViewModels
{
    public class CreateItemViewModel
    {
        public int collectionId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(15, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 15)]
        public string Description { get; set; }

        public string Tags { get; set; }

        public List<Field> Fields { get; set; } = new();
    }
}
