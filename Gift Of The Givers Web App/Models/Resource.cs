using System.ComponentModel.DataAnnotations;

namespace Gift_Of_The_Givers_Web_App.Models
{
    public class Resource
    {
        [Key]
        public int ResourceID { get; set; }

        [Required, StringLength(100)]
        public string ResourceName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }
    }
}