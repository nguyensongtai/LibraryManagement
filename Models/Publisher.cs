using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
  public class Publisher
  {
    [Key]
    public int PublisherId { get; set; } 

    [Required]
    [MaxLength(100)]
    public string? Name { get; set; } 

    [MaxLength(200)]
    public string? Address { get; set; }

    [MaxLength(15)]
    public string? PhoneNumber { get; set; }

    public ICollection<BookTitle>? BookTitles { get; set; }
  }
}
