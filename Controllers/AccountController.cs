using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagement.Controllers
{
  public class AccountController : Controller
  {
    private static List<(string Username, string Password)> _users = new List<(string, string)>
    {
      ("admin", "admin") // Default user
    };

    // GET: Account/Login
    [AllowAnonymous]
    public IActionResult Login()
    {
      if (User.Identity.IsAuthenticated)
      {
        return RedirectToAction("Index", "Home");
      }
      return View();
    }

    // POST: Account/Login
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string username, string password)
    {
      var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);

      if (user != default)
      {
        var claims = new List<Claim>
        {
          new Claim(ClaimTypes.Name, username)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
          CookieAuthenticationDefaults.AuthenticationScheme,
          new ClaimsPrincipal(claimsIdentity));

        TempData["SuccessMessage"] = "Login successful! Welcome back, " + username;
        return RedirectToAction("Index", "Home");
      }

      ViewBag.Error = "Invalid username or password.";
      return View();
    }

    // GET: Account/Register
    [AllowAnonymous]
    public IActionResult Register()
    {
      if (User.Identity.IsAuthenticated)
      {
        return RedirectToAction("Index", "Home");
      }
      return View();
    }

    // POST: Account/Register
    [HttpPost]
    [AllowAnonymous]
    public IActionResult Register(string username, string password)
    {
      if (_users.Any(u => u.Username == username))
      {
        ViewBag.Error = "Username already exists.";
        return View();
      }

      _users.Add((username, password));
      TempData["SuccessMessage"] = "Registration successful! You can now log in.";
      return RedirectToAction("Index", "Home");
    }

    // GET: Account/Logout
    public async Task<IActionResult> Logout()
    {
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

      // Delete Cache when logout
      Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
      Response.Headers["Pragma"] = "no-cache";
      Response.Headers["Expires"] = "0";

      TempData["SuccessMessage"] = "You have been logged out.";
      return RedirectToAction("Login");
    }
  }
}
