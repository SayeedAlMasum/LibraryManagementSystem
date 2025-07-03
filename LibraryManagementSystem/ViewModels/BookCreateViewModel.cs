//BookCreateViewModel.cs
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class BookCreateViewModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Publisher { get; set; }
        public DateTime PublishedDate { get; set; }
        public int CategoryId { get; set; }
        [Required]
        public IFormFile? CoverImage { get; set; }
        public int TotalCopies { get; set; }
        public int BorrowRecords { get; set; }
    }
}
