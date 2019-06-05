using ATKPIMasterFile.BusinessLogic.Aggregators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATKPIMasterFile.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly UserAggregateRoot _userAggregateRoot;

        public HomeController()
        {
        }

        public HomeController(UserAggregateRoot userAggregateRoot)
        {
            _userAggregateRoot = userAggregateRoot;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            //ViewBag.Message = "Your application description page.----"+_userAggregateRoot.GetUserName();

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}