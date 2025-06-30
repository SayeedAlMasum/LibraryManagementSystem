using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult CreateCategory()
        {
            return View();
        }
    }
}
