using Microsoft.AspNetCore.Mvc;

namespace SliceCloud.Web.Controllers;

public class TableAndSectionController() : Controller
{

    #region TableAndSection GET
    public IActionResult TableAndSection()
    {
        return View();
    }
    #endregion
}
