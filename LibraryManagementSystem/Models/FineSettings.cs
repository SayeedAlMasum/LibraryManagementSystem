namespace LibraryManagementSystem.Models
{
    public class FineSettings
    {
        public string SettingsId { get; set; } = Guid.NewGuid().ToString();
        public int FinePerDay { get; set; } // Fine amount per day in cents or any other unit
        public int MaxFine { get; set; } // Maximum fine amount in cents or any other unit
    }
}
