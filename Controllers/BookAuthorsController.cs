using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Controllers
{
  public class BookAuthorsController : Controller
  {
    private readonly AppDbContext _context;

    public BookAuthorsController(AppDbContext context)
    {
      _context = context;
    }

    public async Task<IActionResult> Index()
    {
      var bookAuthors = await _context.BookAuthors
          .Include(ba => ba.BookTitle)
          .ToListAsync();

      return View(bookAuthors);
    }

    public IActionResult Create()
    {
      ViewBag.BookTitles = new SelectList(_context.BookTitles, "BookTitleId", "Title");
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BookTitleId,AuthorName")] BookAuthor bookAuthor)
    {
      var bookExists = await _context.BookTitles.AnyAsync(bt => bt.BookTitleId == bookAuthor.BookTitleId);
      if (!bookExists)
      {
        ModelState.AddModelError("BookTitleId", "The selected Book Title does not exist.");
      }

      if (ModelState.IsValid)
      {
        _context.BookAuthors.Add(bookAuthor);
        try
        {
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
          ModelState.AddModelError("", "An error occurred while saving the book author.");
        }
      }

      ViewBag.BookTitles = new SelectList(_context.BookTitles, "BookTitleId", "Title", bookAuthor.BookTitleId);
      return View(bookAuthor);
    }

    public async Task<IActionResult> Edit(int? bookTitleId, string? authorName)
    {
      if (bookTitleId == null || string.IsNullOrEmpty(authorName))
        return NotFound();

      var bookAuthor = await _context.BookAuthors
          .FirstOrDefaultAsync(ba => ba.BookTitleId == bookTitleId && ba.AuthorName == authorName);

      if (bookAuthor == null)
        return NotFound();

      ViewBag.BookTitles = new SelectList(_context.BookTitles, "BookTitleId", "Title", bookAuthor.BookTitleId);
      return View(bookAuthor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int bookTitleId, string authorName, [Bind("BookTitleId,AuthorName")] BookAuthor updatedBookAuthor)
    {
      if (bookTitleId != updatedBookAuthor.BookTitleId || authorName != updatedBookAuthor.AuthorName)
        return NotFound();

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(updatedBookAuthor);
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!_context.BookAuthors.Any(ba =>
              ba.BookTitleId == updatedBookAuthor.BookTitleId &&
              ba.AuthorName == updatedBookAuthor.AuthorName))
          {
            return NotFound();
          }

          throw;
        }
      }

      ViewBag.BookTitles = new SelectList(_context.BookTitles, "BookTitleId", "Title", updatedBookAuthor.BookTitleId);
      return View(updatedBookAuthor);
    }

    public async Task<IActionResult> Delete(int? bookTitleId, string? authorName)
    {
      if (bookTitleId == null || string.IsNullOrEmpty(authorName))
        return NotFound();

      var bookAuthor = await _context.BookAuthors
          .Include(ba => ba.BookTitle)
          .FirstOrDefaultAsync(ba => ba.BookTitleId == bookTitleId && ba.AuthorName == authorName);

      if (bookAuthor == null)
        return NotFound();

      return View(bookAuthor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int bookTitleId, string authorName)
    {
      var bookAuthor = await _context.BookAuthors
          .FirstOrDefaultAsync(ba => ba.BookTitleId == bookTitleId && ba.AuthorName == authorName);

      if (bookAuthor != null)
      {
        _context.BookAuthors.Remove(bookAuthor);
        await _context.SaveChangesAsync();
      }

      return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int? bookTitleId, string authorName)
    {
      if (bookTitleId == null || string.IsNullOrEmpty(authorName)) return NotFound();

      var authorBook = await _context.BookAuthors
        .Include(b => b.BookTitle)
        .FirstOrDefaultAsync(m => m.BookTitleId == bookTitleId && m.AuthorName == authorName);

      if (authorBook == null) return NotFound();

      return View(authorBook);
    }
  }
}
