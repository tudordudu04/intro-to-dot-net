using Lab03.Persistance;
using Lab03.Requests;
using Microsoft.EntityFrameworkCore;

namespace Lab03.Handlers;

public class GetAllBooksHandler(BookContext context)
{
    private readonly BookContext _context = context;

    public async Task<IResult> Handle(GetAllBooksRequest request)
    {
        var books = await _context.Books.ToListAsync();
        return Results.Ok(books);
    }
}