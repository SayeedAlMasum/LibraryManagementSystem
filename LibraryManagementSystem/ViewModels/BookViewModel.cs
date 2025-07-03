//BookViewModel.cs
namespace LibraryManagementSystem.ViewModels
{
    public class BookViewModel
    {
        public int BookId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Publisher { get; set; }
        public DateTime PublishedDate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CoverImage { get; set; } // ✅ Only the image path
        public int TotalCopies { get; set; }
        public int BorrowRecords { get; set; }
    }
}
