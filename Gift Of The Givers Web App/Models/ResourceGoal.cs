using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Gift_Of_The_Givers_Web_App.Models
{
    public class ResourceGoal
    {
        [Key]
        public int ResourceGoalID { get; set; }

        [ForeignKey("DisasterIncident")]
        public int DisasterIncidentID { get; set; }

        [ForeignKey("Resource")]
        public int ResourceID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Goal quantity must be at least 1.")]
        public int GoalQuantity { get; set; }

        public int CurrentQuantity { get; set; } // This is set automatically, not by the user

        [ValidateNever] 
        public virtual DisasterIncident DisasterIncident { get; set; }
        [ValidateNever]
        public virtual Resource Resource { get; set; }
    }
}