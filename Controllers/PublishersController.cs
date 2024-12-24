using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagement.Controllers
{
  [Authorize]
  public class PublishersController : Controller
  {
    private readonly AppDbContext _context;

    public PublishersController(AppDbContext context)
    {
      _context = context;
    }

    public async Task<IActionResult> Index()
    {
      return View(await _context.Publishers.ToListAsync());
    }

    public async Task<IActionResult> Details(string? name)
    {
      if (name == null) return NotFound();

      var publisher = await _context.Publishers
        .Include(p => p.BookTitles) 
        .FirstOrDefaultAsync(m => m.Name == name);

      if (publisher == null) return NotFound();

      return View(publisher);
    }

    public IActionResult Create()
    {
      return View();
    }

    // POST: Publishers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Address,PhoneNumber")] Publisher publisher)
    {
      if (ModelState.IsValid)
      {
        _context.Add(publisher);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(publisher);
    }

    // GET: Publishers/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null) return NotFound();

      var publisher = await _context.Publishers.FindAsync(id);
      if (publisher == null) return NotFound();

      return View(publisher);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string name, [Bind("Id,Name,Address,PhoneNumber")] Publisher publisher)
    {
      if (name != publisher.Name) return NotFound();

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(publisher);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!PublisherExists(publisher.Name)) return NotFound();
          else throw;
        }
        return RedirectToAction(nameof(Index));
      }
      return View(publisher);
    }

    // GET: Publishers/Delete/5
    public async Task<IActionResult> Delete(string? name)
    {
      if (name == null) return NotFound();

      var publisher = await _context.Publishers
        .Include(p => p.BookTitles) // Include related BookTitles
        .FirstOrDefaultAsync(m => m.Name == name);

      if (publisher == null) return NotFound();

      return View(publisher);
    }

    // POST: Publishers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var publisher = await _context.Publishers.FindAsync(id);
      if (publisher != null)
      {
        _context.Publishers.Remove(publisher);
        await _context.SaveChangesAsync();
      }
      return RedirectToAction(nameof(Index));
    }

    private bool PublisherExists(string name)
    {
      return _context.Publishers.Any(e => e.Name == name);
    }
  }
}
