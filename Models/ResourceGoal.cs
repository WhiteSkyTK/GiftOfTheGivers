namespace Gift_Of_The_Givers_Web_App.Models
{
    public class ResourceGoal
    {
        public int ResourceGoalID { get; set; }
        public int DisasterIncidentID { get; set; }
        public int ResourceID { get; set; }
        public int GoalQuantity { get; set; }
        public int CurrentQuantity { get; set; }

        public virtual DisasterIncident DisasterIncident { get; set; }
        public virtual Resource Resource { get; set; }
    }
}
