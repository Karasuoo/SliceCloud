using Microsoft.AspNetCore.Mvc;

namespace SliceCloud.Web.Controllers;

public class TaxesAndFeesController() : Controller
{

    #region TaxesAndFees GET
    public IActionResult TaxesAndFees()
    {
        return View();
    }
    #endregion
}
