using Microsoft.EntityFrameworkCore;
using crud.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProductoDb>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapGet("/producto", async (ProductoDb db) =>
    await db.Productos.ToListAsync());

app.MapGet("/producto/{id}", async (int id,ProductoDb db) =>
    await db.Productos.FindAsync(id)
    is Producto producto 
    ? Results.Ok(producto) 
    : Results.NotFound()
);


app.MapPost("/producto", async (Producto producto,ProductoDb db) =>
{
    db.Productos.Add(producto);
    await db.SaveChangesAsync();

    return Results.Created($"/productos/{producto.ID}", producto);
}
    );
app.MapPut("/producto", async (int id, Producto putProducto,ProductoDb db) =>{

    var producto = await db.Productos.FindAsync(id);
    if(producto is null) return Results.NotFound();

    producto.Nombre = putProducto.Nombre;

    await db.SaveChangesAsync();

    return Results.NoContent();

});
app.MapDelete("/producto/{id}", async (int id, ProductoDb db) =>{
    if(await db.Productos.FindAsync(id) is Producto producto){
        db.Productos.Remove(producto);
        await db.SaveChangesAsync();
        return Results.Ok(producto);
    }
    
    return Results.NotFound();
});

app.Run();
