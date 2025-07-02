using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class BookManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookManagementController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult CreateBook()
        {
            return View();
        }
    }
}
