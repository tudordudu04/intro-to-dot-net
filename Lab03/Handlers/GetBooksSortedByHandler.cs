using Lab03.Persistance;
using Lab03.Requests;
using Lab03.Validators;
using Microsoft.EntityFrameworkCore;

namespace Lab03.Handlers;

public class GetBooksSortedByHandler(BookContext context)
{
    private readonly BookContext _context = context;
    
    public async Task<IResult> Handle(GetBooksSortedByRequest request)
    {
        var validator = new GetBooksSortedByValidator();
        var validatorResults = await validator.ValidateAsync(request);
        if (!validatorResults.IsValid)
            return Results.BadRequest();
        
        IQueryable<Book> query = _context.Books;

        if (request.SortBy == "title")
            query = query.OrderBy(b => b.Title);
        else if (request.SortBy == "year")
            query = query.OrderBy(b => b.Year);

        var books = await query.ToListAsync();
        return Results.Ok(books);
    }
}