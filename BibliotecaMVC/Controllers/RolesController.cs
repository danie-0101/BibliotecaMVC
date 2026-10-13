using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace BibliotecaMVC.Controllers;
[Authorize(Roles = "Administrador")]
public class RolesController : Controller
{
    private readonly UserManager<IdentityUser> _users;
    public RolesController(UserManager<IdentityUser> users) => _users = users;
    public async Task<IActionResult> Index()
    {
        var result = new List<(string Email, string Rol)>();
        foreach (var user in _users.Users.ToList())
        {
            var roles = await _users.GetRolesAsync(user);
            result.Add((user.Email ?? "", string.Join(", ", roles)));
        }
        return View(result);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Asignar(string email, string rol)
    {
        if (!new[] { "Administrador", "Lector" }.Contains(rol)) return BadRequest();
        var user = await _users.FindByEmailAsync(email);
        if (user == null) return NotFound();
        var roles = await _users.GetRolesAsync(user);
        var admins = await _users.GetUsersInRoleAsync("Administrador");
        if (rol == "Administrador" && !roles.Contains("Administrador") && admins.Count >= 2)
        {
            TempData["Error"] = "Solo se permiten dos administradores.";
            return RedirectToAction(nameof(Index));
        }
        if (roles.Contains("Administrador") && rol != "Administrador" && admins.Count <= 1)
        {
            TempData["Error"] = "No se puede quitar el último administrador.";
            return RedirectToAction(nameof(Index));
        }
        var removed = await _users.RemoveFromRolesAsync(user, roles);
        if (!removed.Succeeded) { TempData["Error"] = "No se pudo actualizar el rol."; return RedirectToAction(nameof(Index)); }
        var assigned = await _users.AddToRoleAsync(user, rol);
        TempData[assigned.Succeeded ? "Success" : "Error"] = assigned.Succeeded ? "Rol actualizado. El usuario debe iniciar sesión nuevamente." : "Error al asignar el rol.";
        return RedirectToAction(nameof(Index));
    }
}
