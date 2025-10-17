using Lab03.Persistance;
using Lab03.Requests;
using Lab03.Validators;

namespace Lab03.Handlers;

public class CreateBookHandler(BookContext context)
{
    private readonly BookContext _context = context;

    public async Task<IResult> Handle(CreateBookRequest request)
    {
        var validator = new CreateBookValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors);
        
        var book = new Book(0, request.Title, request.Author, request.Year);
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        
        return Results.Created($"/books/{book.Id}", book);
    }
    
}