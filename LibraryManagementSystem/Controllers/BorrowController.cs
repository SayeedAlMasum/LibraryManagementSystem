//BorrowController.cs
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "User")]
    public class BorrowController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BorrowController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Confirm Borrow Page
        public async Task<IActionResult> BorrowBook(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null || book.TotalCopies <= 0)
            {
                TempData["ErrorMessage"] = "Book is not available!";
                return RedirectToAction("IndexBook", "BookManagement");
            }

            return View(book);
        }

        // POST: Borrow the Book
        [HttpPost]
        public async Task<IActionResult> ConfirmBorrow(int bookId)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == bookId);
            if (book == null || book.TotalCopies <= 0)
            {
                return Json(new { success = false, message = "Book not available" });
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found. Please log in again." });
            }

            var borrowRecord = new BorrowRecord
            {
                BookId = bookId,
                UserId = user.Id,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7)
            };

            book.TotalCopies -= 1;
            book.BorrowRecords += 1;

            _context.BorrowRecords.Add(borrowRecord);
            _context.Books.Update(book);

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Book borrowed successfully" });
        }

        // GET: List of Borrowed Books
        public async Task<IActionResult> BorrowBookList()
        {
            var user = await _userManager.GetUserAsync(User);

            var borrowRecords = await _context.BorrowRecords
                .Include(br => br.Book)
                .Where(br => br.UserId == user.Id && br.ReturnDate == null) // ✅ Only show active borrows
                .ToListAsync();

            return View(borrowRecords);
        }

        // Show Full Borrow History
        // GET: Borrow History Page
        public async Task<IActionResult> BorrowHistory()
        {
            var user = await _userManager.GetUserAsync(User);

            var borrowRecords = await _context.BorrowRecords
                .Include(br => br.Book)
                .Where(br => br.UserId == user.Id)
                .ToListAsync();

            return View(borrowRecords);
        }

        [HttpPost]
        public async Task<IActionResult> ReturnBook(int borrowId)
        {
            var borrowRecord = await _context.BorrowRecords
                .Include(br => br.Book)
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId);

            if (borrowRecord == null || borrowRecord.ReturnDate != null)
            {
                return Json(new { success = false, message = "Borrow record not found or already returned." });
            }

            borrowRecord.ReturnDate = DateTime.Now;

            // Fine Calculation: 10৳ per day if late
            if (borrowRecord.ReturnDate > borrowRecord.DueDate)
            {
                var daysLate = (borrowRecord.ReturnDate.Value - borrowRecord.DueDate).Days;
                borrowRecord.FineAmount = daysLate * 10;
            }

            borrowRecord.Book.TotalCopies += 1;

            _context.BorrowRecords.Update(borrowRecord);
            _context.Books.Update(borrowRecord.Book);

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Book returned successfully." });
        }


    }
}
