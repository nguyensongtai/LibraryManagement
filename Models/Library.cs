using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryManagement.Models
{
  public class Library
  {
    public int LibraryId { get; set; } // Primary Key

    public string? LibraryName { get; set; }

    public string? Address { get; set; }

    public ICollection<BookCopy>? BookCopies { get; set; }
    public ICollection<BookLoan>? BookLoans { get; set; }
  }
}

