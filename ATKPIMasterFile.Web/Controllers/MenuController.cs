using ATKPIMasterFile.BusinessLogic.Aggregators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATKPIMasterFile.Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly UserAggregateRoot _userAggregateRoot;

        public MenuController(UserAggregateRoot userAggregateRoot)
        {
            _userAggregateRoot = userAggregateRoot;
        }

        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Menu()
        {
            return PartialView(_userAggregateRoot.GetUser());
        }
    }
}