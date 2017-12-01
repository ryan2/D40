using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using D40.Models;
using D40.ViewModels;

namespace D40.Controllers
{
    public class HomeController : Controller
    {
        private D40DBContext db = new D40DBContext();
        public ActionResult Index()
        {
            var users = db.Names;
            var assets = db.D40.Where(s=>s.Returned_Date== null);
            ViewBag.active = users.Where(s => s.Active == true).Count();
            ViewBag.inactive = users.Where(s => s.Active == false).Count();
            ViewBag.computer = assets.Where(s => s.Category == "Computer").Count();
            ViewBag.phone = assets.Where(s => s.Category == "Phone").Count();
            ViewBag.phoneservices = assets.Where(s => s.Category == "Phone Services").Count();
            ViewBag.printer = assets.Where(s => s.Category == "printer").Count();
            ViewBag.totalassets = ViewBag.Computer + ViewBag.phone + ViewBag.phoneservices + ViewBag.printer;
            var tickets = db.Tickets.Where(s => s.Closed_Date != null);
            ViewBag.tickets = tickets.Count();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}