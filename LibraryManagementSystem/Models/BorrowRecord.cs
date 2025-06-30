using System.Runtime.CompilerServices;

namespace LibraryManagementSystem.Models
{
    public class BorrowRecord
    {
        public int BorrwoId { get; set; }
        public int BookId { get; set; }
        public Guid UserId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int FineAmount { get; set; } // Fine amount in cents or any other unit
    }
}
