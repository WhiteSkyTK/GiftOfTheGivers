using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_Of_The_Givers_Web_App.Models
{
    public class DisasterIncident
    {
        [Key]
        public int IncidentID { get; set; }

        [ForeignKey("User")]
        public int ReportedByUserID { get; set; }

        [Required, StringLength(255)]
        public string Location { get; set; } = string.Empty;

        [Column(TypeName = "decimal(9, 6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(9, 6)")]
        public decimal Longitude { get; set; }

        public string? Description { get; set; }

        public DateTime IncidentDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        // Navigation property
        public virtual User? User { get; set; }
    }
}