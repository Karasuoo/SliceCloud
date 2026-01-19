using Microsoft.AspNetCore.Mvc;

namespace SliceCloud.Web.Controllers;

public class CustomersController() : Controller
{

    #region Customers GET
    public IActionResult Customers()
    {
        return View();
    }
    #endregion
}
