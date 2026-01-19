using Microsoft.AspNetCore.Mvc;

namespace SliceCloud.Web.Controllers;

public class DashboardController() : Controller
{

    #region Index GET
    public IActionResult Index()
    {
        return View();
    }
    #endregion
}
