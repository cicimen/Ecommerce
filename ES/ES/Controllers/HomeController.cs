using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ES.Models;

namespace ES.Controllers
{
    public class HomeController : Controller
    {
        public ESDBDataContext Db = new ESDBDataContext();

        public ActionResult Index()
        {
            return View(
                        new { 
                            CategoryList = Db.Categories.OrderBy(d=> d.OrderNumber).ToList()
                        }
                );
        }

    }
}
