using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
  public class Reader
  {
    [Key]
    public int ReaderCardId { get; set; } // Primary Key

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public ICollection<BookLoan>? BookLoans { get; set; }
  }
}
