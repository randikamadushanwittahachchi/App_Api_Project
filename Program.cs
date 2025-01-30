using Microsoft.EntityFrameworkCore;
using Mobile_App_01.backend.Model;

var builder = WebApplication.CreateBuilder(args);

// Add Product Service to the container.

builder.Services.AddDbContext<ProductDbContext>(options => options.UseSqlite("Data Source=Product.db"));


var app = builder.Build();

// Create a Product Migration is created.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => "Hello World!");

app.MapGet("/Product", async (ProductDbContext db) =>
{
    return await db.Products.ToListAsync();
});

app.MapGet("/Product/{id}", async (ProductDbContext db, int id) =>
{
    return await db.Products.FindAsync(id) is Product product ? Results.Ok(product) : Results.NotFound();
});

app.MapPost("/Product", async (ProductDbContext db, Product product) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/Product/{product.Id}", product);
});

app.MapPut("Product/{id}", async (ProductDbContext db, int id, Product product) =>
{
    var existingProduct = await db.Products.FindAsync(id);
    if (existingProduct is null) return Results.NotFound();
    if (id != product.Id)
    {
        return Results.BadRequest("Id mismatch");
    }
    existingProduct.Name = product.Name;
    existingProduct.Description = product.Description;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("Product/{id}", async (ProductDbContext db, int id) =>
{
    var existingProduct = await db.Products.FindAsync(id);
    if (existingProduct is null) return Results.NotFound();
    db.Products.Remove(existingProduct);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
