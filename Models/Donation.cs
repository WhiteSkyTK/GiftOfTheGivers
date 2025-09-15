using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_Of_The_Givers_Web_App.Models
{
    public class Donation
    {
        [Key]
        public int DonationID { get; set; }

        [ForeignKey("User")]
        public int DonorUserID { get; set; }

        [ForeignKey("ReliefProject")]
        public int? ProjectID { get; set; } // Nullable

        public bool IsMonetary { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; } // Nullable

        [ForeignKey("Resource")]
        public int? ResourceID { get; set; } // Nullable

        public int? Quantity { get; set; } // Nullable

        public DateTime DonationDate { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual ReliefProject? ReliefProject { get; set; }
        public virtual Resource? Resource { get; set; }
    }
}