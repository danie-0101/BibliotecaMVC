using Microsoft.AspNetCore.Identity;
using BibliotecaMVC.Data;
using BibliotecaMVC.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar el DbContext de Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<BibliotecaMVC.Data.IdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<BibliotecaMVC.Data.IdentityDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Cuenta/Login";
    options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
});
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

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Los roles se crean al iniciar; los usuarios se registran desde /Cuenta/Registro.
using (var scope = app.Services.CreateScope())
{
    var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var rol in new[] { "Administrador", "Lector" })
        if (!await roles.RoleExistsAsync(rol))
        {
            var resultado = await roles.CreateAsync(new IdentityRole(rol));
            if (!resultado.Succeeded) throw new Exception(string.Join("; ", resultado.Errors.Select(e => e.Description)));
        }
    // Configurar mediante dotnet user-secrets, nunca guardar contraseñas en el repositorio.
    var adminEmail = builder.Configuration["BootstrapAdmin:Email"];
    var adminPassword = builder.Configuration["BootstrapAdmin:Password"];
    if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
    {
        var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var admin = await users.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new IdentityUser { UserName = adminEmail, Email = adminEmail };
            var created = await users.CreateAsync(admin, adminPassword);
            if (!created.Succeeded) throw new Exception(string.Join("; ", created.Errors.Select(e => e.Description)));
        }
        if (!await users.IsInRoleAsync(admin, "Administrador"))
        {
            var assigned = await users.AddToRoleAsync(admin, "Administrador");
            if (!assigned.Succeeded) throw new Exception(string.Join("; ", assigned.Errors.Select(e => e.Description)));
        }
    }
}
app.Run();