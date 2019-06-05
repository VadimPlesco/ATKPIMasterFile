using ATKPIMasterFile.BusinessLogic.Aggregators;
using ATKPIMasterFile.BusinessLogic.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATKPIMasterFile.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminAggregateRoot _adminAggregateRoot;

        public AdminController(AdminAggregateRoot adminAggregateRoot)
        {
            _adminAggregateRoot = adminAggregateRoot;
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageUsers()
        {
            return View(_adminAggregateRoot.GetAllUsers());
        }

        [HttpPost]
        public ActionResult ManageUsers(ManageUsersViewModel viewModel)
        {
            return View(_adminAggregateRoot.GetAllUsers());
        }

        
        public ActionResult ManageUsersTable()
        {
            return View(_adminAggregateRoot.GetAllUsers());
        }

        [HttpPost]
        public JsonResult StudentList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
               // Thread.Sleep(200);
                var students = _adminAggregateRoot.GetAllUsers().ManageUsers;
                var studentCount = students.Count;
                return Json(new { Result = "OK", Records = students, TotalRecordCount = studentCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult ManageUser()
        {
            return View(new ManageUserViewModel { UserId = _adminAggregateRoot.GetCurrentUserId() });
        }
    }
}