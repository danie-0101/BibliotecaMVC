using BibliotecaMVC.Data;
using BibliotecaMVC.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar el DbContext de Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de Dependencias: se registra la relación entre la
// abstracción IAutorService y su implementación concreta AutorService.
builder.Services.AddScoped<IAutorService, AutorService>();

// Actividad 5 (Reto): implementación alternativa
// builder.Services.AddScoped<IAutorService, AutorServiceAlternativo>();

// Inyección de Dependencias para Categorías usando ADO.NET
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();