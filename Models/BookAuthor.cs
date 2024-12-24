using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Models
{
  [PrimaryKey(nameof(BookTitleId), nameof(AuthorName))]
  public class BookAuthor
  {
    public int BookTitleId { get; set; }

    public string? AuthorName { get; set; }

    [ForeignKey("BookTitleId")]
    public BookTitle? BookTitle { get; set; }
  }
}
