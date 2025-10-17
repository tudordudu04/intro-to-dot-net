using Lab03.Persistance;
using Lab03.Requests;

namespace Lab03.Handlers;

public class DeleteBookHandler(BookContext context)
{
    private readonly BookContext _context = context;

    public async Task<IResult> Handle(DeleteBookRequest request)
    {
        var book = await _context.Books.FindAsync(request.Id);
        if (book == null)
            return Results.NotFound();
        
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        
        return Results.NoContent();
    }
}