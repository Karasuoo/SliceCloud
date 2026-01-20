using Microsoft.AspNetCore.Mvc;

namespace SliceCloud.Web.Controllers;

public class OrdersController() : Controller
{

    #region Orders GET
    public IActionResult Orders()
    {
        return View();
    }
    #endregion
}
