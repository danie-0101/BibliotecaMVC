using BibliotecaMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Inyección de Dependencias: se registra la relación entre la
// abstracción IAutorService y su implementación concreta AutorService.
// Ciclo de vida Scoped: una instancia por cada solicitud HTTP.
builder.Services.AddScoped<IAutorService, AutorService>();

// Actividad 5 (Reto): para usar la implementación alternativa basta con
// comentar la línea anterior y descomentar la siguiente. El controlador
// AutoresController NO requiere ningún cambio.
// builder.Services.AddScoped<IAutorService, AutorServiceAlternativo>();

// Inyección de Dependencias para Categorías usando ADO.NET
// Se registra la relación entre ICategoriaService y CategoriaService
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
