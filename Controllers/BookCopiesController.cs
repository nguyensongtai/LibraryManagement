using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Controllers
{
  [Authorize]
  public class BookCopiesController : Controller
  {
    private readonly AppDbContext _context;

    public BookCopiesController(AppDbContext context)
    {
      _context = context;
    }

    public async Task<IActionResult> Index()
    {
      var bookCopies = _context.BookCopies
        .Include(b => b.BookTitle)
        .Include(l => l.Library);
      return View(await bookCopies.ToListAsync());
    }

    public IActionResult Create()
    {
      ViewBag.BookTitles = new SelectList(_context.BookTitles, "BookTitleId", "Title");
      ViewBag.Libraries = new SelectList(_context.Libraries, "LibraryId", "LibraryName");
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BookTitleId,LibraryId,Quantity")] BookCopy bookCopy)
    {
      var existingCopy = await _context.BookCopies
        .FirstOrDefaultAsync(bc => bc.BookTitleId == bookCopy.BookTitleId && bc.LibraryId == bookCopy.LibraryId);

      if (existingCopy != null)
      {
        existingCopy.Quantity += bookCopy.Quantity;

        try
        {
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
          ModelState.AddModelError("", "An error occurred while updating the book copy quantity.");
        }
      }
      else
      {
        if (ModelState.IsValid)
        {
          _context.BookCopies.Add(bookCopy);

          try
          {
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
          }
          catch (DbUpdateException ex)
          {
            ModelState.AddModelError("", "An error occurred while creating the book copy.");
          }
        }
      }

      ViewBag.BookTitles = new SelectList(_context.BookTitles, "BookTitleId", "Title", bookCopy.BookTitleId);
      ViewBag.Libraries = new SelectList(_context.Libraries, "LibraryId", "LibraryName", bookCopy.LibraryId);
      return View(bookCopy);
    }

    // GET: BookCopies/Details
    public async Task<IActionResult> Details(int? bookTitleId, int? libraryId)
    {
      if (bookTitleId == null || libraryId == null) return NotFound();

      var bookCopy = await _context.BookCopies
        .Include(b => b.BookTitle)
        .Include(l => l.Library)
        .FirstOrDefaultAsync(m => m.BookTitleId == bookTitleId && m.LibraryId == libraryId);

      if (bookCopy == null) return NotFound();

      return View(bookCopy);
    }

    public async Task<IActionResult> Edit(int? bookTitleId, int? libraryId)
    {
      if (bookTitleId == null || libraryId == null) return NotFound();

      var bookCopy = await _context.BookCopies
        .FirstOrDefaultAsync(m => m.BookTitleId == bookTitleId && m.LibraryId == libraryId);

      if (bookCopy == null) return NotFound();

      ViewBag.BookTitles = new SelectList(_context.BookTitles, "BookTitleId", "Title", bookCopy.BookTitleId);
      ViewBag.Libraries = new SelectList(_context.Libraries, "LibraryId", "LibraryName", bookCopy.LibraryId);
      return View(bookCopy);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int bookTitleId, int libraryId, [Bind("BookTitleId,LibraryId,Quantity")] BookCopy bookCopy)
    {
      if (bookTitleId != bookCopy.BookTitleId || libraryId != bookCopy.LibraryId) return NotFound();

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(bookCopy);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!BookCopyExists(bookCopy.BookTitleId, bookCopy.LibraryId)) return NotFound();
          else throw;
        }
        return RedirectToAction(nameof(Index));
      }
      ViewBag.BookTitles = new SelectList(_context.BookTitles, "BookTitleId", "Title", bookCopy.BookTitleId);
      ViewBag.Libraries = new SelectList(_context.Libraries, "LibraryId", "LibraryName", bookCopy.LibraryId);
      return View(bookCopy);
    }

    public async Task<IActionResult> Delete(int? bookTitleId, int? libraryId)
    {
      if (bookTitleId == null || libraryId == null) return NotFound();

      var bookCopy = await _context.BookCopies
        .Include(b => b.BookTitle)
        .Include(l => l.Library)
        .FirstOrDefaultAsync(m => m.BookTitleId == bookTitleId && m.LibraryId == libraryId);

      if (bookCopy == null) return NotFound();

      return View(bookCopy);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int bookTitleId, int libraryId)
    {
      var bookCopy = await _context.BookCopies.FindAsync(bookTitleId, libraryId);
      if (bookCopy != null)
      {
        _context.BookCopies.Remove(bookCopy);
        await _context.SaveChangesAsync();
      }
      return RedirectToAction(nameof(Index));
    }

    private bool BookCopyExists(int bookTitleId, int libraryId)
    {
      return _context.BookCopies.Any(e => e.BookTitleId == bookTitleId && e.LibraryId == libraryId);
    }
  }
}
