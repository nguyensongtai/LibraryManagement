using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;

namespace LibraryManagement.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<BookTitle> BookTitles { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Library> Libraries { get; set; }
    public DbSet<Reader> Readers { get; set; }
    public DbSet<BookLoan> BookLoans { get; set; }
    public DbSet<BookCopy> BookCopies { get; set; }
    public DbSet<BookAuthor> BookAuthors { get; set; }
  }
}