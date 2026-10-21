
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers;

[Authorize(Roles = "Admin")]
public class RolesController : Controller
{
    private readonly UserManager<IdentityUser> _users;

    private static readonly string[] RolesPermitidos =
    {
        "Admin",
        "Bibliotecario",
        "Usuario"
    };

    public RolesController(UserManager<IdentityUser> users)
    {
        _users = users;
    }

    // MOSTRAR USUARIOS Y ROLES
    public async Task<IActionResult> Index()
    {
        var resultado = new List<(string Email, string Rol)>();

        foreach (var usuario in _users.Users.ToList())
        {
            var roles = await _users.GetRolesAsync(usuario);

            resultado.Add((
                usuario.Email ?? "",
                string.Join(", ", roles)
            ));
        }

        return View(resultado);
    }

    // ASIGNAR ROL
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Asignar(
        string email,
        string rol)
    {
        if (!RolesPermitidos.Contains(rol))
            return BadRequest();

        var usuario = await _users.FindByEmailAsync(email);

        if (usuario == null)
            return NotFound();

        var rolesActuales =
            await _users.GetRolesAsync(usuario);

        var administradores =
            await _users.GetUsersInRoleAsync("Admin");

        // Proteger al último administrador
        if (rolesActuales.Contains("Admin") &&
            rol != "Admin" &&
            administradores.Count <= 1)
        {
            TempData["Error"] =
                "No se puede quitar el último administrador.";

            return RedirectToAction(nameof(Index));
        }

        // Evitar asignaciones duplicadas
        if (rolesActuales.Contains(rol) &&
            rolesActuales.Count(
                r => RolesPermitidos.Contains(r)) == 1)
        {
            TempData["Success"] =
                "El usuario ya tiene ese rol.";

            return RedirectToAction(nameof(Index));
        }

        // Agregar el rol nuevo
        if (!rolesActuales.Contains(rol))
        {
            var agregado =
                await _users.AddToRoleAsync(usuario, rol);

            if (!agregado.Succeeded)
            {
                TempData["Error"] =
                    string.Join("; ",
                        agregado.Errors.Select(
                            e => e.Description));

                return RedirectToAction(nameof(Index));
            }
        }

        // Quitar otros roles nuevos, pero conservar
        // los heredados para no afectar otros módulos.
        var quitar = rolesActuales
            .Where(r => RolesPermitidos.Contains(r) &&
                        r != rol)
            .ToArray();

        if (quitar.Length > 0)
        {
            var eliminado =
                await _users.RemoveFromRolesAsync(
                    usuario, quitar);

            if (!eliminado.Succeeded)
            {
                if (!rolesActuales.Contains(rol))
                {
                    await _users.RemoveFromRoleAsync(
                        usuario, rol);
                }

                TempData["Error"] =
                    "No se pudieron actualizar los roles: " +
                    string.Join("; ",
                        eliminado.Errors.Select(
                            e => e.Description));

                return RedirectToAction(nameof(Index));
            }
        }

        TempData["Success"] =
            "Rol actualizado. El usuario debe cerrar sesión e ingresar nuevamente.";

        return RedirectToAction(nameof(Index));
    }
}
