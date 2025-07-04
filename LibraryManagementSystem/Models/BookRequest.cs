//BookRequest.cs
namespace LibraryManagementSystem.Models
{
    public class BookRequest
    {
        public int RequestId { get; set; }
        public int BookId { get; set; }
        public string UserId { get; set; }
        public DateTime RequestDate { get; set; }
        public bool IsApproved { get; set; } // e.g., "Pending", "Approved", "Rejected"
        public DateTime ApprovalDate { get; set; } 
        public bool Status { get; set; } // e.g., "Active", "Cancelled"
    }
}
