//CategoryController.cs
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult IndexCategory()
        {
            var categories = _context.Categories.ToList();
            return View("IndexCategory", categories); // Custom View
        }

        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();

                return RedirectToAction("IndexCategory", "Category");
            }

            return View(category);
        }

        public IActionResult EditCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View("EditCategory", category); // Custom view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                return RedirectToAction("IndexCategory", "Category");
            }
            return View("EditCategory", category); // Custom View
        }

        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View("DeleteCategory", category); // Custom View
        }

        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();

                // Call reseed method here
                ReseedCategoryTable();
            }
            return RedirectToAction("IndexCategory", "Category");
        }

        // ======== Reseed Method =========
        private void ReseedCategoryTable()
        {
            var maxId = _context.Categories.Any() ? _context.Categories.Max(c => c.CategoryId) : 0;
            string reseedSql = $"DBCC CHECKIDENT ('Categories', RESEED, {maxId})";
            _context.Database.ExecuteSqlRaw(reseedSql);
        }
    }
}

