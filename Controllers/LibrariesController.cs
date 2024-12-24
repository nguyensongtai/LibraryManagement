using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagement.Controllers
{
  [Authorize]
  public class LibrariesController : Controller
  {
    private readonly AppDbContext _context;

    public LibrariesController(AppDbContext context)
    {
      _context = context;
    }

    // GET: Libraries
    public async Task<IActionResult> Index()
    {
      return View(await _context.Libraries.ToListAsync());
    }

    // GET: Libraries/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null) return NotFound();

      var library = await _context.Libraries.FirstOrDefaultAsync(m => m.LibraryId == id);
      if (library == null) return NotFound();

      return View(library);
    }

    // GET: Libraries/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Libraries/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("LibraryId,LibraryName,Address")] Library library)
    {
      if (ModelState.IsValid)
      {
        _context.Add(library);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(library);
    }

    // GET: Libraries/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null) return NotFound();

      var library = await _context.Libraries.FindAsync(id);
      if (library == null) return NotFound();

      return View(library);
    }

    // POST: Libraries/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("LibraryId,LibraryName,Address")] Library library)
    {
      if (id != library.LibraryId) return NotFound();

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(library);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!LibraryExists(library.LibraryId)) return NotFound();
          else throw;
        }
        return RedirectToAction(nameof(Index));
      }
      return View(library);
    }

    // GET: Libraries/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null) return NotFound();

      var library = await _context.Libraries.FirstOrDefaultAsync(m => m.LibraryId == id);
      if (library == null) return NotFound();

      return View(library);
    }

    // POST: Libraries/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var library = await _context.Libraries.FindAsync(id);
      if (library != null)
      {
        _context.Libraries.Remove(library);
        await _context.SaveChangesAsync();
      }
      return RedirectToAction(nameof(Index));
    }

    private bool LibraryExists(int id)
    {
      return _context.Libraries.Any(e => e.LibraryId == id);
    }
  }
}
