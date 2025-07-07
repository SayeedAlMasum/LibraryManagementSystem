// BorrowController.cs
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class BorrowController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BorrowController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "User")]
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
                DueDate = DateTime.Now.AddDays(7),
                Status = "Pending"
            };

            _context.BorrowRecords.Add(borrowRecord);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Borrow request submitted for admin approval." });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PendingRequests()
        {
            var pendingRequests = await _context.BorrowRecords
                .Include(br => br.Book)
                .Include(br => br.User)
                .Where(br => br.Status == "Pending")
                .ToListAsync();

            return View(pendingRequests);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ApproveRequest(int borrowId)
        {
            var borrowRecord = await _context.BorrowRecords
                .Include(br => br.Book)
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId);

            if (borrowRecord == null || borrowRecord.Status != "Pending")
            {
                return Json(new { success = false, message = "Borrow request not found or already processed." });
            }

            if (borrowRecord.Book.TotalCopies <= 0)
            {
                return Json(new { success = false, message = "Book not available." });
            }

            borrowRecord.Status = "Approved";
            borrowRecord.Book.TotalCopies -= 1;
            borrowRecord.Book.BorrowRecords += 1;

            _context.BorrowRecords.Update(borrowRecord);
            _context.Books.Update(borrowRecord.Book);

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Borrow request approved." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> RejectRequest(int borrowId)
        {
            var borrowRecord = await _context.BorrowRecords.FirstOrDefaultAsync(br => br.BorrowId == borrowId);

            if (borrowRecord == null || borrowRecord.Status != "Pending")
            {
                return Json(new { success = false, message = "Borrow request not found or already processed." });
            }

            borrowRecord.Status = "Rejected";
            _context.BorrowRecords.Update(borrowRecord);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Borrow request rejected." });
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> BorrowBookList()
        {
            var user = await _userManager.GetUserAsync(User);
            var borrowRecords = await _context.BorrowRecords
                .Include(br => br.Book)
                .Where(br => br.UserId == user.Id && br.Status == "Approved" && br.ReturnDate == null)
                .ToListAsync();

            return View(borrowRecords);
        }

        [Authorize]
        public async Task<IActionResult> BorrowHistory()
        {
            var user = await _userManager.GetUserAsync(User);

            IQueryable<BorrowRecord> query = _context.BorrowRecords.Include(br => br.Book).Include(br => br.User);

            if (User.IsInRole("Admin"))
            {
                var allRecords = await query.ToListAsync();
                return View(allRecords);
            }
            else
            {
                var userRecords = await query.Where(br => br.UserId == user.Id).ToListAsync();
                return View(userRecords);
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> ReturnBook(int borrowId)
        {
            var borrowRecord = await _context.BorrowRecords.Include(br => br.Book).FirstOrDefaultAsync(br => br.BorrowId == borrowId);

            if (borrowRecord == null || borrowRecord.ReturnDate != null)
            {
                return Json(new { success = false, message = "Borrow record not found or already returned." });
            }

            borrowRecord.ReturnDate = DateTime.Now;

            if (borrowRecord.ReturnDate > borrowRecord.DueDate)
            {
                var daysLate = (borrowRecord.ReturnDate.Value - borrowRecord.DueDate).Days;
                borrowRecord.FineAmount = daysLate * 50;
            }

            borrowRecord.Book.TotalCopies += 1;

            _context.BorrowRecords.Update(borrowRecord);
            _context.Books.Update(borrowRecord.Book);

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Book returned successfully." });
        }
    }
}
