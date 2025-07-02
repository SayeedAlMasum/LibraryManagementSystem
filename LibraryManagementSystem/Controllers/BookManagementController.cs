//BookManagementController.cs
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
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
        public IActionResult IndexBook()
        {
            var books = _context.Books
                .Include(b => b.Category) // This will join Category table data
                .ToList();
            return View(books);
        }
        public IActionResult CreateBook()
        {
            // Fetch categories and assign to ViewBag
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        public IActionResult CreateBook(Book book)
        {
            
                _context.Books.Add(book);
                _context.SaveChanges();
                return RedirectToAction("IndexBook", "BookManagement");

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