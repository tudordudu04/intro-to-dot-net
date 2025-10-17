using Lab03.Persistance;
using Lab03.Requests;
using Lab03.Validators;
using Microsoft.EntityFrameworkCore;

namespace Lab03.Handlers;

public class GetBooksByPaginationHandler(BookContext context)
{
    private readonly BookContext _context = context;

    public async Task<IResult> Handler(GetBooksByPaginationRequest request)
    {
        var validator = new GetBooksByPaginationValidator();
        var validatorResults = await validator.ValidateAsync(request);
        if(!validatorResults.IsValid)
            return Results.BadRequest(validatorResults.Errors);

        var books = await _context.Books.ToListAsync(); //aici ar trebui sa fie luat dupa paginare :PP
        return Results.Ok(books);
    }
}