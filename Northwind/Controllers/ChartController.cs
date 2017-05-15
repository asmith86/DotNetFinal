using System.Collections.Generic;
using System.Web.Mvc;
using Northwind.Models;

namespace Northwind.Controllers
{
    public class ChartController : Controller
    {
        public JsonResult ChartData()
        {
            List<ChartData> cd = new List<ChartData>()
            {
                new ChartData("project A", "81531.946062978"),
                new ChartData("project B", "67313.916593765"),
                new ChartData("project C", "92440.723376746")
            };

            return Json(cd, JsonRequestBehavior.AllowGet);
        }
    }
}
