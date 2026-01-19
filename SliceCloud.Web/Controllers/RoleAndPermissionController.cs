using Microsoft.AspNetCore.Mvc;

namespace SliceCloud.Web.Controllers;

public class RoleAndPermissionController() : Controller
{

    #region RoleAndPermission GET
    public IActionResult RoleAndPermission()
    {
        return View();
    }
    #endregion
}
