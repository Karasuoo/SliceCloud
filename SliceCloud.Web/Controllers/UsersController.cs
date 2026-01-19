using Microsoft.AspNetCore.Mvc;

namespace SliceCloud.Web.Controllers;

public class UsersController() : Controller
{

    #region Users GET
    public IActionResult Users()
    {
        return View();
    }
    #endregion
}
