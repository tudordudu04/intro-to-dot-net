using Lab03.Persistance;
using Lab03.Requests;
using Lab03.Validators;
using Microsoft.EntityFrameworkCore;

namespace Lab03.Handlers;

public class GetBooksByAuthorHandler(BookContext context)
{
    private readonly BookContext _context = context;

    public async Task<IResult> Handle(GetBooksByAuthorRequest request)
    {
        var validator = new GetBooksByAuthorValidator();
        var validatorResults = await validator.ValidateAsync(request);
        if (!validatorResults.IsValid)
            return Results.BadRequest();
        
        var books = await _context.Books
            .Where(b => b.Author == request.Author)
            .ToListAsync();

        return Results.Ok(books);
    }
}