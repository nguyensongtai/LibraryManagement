using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
  public class BookTitle
  {
    [Key]
    public int BookTitleId { get; set; }

    [Required]
    [MaxLength(200)]
    public string? Title { get; set; }

    public int PublisherId { get; set; }

    [ForeignKey("PublisherId")]
    public Publisher? Publisher { get; set; }

    public ICollection<BookLoan>? BookLoans { get; set; }
    public ICollection<BookAuthor>? BookAuthors { get; set; }
  }
}
