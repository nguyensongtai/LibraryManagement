using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Models
{
  [PrimaryKey(nameof(BookTitleId), nameof(LibraryId))]
  public class BookCopy
  {
    public int BookTitleId { get; set; } 

    public int LibraryId { get; set; } 

    public int Quantity { get; set; }

    [ForeignKey("BookTitleId")]
    public BookTitle? BookTitle { get; set; }

    [ForeignKey("LibraryId")]
    public Library? Library { get; set; }
  }
}
