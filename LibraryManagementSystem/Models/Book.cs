using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Publisher { get; set; }
        public DateTime PublishedDate { get; set; }
        public int CategoryId { get; set; }
        public string CoverImage { get; set; }
        public int TotalCopies { get; set; }
        public int BorrowRecords { get; set; }
    }
}
