using System.ComponentModel.DataAnnotations;

namespace Gift_Of_The_Givers_Web_App.ViewModels
{
    public class EditIncidentViewModel
    {
        public int IncidentID { get; set; }
        public string ReportedByUserID { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; }

        [Required, StringLength(255)]
        public string Location { get; set; }

        public string? Description { get; set; }

        [Url]
        public string? ImageUrl { get; set; }

        [DataType(DataType.Currency)]
        public decimal FundingGoal { get; set; }

        [DataType(DataType.Currency)]
        public decimal CurrentFunds { get; set; }

        public string Status { get; set; }
    }
}