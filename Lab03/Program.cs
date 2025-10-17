using FluentValidation;
using Lab03.Handlers;
using Lab03.Requests;
using Lab03.Persistance;
using Lab03.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<BookContext>(options =>
    options.UseSqlite("Data Source=books.db"));

builder.Services.AddScoped<CreateBookHandler>();
builder.Services.AddScoped<GetAllBooksHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookValidator>();
builder.Services.AddScoped<GetBookByIdHandler>();
builder.Services.AddScoped<UpdateBookHandler>();
builder.Services.AddScoped<DeleteBookHandler>();
builder.Services.AddScoped<GetBooksByPaginationHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<GetBooksByPaginationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateBookValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookValidator>();
builder.Services.AddScoped<GetBooksByAuthorHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<GetBooksByAuthorValidator>();
builder.Services.AddScoped<GetBooksSortedByHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<GetBooksSortedByValidator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/books", async (CreateBookRequest req, CreateBookHandler handler) =>
    await handler.Handle(req));
app.MapDelete("/books/{id:int}", async (int id, DeleteBookHandler handler) =>
    {
    var request = new DeleteBookRequest(id);
    return await handler.Handle(request);
    });
app.MapPut("/books/{id:int}", async (UpdateBookRequest req, UpdateBookHandler handler) =>
    await handler.Handle(req));
app.MapGet("/books", async (int page, int pageSize, GetBooksByPaginationHandler handler) =>
{
    var request = new GetBooksByPaginationRequest(page, pageSize);
    return await handler.Handler(request);
});
app.MapGet("/allBooks", async (GetAllBooksHandler handler) =>
    await handler.Handle(new GetAllBooksRequest()));
app.MapGet("/books/{id:int}", async (int id, GetBookByIdHandler handler) =>
{
    var request = new GetBookByIdRequest(id);
    return await handler.Handle(request);
});
app.MapGet("/booksByAuthor", async (string author, GetBooksByAuthorHandler handler) =>
{
    var request = new GetBooksByAuthorRequest(author);
    return await handler.Handle(request);
});
app.MapGet("/booksSortedBy", async (string sortedBy, GetBooksSortedByHandler handler) =>
{
    var request = new GetBooksSortedByRequest(sortedBy);
    return await handler.Handle(request);
});

app.Run();
