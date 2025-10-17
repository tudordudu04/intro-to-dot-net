using Microsoft.EntityFrameworkCore;

namespace Lab03.Persistance;

public class BookContext(DbContextOptions<BookContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; }
}