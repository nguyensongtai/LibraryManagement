using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagement.Controllers
{
  [Authorize]
  public class ReadersController : Controller
  {
    private readonly AppDbContext _context;

    public ReadersController(AppDbContext context)
    {
      _context = context;
    }

    // GET: Readers
    public async Task<IActionResult> Index()
    {
      return View(await _context.Readers.ToListAsync());
    }

    // GET: Readers/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null) return NotFound();

      var reader = await _context.Readers
        .Include(r => r.BookLoans)
        .FirstOrDefaultAsync(m => m.ReaderCardId == id);

      if (reader == null) return NotFound();

      return View(reader);
    }

    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ReaderCardId,Name,Address,PhoneNumber")] Reader reader)
    {
      if (ModelState.IsValid)
      {
        _context.Add(reader);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(reader);
    }

    // GET: Readers/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null) return NotFound();

      var reader = await _context.Readers.FindAsync(id);
      if (reader == null) return NotFound();

      return View(reader);
    }

    // POST: Readers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ReaderCardId,Name,Address,PhoneNumber")] Reader reader)
    {
      if (id != reader.ReaderCardId) return NotFound();

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(reader);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!ReaderExists(reader.ReaderCardId)) return NotFound();
          else throw;
        }
        return RedirectToAction(nameof(Index));
      }
      return View(reader);
    }

    // GET: Readers/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null) return NotFound();

      var reader = await _context.Readers
        .Include(r => r.BookLoans) // Include related BookLoans
        .FirstOrDefaultAsync(m => m.ReaderCardId == id);

      if (reader == null) return NotFound();

      return View(reader);
    }

    // POST: Readers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var reader = await _context.Readers
        .Include(r => r.BookLoans)
        .FirstOrDefaultAsync(r => r.ReaderCardId == id);

      if (reader != null)
      {
        // Remove associated BookLoans
        if (reader.BookLoans != null)
        {
          _context.BookLoans.RemoveRange(reader.BookLoans);
        }

        _context.Readers.Remove(reader);
        await _context.SaveChangesAsync();
      }

      return RedirectToAction(nameof(Index));
    }

    private bool ReaderExists(int id)
    {
      return _context.Readers.Any(e => e.ReaderCardId == id);
    }
  }
}
