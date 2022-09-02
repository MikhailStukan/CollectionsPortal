using CollectionsPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace CollectionsPortal.ViewModels
{
    public class CreateCollectionViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [StringLength(15, MinimumLength = 3)]
        public string Name { get; set; }


        public IFormFile ImageFile { get; set; }


        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(100, MinimumLength = 20)]
        public string Description { get; set; }

        public Topic Topic { get; set; }

        public string Tags { get; set; }

        public List<FieldTemplate> Fields { get; set; } = new();
    }
}
