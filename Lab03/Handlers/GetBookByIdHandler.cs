using Lab03.Persistance;
using Lab03.Requests;
using Lab03.Validators;

namespace Lab03.Handlers;

public class GetBookByIdHandler(BookContext context)
{
    private readonly BookContext _context = context;

    public async Task<IResult> Handle(GetBookByIdRequest request)
    {
        var validator = new GetBookByIdValidator();
        var validatorResults = await validator.ValidateAsync(request);
        if(!validatorResults.IsValid)
            return Results.BadRequest(validatorResults.Errors);
        
        var book = await _context.Books.FindAsync(request.Id);
        if (book == null)
            return Results.NotFound();
        return Results.Ok(book);
    }
}