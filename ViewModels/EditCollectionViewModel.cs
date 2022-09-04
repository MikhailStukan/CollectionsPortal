using System.ComponentModel.DataAnnotations;

namespace CollectionsPortal.ViewModels
{
    public class EditCollectionViewModel
    {
        public int collectionId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(15, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(100, MinimumLength = 10)]
        public string Description { get; set; }

        public IFormFile ImageFile { get; set; }

    }
}
