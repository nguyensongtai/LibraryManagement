using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
  [Authorize]
  public class HomeController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}
