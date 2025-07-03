//BookManagementController.cs
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;// <-- Add this

namespace LibraryManagementSystem.Controllers
{
    public class BookManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookManagementController(ApplicationDbContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        public IActionResult IndexBook()
        {
            var books = _context.Books
                .Include(b => b.Category)
                .Select(b => new BookViewModel
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Description = b.Description,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    Publisher = b.Publisher,
                    PublishedDate = b.PublishedDate,
                    CategoryName = b.Category.CategoryName,
                    CoverImage = b.CoverImage,
                    TotalCopies = b.TotalCopies,
                    BorrowRecords = b.BorrowRecords
                }).ToList();

            return View(books);
        }
        public IActionResult CreateBook()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateBook(BookCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string fileName = null;

                if (model.CoverImage != null && model.CoverImage.Length > 0)
                {
                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.CoverImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.CoverImage.CopyToAsync(stream);
                    }
                }

                var book = new Book
                {
                    Title = model.Title,
                    Author = model.Author,
                    ISBN = model.ISBN,
                    Publisher = model.Publisher,
                    PublishedDate = model.PublishedDate,
                    CategoryId = model.CategoryId,
                    Description = model.Description,
                    CoverImage = fileName != null ? "/images/" + fileName : null,
                    TotalCopies = model.TotalCopies,
                    BorrowRecords = model.BorrowRecords
                };

                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction("IndexBook", "BookManagement");
            }

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View(model);
        }


        public IActionResult EditBook(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }
            // Fetch categories and assign to ViewBag
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", book.CategoryId);
            return View(book);
        }
        [HttpPost]
        public IActionResult EditBook(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Update(book);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            // Repopulate categories if model state is invalid
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", book.CategoryId);
            return View(book);
        }
        public IActionResult DeleteBook(int id)
        {
            var book = _context.Books
     .Include(b => b.Category)
     .FirstOrDefault(b => b.BookId == id);

            return View(book);
        }
        [HttpPost, ActionName("DeleteBook")]
        public IActionResult DeleteConfirmed(int BookId)
        {
            var book = _context.Books.Find(BookId);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
                return RedirectToAction("IndexBook", "BookManagement");
            }
            return NotFound();
        }

    }
}