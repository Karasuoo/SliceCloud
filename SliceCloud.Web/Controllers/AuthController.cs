using Microsoft.AspNetCore.Mvc;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Web.Controllers;


public class AuthController(IAuthService authService) : Controller
{
    private readonly IAuthService _authService = authService;

    #region Login GET
    public IActionResult Login()
    {
        return View();
    }
    #endregion
}
