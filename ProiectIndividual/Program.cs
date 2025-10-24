using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProiectIndividual.Persistance;
using ProiectIndividual.Products;
using ProiectIndividual.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc
    (
        "v1",
        new OpenApiInfo
        {
            Title = "Product Management API",
            Version = "v1",
            Description = "API for managing products.",
            Contact = new OpenApiContact
            {
                Name = "API Support",
                Email = "support@example.com",


            }
        });
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ProductManagementContext>(options =>
    options.UseSqlite("Data Source=productmanagement.db"));
builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<GetAllProductsHandler>();
builder.Services.AddScoped<DeleteProductHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductManagementContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI
        (
            c=>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API V1");
                c.RoutePrefix = string.Empty;
                c.DisplayRequestDuration();
            }
        );
    
    app.MapOpenApi();
}


app.UseHttpsRedirection();

app.MapPost("/products", async (CreateProductProfileRequest req, CreateProductHandler handler) =>
    await handler.Handle(req));
app.MapGet("/products", async (GetAllProductsHandler handler) =>
    await handler.Handle(new GetAllProductsRequest()));
app.MapDelete("/products/{id:guid}", async (Guid id, DeleteProductHandler handler) =>
{
    await handler.Handle(new DeleteProductRequest(id));
});
app.Run();
