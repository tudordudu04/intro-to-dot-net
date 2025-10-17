using Lab03.Persistance;
using Lab03.Requests;
using Lab03.Validators;

namespace Lab03.Handlers;

public class UpdateBookHandler(BookContext context)
{
    private readonly BookContext _context = context;

    public async Task<IResult> Handle(UpdateBookRequest request)
    {
        var validator = new UpdateBookValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors);
        
        var book = await _context.Books.FindAsync(request.Id);
        if (book == null)
            return Results.NotFound();

        book = book with { Title = request.Title, Author = request.Author, Year = request.Year };
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
        return Results.Ok(book);
    }
}