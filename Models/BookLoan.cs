using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Models
{
  [PrimaryKey(nameof(BookTitleId), nameof(LibraryId), nameof(ReaderCardId), nameof(BorrowDate))]
  public class BookLoan
  {
    public int BookTitleId { get; set; } 

    public int LibraryId { get; set; } 

    public int ReaderCardId { get; set; }

    public DateTime BorrowDate { get; set; } 

    public DateTime? ReturnDate { get; set; }

    [ForeignKey("BookTitleId")]
    public BookTitle? BookTitle { get; set; }

    [ForeignKey("LibraryId")]
    public Library? Library { get; set; }

    [ForeignKey("ReaderCardId")]
    public Reader? Reader { get; set; }
  }
}
