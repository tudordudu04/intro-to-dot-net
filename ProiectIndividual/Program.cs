using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProiectIndividual.Handlers;
using ProiectIndividual.Mapping;
using ProiectIndividual.Persistance;
using ProiectIndividual.Requests;
using ProiectIndividual.Validators;
using ProiectIndividual.Middleware;
using ProiectIndividual.Products;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product Management API",
        Version = "v1",
        Description = "API for managing products.",
        Contact = new OpenApiContact { Name = "API Support", Email = "support@example.com" }
    });
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ProductManagementContext>(o => o.UseSqlite("Data Source=productmanagement.db"));
builder.Services.AddHttpContextAccessor();

// Handlers
builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<GetAllProductsHandler>();
builder.Services.AddScoped<DeleteProductHandler>();
builder.Services.AddScoped<UpdateProductHandler>();
builder.Services.AddScoped<GetProductByIdHandler>();

// Validators
builder.Services.AddScoped<IValidator<CreateProductProfileRequest>, CreateProductValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
builder.Services.AddFluentValidationAutoValidation();

// AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AdvancedProductMappingProfile>(), typeof(AdvancedProductMappingProfile));

builder.Services.AddCors(o =>
{
    o.AddPolicy("DevCors", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<ProductManagementContext>();
    ctx.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API V1");
        c.RoutePrefix = string.Empty;
        c.DisplayRequestDuration();
    });
    app.MapOpenApi();
}

app.UseMiddleware<CorrelationMiddleware>();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("DevCors");
app.UseHttpsRedirection();

app.MapPost("/products", async (CreateProductProfileRequest req, CreateProductHandler handler) =>
        await handler.Handle(req))
    .WithName("CreateProduct")
    .WithSummary("Create a new product")
    .WithDescription("Creates a product profile and returns its mapped DTO with derived display fields.")
    .Produces<ProductProfileDTO>(201)
    .Produces(400)
    .Produces(500);

app.MapGet("/products", async (GetAllProductsHandler handler) =>
    await handler.Handle(new GetAllProductsRequest()));
app.MapDelete("/products/{id:guid}", async (Guid id, DeleteProductHandler handler) =>
    await handler.Handle(new DeleteProductRequest(id)));
app.MapGet("/products/{id:Guid}", async (Guid id, GetProductByIdHandler handler) =>
    await handler.Handle(new GetProductByIdRequest(id)));
app.MapPut("/products/{id:Guid}", async (Guid id, UpdateProductRequest req, UpdateProductHandler handler) =>
    await handler.Handle(req));

app.Run();
