using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;
using PagedList;

namespace LibraryManagement.Controllers
{
  public class BookTitlesController : Controller
  {
    private readonly AppDbContext _context;

    public BookTitlesController(AppDbContext context)
    {
      _context = context;
    }

    public async Task<IActionResult> Index(int? page, int pageSize = 10)
    {
      int pageNumber = page ?? 1;
      var totalRecords = await _context.BookTitles.CountAsync();

      var bookTitles = await _context.BookTitles
          .Include(b => b.Publisher)
          .OrderBy(b => b.Title)
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

      var pagedList = new StaticPagedList<BookTitle>(bookTitles, pageNumber, pageSize, totalRecords);
      return View(pagedList);
    }

    public async Task<IActionResult> Details(int? id)
    {
      if (id == null) return NotFound();

      var bookTitle = await _context.BookTitles
          .Include(b => b.Publisher)
          .FirstOrDefaultAsync(m => m.BookTitleId == id);

      if (bookTitle == null) return NotFound();

      return View(bookTitle);
    }

    public IActionResult Create()
    {
      ViewBag.PublisherList = new SelectList(_context.Publishers, "PublisherId", "Name");
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BookTitleId,Title,PublisherId")] BookTitle bookTitle)
    {
      // Validate PublisherId exists in the Publishers table
      var publisherExists = await _context.Publishers.AnyAsync(p => p.PublisherId == bookTitle.PublisherId);
      if (!publisherExists)
      {
        ModelState.AddModelError("PublisherId", "The selected Publisher does not exist.");
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Add(bookTitle);
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
          ModelState.AddModelError("", "Unable to save changes. Please ensure the Publisher exists.");
        }
      }
      ViewBag.PublisherList = new SelectList(_context.Publishers, "PublisherId", "Name", bookTitle.PublisherId);
      return View(bookTitle);
    }

    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null) return NotFound();

      var bookTitle = await _context.BookTitles.FindAsync(id);

      if (bookTitle == null) return NotFound();

      ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "Name", bookTitle.PublisherId);
      return View(bookTitle);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("BookTitleId,Title,PublisherId")] BookTitle bookTitle)
    {
      if (id != bookTitle.BookTitleId) return NotFound();

      // Validate PublisherId exists in the Publishers table
      var publisherExists = await _context.Publishers.AnyAsync(p => p.PublisherId == bookTitle.PublisherId);
      if (!publisherExists)
      {
        ModelState.AddModelError("PublisherId", "The selected Publisher does not exist.");
        ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "Name", bookTitle.PublisherId);
        return View(bookTitle);
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(bookTitle);
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
          ModelState.AddModelError("", "Unable to save changes. Please ensure the Publisher exists.");
        }
      }

      ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "Name", bookTitle.PublisherId);
      return View(bookTitle);
    }

    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null) return NotFound();

      var bookTitle = await _context.BookTitles
          .Include(b => b.Publisher)
          .FirstOrDefaultAsync(m => m.BookTitleId == id);

      if (bookTitle == null) return NotFound();

      return View(bookTitle);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var bookTitle = await _context.BookTitles.FindAsync(id);
      if (bookTitle != null)
      {
        _context.BookTitles.Remove(bookTitle);
        await _context.SaveChangesAsync();
      }
      return RedirectToAction(nameof(Index));
    }

    private bool BookTitleExists(int id)
    {
      return _context.BookTitles.Any(e => e.BookTitleId == id);
    }
  }
}
