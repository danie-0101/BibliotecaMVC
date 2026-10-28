
using Microsoft.AspNetCore.Identity;
using BibliotecaMVC.Data;
using BibliotecaMVC.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURACIÓN DE SERVICIOS
builder.Services.AddControllersWithViews();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Base de datos de BibliotecaMVC
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<BibliotecaMVC.Data.IdentityDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// AUTENTICACIÓN Y ROLES
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<BibliotecaMVC.Data.IdentityDbContext>()
.AddDefaultTokenProviders();

// Configuración de inicio de sesión y acceso denegado
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Cuenta/Login";
    options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
});

// Servicios existentes
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

var app = builder.Build();

// MANEJO GLOBAL DE ERRORES
app.UseExceptionHandler("/Home/Error");

if (!app.Environment.IsDevelopment())
{
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

// CREAR ROLES Y CONFIGURAR ADMINISTRADOR
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<IdentityUser>>();

    // Roles existentes del proyecto
    string[] roles =
    {
        "Admin",
        "Bibliotecario",
        "Usuario",
        "Administrador",
        "Lector"
    };

    foreach (var rol in roles)
    {
        if (!await roleManager.RoleExistsAsync(rol))
        {
            var resultado = await roleManager.CreateAsync(
                new IdentityRole(rol));

            if (!resultado.Succeeded)
            {
                throw new Exception(
                    string.Join("; ",
                        resultado.Errors.Select(e => e.Description)));
            }
        }
    }

    // Migrar roles antiguos sin eliminar las asignaciones anteriores
    foreach (var usuario in userManager.Users.ToList())
    {
        var rolesActuales =
            await userManager.GetRolesAsync(usuario);

        string? nuevoRol = null;

        if (rolesActuales.Contains("Administrador"))
        {
            nuevoRol = "Admin";
        }
        else if (rolesActuales.Contains("Lector"))
        {
            nuevoRol = "Usuario";
        }

        if (nuevoRol != null &&
            !rolesActuales.Contains(nuevoRol))
        {
            var resultado = await userManager
                .AddToRoleAsync(usuario, nuevoRol);

            if (!resultado.Succeeded)
            {
                throw new Exception(
                    string.Join("; ",
                        resultado.Errors.Select(e => e.Description)));
            }
        }
    }

    // CREAR O CONFIGURAR ADMINISTRADOR
    var adminEmail =
        builder.Configuration["BootstrapAdmin:Email"];

    var adminPassword =
        builder.Configuration["BootstrapAdmin:Password"];

    if (!string.IsNullOrWhiteSpace(adminEmail) &&
        !string.IsNullOrWhiteSpace(adminPassword))
    {
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail
            };

            var creado = await userManager.CreateAsync(
                admin, adminPassword);

            if (!creado.Succeeded)
            {
                throw new Exception(
                    string.Join("; ",
                        creado.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            var asignado = await userManager
                .AddToRoleAsync(admin, "Admin");

            if (!asignado.Succeeded)
            {
                throw new Exception(
                    string.Join("; ",
                        asignado.Errors.Select(e => e.Description)));
            }
        }
    }
}

app.Run();
