//BorrowController.cs
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize] // Only logged-in users can borrow books
    public class BorrowController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BorrowController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Borrow a book
        public async Task<IActionResult> BorrowBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null || book.TotalCopies <= 0)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var borrowRecord = new BorrowRecord
            {
                BookId = id,
                UserId = userId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7), // 7 days borrow duration
                FineAmount = 0
            };

            _context.BorrowRecords.Add(borrowRecord);

            book.TotalCopies -= 1;
            _context.Books.Update(book);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Book borrowed successfully!";
            return RedirectToAction("IndexBook", "BookManagement");
        }

        // Return a book
        public async Task<IActionResult> ReturnBook(int id)
        {
            var record = await _context.BorrowRecords.Include(b => b.Book).FirstOrDefaultAsync(b => b.BorrowId == id);
            if (record == null || record.ReturnDate != null)
            {
                return NotFound();
            }

            record.ReturnDate = DateTime.Now;

            // Fine calculation if overdue
            if (record.ReturnDate > record.DueDate)
            {
                var daysOverdue = (record.ReturnDate.Value - record.DueDate).Days;
                record.FineAmount = daysOverdue * 10; // Example: 10 units fine per day
            }

            record.Book.TotalCopies += 1;

            _context.BorrowRecords.Update(record);
            _context.Books.Update(record.Book);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Book returned successfully!";
            return RedirectToAction("MyBorrowedBooks");
        }

        // View Borrowed Books for current user
        public async Task<IActionResult> MyBorrowedBooks()
        {
            var userId = _userManager.GetUserId(User);
            var records = await _context.BorrowRecords
                .Include(b => b.Book)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return View(records);
        }
    }
}
