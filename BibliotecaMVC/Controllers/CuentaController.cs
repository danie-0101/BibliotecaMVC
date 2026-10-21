
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers;

public class CuentaController : Controller
{
    private readonly SignInManager<IdentityUser> _signIn;
    private readonly UserManager<IdentityUser> _users;

    public CuentaController(
        SignInManager<IdentityUser> signIn,
        UserManager<IdentityUser> users)
    {
        _signIn = signIn;
        _users = users;
    }

    // INICIAR SESIÓN
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _signIn.PasswordSignInAsync(
            model.Email,
            model.Password,
            false,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return LocalRedirect(
                Url.IsLocalUrl(returnUrl)
                    ? returnUrl!
                    : Url.Action("Index", "Libros")!);
        }

        ModelState.AddModelError(
            "",
            "Credenciales incorrectas o acceso bloqueado.");

        return View(model);
    }

    // REGISTRAR USUARIO
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Registro()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(
        RegistroViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _users.CreateAsync(
            user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    "", error.Description);
            }

            return View(model);
        }

        // Los usuarios nuevos reciben el rol Usuario
        var roleResult = await _users.AddToRoleAsync(
            user, "Usuario");

        if (!roleResult.Succeeded)
        {
            var cleanup = await _users.DeleteAsync(user);

            foreach (var error in roleResult.Errors)
            {
                ModelState.AddModelError(
                    "", error.Description);
            }

            if (!cleanup.Succeeded)
            {
                ModelState.AddModelError(
                    "",
                    "No se pudo revertir el registro. Contacte al administrador.");
            }

            return View(model);
        }

        await _signIn.SignInAsync(user, false);

        return RedirectToAction("Index", "Libros");
    }

    // CERRAR SESIÓN
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }

    // ACCESO DENEGADO
    public IActionResult AccesoDenegado()
    {
        return View();
    }
}
