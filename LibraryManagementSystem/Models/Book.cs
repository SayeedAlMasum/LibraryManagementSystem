//Book.cs
namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Publisher { get; set; }
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string CoverImage { get; set; }
        public int TotalCopies { get; set; } = 100;
        public int BorrowRecords { get; set; }
    }
}
