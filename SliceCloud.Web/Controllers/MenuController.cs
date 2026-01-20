using Microsoft.AspNetCore.Mvc;

namespace SliceCloud.Web.Controllers;

public class MenuController() : Controller
{

    #region Menu GET
    public IActionResult Menu()
    {
        return View();
    }
    #endregion
}
