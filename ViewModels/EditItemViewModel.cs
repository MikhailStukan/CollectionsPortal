using CollectionsPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace CollectionsPortal.ViewModels
{
    public class EditItemViewModel
    {
        public int itemId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(15, MinimumLength = 3)]
        public string Name { get; set; }

        public IFormFile ImageFile { get; set; }

        [Required]
        public List<Field> Fields { get; set; } = new();
    }
}
