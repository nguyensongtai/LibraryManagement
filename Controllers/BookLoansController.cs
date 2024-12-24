using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList;

namespace LibraryManagement.Controllers
{
  [Authorize]
  public class BookLoansController : Controller
  {
    private readonly AppDbContext _context;

    public BookLoansController(AppDbContext context)
    {
      _context = context;
    }

    public async Task<IActionResult> Index(int? page, int pageSize = 10)
    {
      int pageNumber = page ?? 1;

      var totalRecords = await _context.BookLoans.CountAsync();

      var loans = await _context.BookLoans
          .Include(b => b.BookTitle)
          .Include(l => l.Library)
          .Include(r => r.Reader)
          .OrderBy(b => b.BorrowDate)
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

      var pagedList = new StaticPagedList<BookLoan>(loans, pageNumber, pageSize, totalRecords);

      return View(pagedList);
    }

    public async Task<IActionResult> Details(int? bookTitleId, int? libraryId, int? readerCardId)
    {
      if (bookTitleId == null || libraryId == null || readerCardId == null)
        return NotFound();

      var loan = await _context.BookLoans
        .Include(b => b.BookTitle)
        .Include(l => l.Library)
        .Include(r => r.Reader)
        .FirstOrDefaultAsync(m =>
          m.BookTitleId == bookTitleId &&
          m.LibraryId == libraryId &&
          m.ReaderCardId == readerCardId);

      if (loan == null) return NotFound();

      return View(loan);
    }

    public IActionResult Create()
    {
      PopulateViewData();
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BookTitleId,LibraryId,ReaderCardId,BorrowDate,ReturnDate")] BookLoan loan)
    {
      if (!await ValidateLoanEntities(loan))
      {
        PopulateViewData(loan);
        return View(loan);
      }

      if (ModelState.IsValid)
      {
        _context.Add(loan);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }

      PopulateViewData(loan);
      return View(loan);
    }

    public async Task<IActionResult> Edit(int? bookTitleId, int? libraryId, int? readerCardId)
    {
      if (bookTitleId == null || libraryId == null || readerCardId == null)
        return NotFound();

      var loan = await _context.BookLoans
          .Include(b => b.BookTitle)
          .Include(l => l.Library)
          .Include(r => r.Reader)
          .FirstOrDefaultAsync(m =>
              m.BookTitleId == bookTitleId &&
              m.LibraryId == libraryId &&
              m.ReaderCardId == readerCardId);

      if (loan == null) return NotFound();

      PopulateViewData(loan);
      return View(loan);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int bookTitleId, int libraryId, int readerCardId,
        [Bind("ReturnDate")] BookLoan updatedLoan)
    {
      var loan = await _context.BookLoans
          .FirstOrDefaultAsync(m =>
              m.BookTitleId == bookTitleId &&
              m.LibraryId == libraryId &&
              m.ReaderCardId == readerCardId);

      if (loan == null) return NotFound();

      loan.ReturnDate = updatedLoan.ReturnDate;

      if (ModelState.IsValid)
      {
        try
        {
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!BookLoanExists(bookTitleId, libraryId, readerCardId))
            return NotFound();

          throw;
        }
      }

      PopulateViewData(loan);
      return View(loan);
    }

    public async Task<IActionResult> Delete(int? bookTitleId, int? libraryId, int? readerCardId)
    {
      if (bookTitleId == null || libraryId == null || readerCardId == null)
        return NotFound();

      var loan = await _context.BookLoans
          .Include(b => b.BookTitle)
          .Include(l => l.Library)
          .Include(r => r.Reader)
          .FirstOrDefaultAsync(m =>
              m.BookTitleId == bookTitleId &&
              m.LibraryId == libraryId &&
              m.ReaderCardId == readerCardId);

      if (loan == null) return NotFound();

      return View(loan);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int bookTitleId, int libraryId, int readerCardId)
    {
      var loan = await _context.BookLoans
          .FirstOrDefaultAsync(m =>
              m.BookTitleId == bookTitleId &&
              m.LibraryId == libraryId &&
              m.ReaderCardId == readerCardId);

      if (loan == null)
        return NotFound();

      _context.BookLoans.Remove(loan);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool BookLoanExists(int bookTitleId, int libraryId, int readerCardId)
    {
      return _context.BookLoans.Any(e =>
          e.BookTitleId == bookTitleId &&
          e.LibraryId == libraryId &&
          e.ReaderCardId == readerCardId);
    }

    private async Task<bool> ValidateLoanEntities(BookLoan loan)
    {
      var bookTitleExists = await _context.BookTitles.AnyAsync(b => b.BookTitleId == loan.BookTitleId);
      var libraryExists = await _context.Libraries.AnyAsync(l => l.LibraryId == loan.LibraryId);
      var readerExists = await _context.Readers.AnyAsync(r => r.ReaderCardId == loan.ReaderCardId);

      if (!bookTitleExists)
        ModelState.AddModelError("BookTitleId", "The selected Book Title does not exist.");
      if (!libraryExists)
        ModelState.AddModelError("LibraryId", "The selected Library does not exist.");
      if (!readerExists)
        ModelState.AddModelError("ReaderCardId", "The selected Reader does not exist.");

      return bookTitleExists && libraryExists && readerExists;
    }

    private void PopulateViewData(BookLoan? loan = null)
    {
      ViewData["BookTitleId"] = new SelectList(_context.BookTitles, "BookTitleId", "Title", loan?.BookTitleId);
      ViewData["LibraryId"] = new SelectList(_context.Libraries, "LibraryId", "LibraryName", loan?.LibraryId);
      ViewData["ReaderCardId"] = new SelectList(_context.Readers, "ReaderCardId", "CardNumber", loan?.ReaderCardId);
      ViewBag.BookTitles = new SelectList(_context.BookTitles, "BookTitleId", "Title");
      ViewBag.Libraries = new SelectList(_context.Libraries, "LibraryId", "LibraryName");
      ViewBag.Readers = new SelectList(_context.Readers, "ReaderCardId", "Name");
    }
  }
}