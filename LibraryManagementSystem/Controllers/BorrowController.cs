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
                .Where(br => br.UserId == user.Id)
                .ToListAsync();

            return View(borrowRecords);
        }

    }
}
