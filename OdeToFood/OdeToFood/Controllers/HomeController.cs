using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using OdeToFood.Models;
using PagedList;
using System.Web.UI;

namespace OdeToFood.Controllers
{
    public class HomeController : Controller
    {
        IOdeToFoodDb _db = new OdeToFoodDb();

        public HomeController()
        {
            _db = new OdeToFoodDb();
        }

        //bu unit test yapmak için
        public HomeController(IOdeToFoodDb db)
        {
            _db = db;
        }

        public ActionResult Autocomplete(string term)
        {
            var model = _db.Query<Restaurant>()
                .Where(r => r.Name.StartsWith(term))
                .Take(10)
                .Select(r => new
                {
                    label =r.Name
                });

            return Json(model,JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        [OutputCache(Duration = 60)]
        public ActionResult SayHello()
        {
            return Content("Hello");
        }

        [OutputCache(CacheProfile="Long", VaryByHeader="X-Requested-With;Accept-Language", Location=OutputCacheLocation.Server)]
        public ActionResult Index(string searchTerm = null,int page=1)
        {
            //var model = // _db.Restaurants.ToList();
            //from r in _db.Restaurants
            //orderby r.Reviews.Average(review => review.Rating) descending
            //select new RestaurantListViewModel
            //{
            //    Id= r.Id,
            //    Name=r.Name,
            //    City=r.City,
            //    Country=r.Country,
            //    CountOfReviews = r.Reviews.Count()
            //};

            var model = _db.Query<Restaurant>().OrderByDescending(r => r.Reviews.Average(review => review.Rating))
                .Where(r=> searchTerm == null || r.Name.StartsWith(searchTerm))
                //.Take(10)
                .Select(r => new RestaurantListViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    City = r.City,
                    Country = r.Country,
                    CountOfReviews = r.Reviews.Count()
                }).ToPagedList(page,10);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_Restaurants",model);
            }

            return View(model);
        }

        [Authorize]
        public ActionResult About()
        {
            var model = new AboutModel 
            { 
                Name ="Scott", Location="Maryland, USA"
            };

            return View(model);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if(_db != null)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    
    }
}
