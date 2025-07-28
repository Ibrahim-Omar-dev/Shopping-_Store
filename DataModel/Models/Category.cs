using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name="Category Name")]
        [MaxLength(30,ErrorMessage ="The Length must to less than 30 characters")]
        public string Name { get; set; }
        [Display(Name="Display Order")]
        [Range(1,1000)]
        public int DisplayOrder { get; set; }
    }
}
