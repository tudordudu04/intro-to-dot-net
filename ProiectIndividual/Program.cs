using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProiectIndividual.Handlers;
using ProiectIndividual.Mapping;
using ProiectIndividual.Persistance;
using ProiectIndividual.Requests;
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
builder.Services.AddScoped<UpdateProductHandler>();
builder.Services.AddScoped<GetProductByIdHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateProductValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AdvancedProductMappingProfile>(), typeof(AdvancedProductMappingProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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

// app.UseGlobalExceptionMiddleware();

app.UseCors("DevCors");

app.UseHttpsRedirection();

app.MapPost("/products", async (CreateProductProfileRequest req, CreateProductHandler handler) =>
    await handler.Handle(req));
app.MapGet("/products", async (GetAllProductsHandler handler) =>
    await handler.Handle(new GetAllProductsRequest()));
app.MapDelete("/products/{id:guid}", async (Guid id, DeleteProductHandler handler) => 
    await handler.Handle(new DeleteProductRequest(id)));
app.MapGet("/products/{id:Guid}", async (Guid id, GetProductByIdHandler handler) =>
    await handler.Handle(new GetProductByIdRequest(id)));
app.MapPut("/products/{id:Guid}", async (Guid id, UpdateProductRequest req, UpdateProductHandler handler) =>
    await handler.Handle(req));
app.Run();
