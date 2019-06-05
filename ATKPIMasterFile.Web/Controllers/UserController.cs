using ATKPIMasterFile.BusinessLogic.Aggregators;
using ATKPIMasterFile.BusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;

namespace ATKPIMasterFile.Web.Controllers
{
    public class UserController : Controller
    {

        private readonly UserAggregateRoot _userAggregateRoot;

        public UserController()
        {
        }

        public UserController(UserAggregateRoot userAggregateRoot)
        {
            _userAggregateRoot = userAggregateRoot;
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }

        // GET: User
        [HttpGet]
        public ActionResult Index()
        {
            var dateTimeNow = DateTime.Now.AddMonths(-1);

            // if (SelectedYear.HasValue == false) 
            //SelectedYear = (short)(dateTimeNow.Year);
            var years = new List<short> { 2015, 2016, 2017, 2018, 2019 };
            ViewBag.SelectedYear = new SelectList(years, (short)dateTimeNow.Year);

           // if (SelectedMonth.HasValue == false) SelectedMonth = (short)dateTimeNow.Month;
            var moths = new List<short> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            ViewBag.SelectedMonth = new SelectList(moths, (short)dateTimeNow.Month);
            

            // if (SelectedYearEnd.HasValue == false) SelectedYearEnd = (short)(dateTimeNow.Year);
            ViewBag.SelectedYearEnd = new SelectList(years, (short)dateTimeNow.Year);

            //if (SelectedMonthEnd.HasValue == false) SelectedMonthEnd = (short)dateTimeNow.Month;
            ViewBag.SelectedMonthEnd = new SelectList(moths, (short)dateTimeNow.Month);

            //if (SelectedFilial.HasValue == false) SelectedFilial = 0;

            var filials = _userAggregateRoot.GetAllFilials();

            ViewBag.SelectedFilial = new SelectList(filials, "FilialId", "Name", 0);

            //if (SelectedFilial.Value == 0)
            return View();

            //return View(_userAggregateRoot.GetKPI(SelectedFilial.Value, SelectedMonth.Value, SelectedYear.Value, 
              //  SelectedMonthEnd.Value, SelectedYearEnd.Value));
            // "../User/KPIFilial"
            //return PartialView("KPIFilial", _userAggregateRoot.GetKPI(SelectedFilial.Value, SelectedMonth.Value, SelectedYear.Value, 
            //    SelectedMonthEnd.Value, SelectedYearEnd.Value));
        }

        [HttpPost]
        public ActionResult Index(int? SelectedFilial, short? SelectedYear, short? SelectedMonth, short? SelectedYearEnd, short? SelectedMonthEnd, bool? submit)
        {
            //var dateTimeNow = DateTime.Now.AddMonths(-1);
            //var ww = ViewBag.SelectedYear;
            //if (SelectedYear.HasValue == false) SelectedYear = (short)(dateTimeNow.Year);
            var years = new List<short> { 2015, 2016, 2017, 2018, 2019 };
            ViewBag.SelectedYear = new SelectList(years, SelectedYear);

            //if (SelectedMonth.HasValue == false) SelectedMonth = (short)dateTimeNow.Month;
            var moths = new List<short> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            ViewBag.SelectedMonth = new SelectList(moths, SelectedMonth);

            //if (SelectedYearEnd.HasValue == false) SelectedYearEnd = (short)(dateTimeNow.Year);
            ViewBag.SelectedYearEnd = new SelectList(years, SelectedYearEnd);

            //if (SelectedMonthEnd.HasValue == false) SelectedMonthEnd = (short)dateTimeNow.Month;
            ViewBag.SelectedMonthEnd = new SelectList(moths, SelectedMonthEnd);

            //if (SelectedFilial.HasValue == false) SelectedFilial = 0;

            var filials = _userAggregateRoot.GetAllFilials();

            ViewBag.SelectedFilial = new SelectList(filials, "FilialId", "Name", SelectedFilial);

            if (SelectedFilial.Value == 0)
                return PartialView("KPIFilial");

           
            return PartialView("KPIFilial", _userAggregateRoot.GetKPI(SelectedFilial.Value, SelectedMonth.Value, SelectedYear.Value,
                SelectedMonthEnd.Value, SelectedYearEnd.Value));
        }


        [HttpGet]
        public ActionResult KPIFilialPlans()
        {
            var dateTimeNow = DateTime.Now.AddMonths(-1);

           
            var years = new List<short> { 2019 };
            ViewBag.SelectedYear = new SelectList(years, years[0]);

           
            var moths = new List<short> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            ViewBag.SelectedMonth = new SelectList(moths, moths[0]);

            
            ViewBag.SelectedYearEnd = new SelectList(years, years[0]);

            var mothsEnd = new List<short> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            ViewBag.SelectedMonthEnd = new SelectList(mothsEnd, mothsEnd[0]);

            ViewBag.SelectedCorr = false;



            var filials = _userAggregateRoot.GetAllFilials();
            ViewBag.SelectedFilial = new SelectList(filials, "FilialId", "Name", 0);

            
            return View();

        }


        [HttpPost]
        public ActionResult KPIFilialPlans(int? SelectedFilial, short? SelectedYear, short? SelectedMonth, short? SelectedYearEnd, short? SelectedMonthEnd, bool SelectedCorr)
        {
            
            var years = new List<short> { 2019 };
            ViewBag.SelectedYear = new SelectList(years, SelectedYear);
        
            var moths = new List<short> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            ViewBag.SelectedMonth = new SelectList(moths, SelectedMonth);

            ViewBag.SelectedYearEnd = new SelectList(years, SelectedYearEnd);

            ViewBag.SelectedMonthEnd = new SelectList(moths, SelectedMonthEnd);

            ViewBag.SelectedCorr = SelectedCorr;

            var filials = _userAggregateRoot.GetAllFilials();

            ViewBag.SelectedFilial = new SelectList(filials, "FilialId", "Name", SelectedFilial);

            if (SelectedFilial.Value == 0)
                return PartialView("KPIFilialPlanResult");


            return PartialView("KPIFilialPlanResult", _userAggregateRoot.KPIFilialPlans(SelectedFilial.Value, SelectedMonth.Value, SelectedYear.Value,
                SelectedMonthEnd.Value, SelectedYearEnd.Value, SelectedCorr));
        }


        [HttpGet]
        public ActionResult Details(long? user)
        { 

            return View(_userAggregateRoot.GetUserViewModel());
        }

        [HttpGet]
        public ActionResult AvatarDetails(long? user)
        {
            //ModelState.Clear();
            var userViewModel = _userAggregateRoot.GetUserViewModel();
            return PartialView("Details", userViewModel);
            //return AjaxView(_userAggregateRoot.GetUserViewModel());
            //return View(userViewModel);
        }


        public ActionResult Details1(long? user)
        {

            return Json(_userAggregateRoot.GetUserViewModel(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SalariesDetails(string salariesName, short month, short year, short monthEnd, short yearEnd, int filial)
        {
            return View(_userAggregateRoot.GetSalariesDetails(filial, month, year, monthEnd, yearEnd, salariesName));
        }

        public ActionResult AutosDetails(string autosName, short month, short year, short monthEnd, short yearEnd, int filial)
        {
            ViewBag.RowName = autosName;
            return View(_userAggregateRoot.GetAutosDetails(filial, month, year, monthEnd, yearEnd, autosName));
        }

        public ActionResult PublicServicesDetails(short month, short year, short monthEnd, short yearEnd, int filial)
        {
            return View(_userAggregateRoot.GetPublicServicesDetails(filial, month, year, monthEnd, yearEnd));
        }

        public ActionResult GoodsDetails(string brendName, short month, short year, short monthEnd, short yearEnd, int filial)
        {
            ViewBag.BrendName = brendName;
            return View(_userAggregateRoot.GetGoodsDetails(filial, brendName, month, year, monthEnd, yearEnd));
        }


        public ActionResult AutosReport(int? SelectedFilial, short? SelectedYear, short? SelectedMonth, short? SelectedYearEnd, short? SelectedMonthEnd, 
            short? SelectedCombustibleType, short? SelectedAutoType, short? SelectedDepartment, int? SelectedProject)
        {
            var dateTimeNow = DateTime.Now.AddMonths(-1);

            if (SelectedYear.HasValue == false) SelectedYear = (short)dateTimeNow.Year;
            var years = new List<short> { 2015, 2016, 2017, 2018, 2019 };
            ViewBag.SelectedYear = new SelectList(years, SelectedYear);

            if (SelectedMonth.HasValue == false) SelectedMonth = (short)dateTimeNow.Month;
            var moths = new List<short> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            ViewBag.SelectedMonth = new SelectList(moths, SelectedMonth);

            if (SelectedYearEnd.HasValue == false) SelectedYearEnd = (short)dateTimeNow.Year;
            ViewBag.SelectedYearEnd = new SelectList(years, SelectedYearEnd);

            if (SelectedMonthEnd.HasValue == false) SelectedMonthEnd = (short)dateTimeNow.Month;
            ViewBag.SelectedMonthEnd = new SelectList(moths, SelectedMonthEnd);

            if (SelectedFilial.HasValue == false) SelectedFilial = -1;
            var filials = _userAggregateRoot.GetAllFilialsForReport();
            ViewBag.SelectedFilial = new SelectList(filials, "FilialId", "Name", SelectedFilial);

            if (SelectedCombustibleType.HasValue == false) SelectedCombustibleType = -1;
            ViewBag.SelectedCombustibleType = new SelectList(new[]{
                  new SelectListItem{ Text="Все",    Value="-1"},
                  new SelectListItem{ Text="-",      Value="0"},
                  new SelectListItem{ Text="Бензин", Value="1"},
                  new SelectListItem{ Text="Дизель", Value="2"}
                }, "Value", "Text", SelectedCombustibleType);

            if (SelectedAutoType.HasValue == false) SelectedAutoType = -1;
            ViewBag.SelectedAutoType = new SelectList(new[]{
                  new SelectListItem{ Text="Все",           Value="-1"},
                  new SelectListItem{ Text="-",             Value="0"},
                  new SelectListItem{ Text="Легковая",      Value="1"},
                  new SelectListItem{ Text="Грузовая",      Value="2"},
                  new SelectListItem{ Text="Админ",         Value="3"},
                  new SelectListItem{ Text="Магистральный", Value="4"}
                }, "Value", "Text", SelectedAutoType);


            //if (SelectedDepartment.HasValue == false) SelectedDepartment = 0;
            //ViewBag.SelectedDepartment = new SelectList(new[]{
            //      new SelectListItem{ Text="Все",           Value="-1"},
            //      new SelectListItem{ Text="-",             Value="0"}
            //    }, "Value", "Text", SelectedDepartment);

            if (SelectedProject.HasValue == false) SelectedProject = 0;
            var projects = _userAggregateRoot.GetAllGetProjectsForReport();
            ViewBag.SelectedProject = new SelectList(projects, "ProjectId", "Name", SelectedProject);

            if (SelectedFilial.Value == 0)
                return View();

            return View();
        }


        public ActionResult GetDepartments(int filial)
        {
            var result = new List<SelectListItem>();

            var departments = _userAggregateRoot.GetDepartmentsByFilialId(filial);

            foreach (var department in departments)
            {
                result.Add(new SelectListItem { Text = department.Name, Value = department.DepartmentId.ToString() });
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetPosts(int filial, int department)
        {
            var result = new List<SelectListItem>();

            var postsByDepartmen = _userAggregateRoot.GetPostsByFilialDepartmentId(filial, department);

            foreach (var post in postsByDepartmen)
            {
                result.Add(new SelectListItem { Text = post, Value = post });
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNumbers(string query)
        {
            var result = _userAggregateRoot.GetAutoNumbers(query);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult AutosReport(int? SelectedFilial, int? SelectedDepartment, short? SelectedYear, short? SelectedMonth, short? SelectedYearEnd, short? SelectedMonthEnd, 
            short? SelectedCombustibleType, short? SelectedAutoType, int? SelectedProject, string Number, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {

            if (!SelectedFilial.HasValue)
                return Json(new { Result = "OK" });

            if (SelectedFilial.Value == -1)
                return Json(new { Result = "OK" });


            var history = _userAggregateRoot.
                GetLastHistoryBySourceAction(BusinessLogic.Model.ActionHistoryType.UpdateData, 
                                             BusinessLogic.Model.SourceHistoryType.AutosReport);

            var autosReportLastUpdate = string.Empty;

            if (history != null)
                autosReportLastUpdate = history.Date.ToShortDateString();


            try
            {
                var autos = _userAggregateRoot.GetAutosReport(SelectedFilial.Value, SelectedDepartment.Value, SelectedMonth.Value,
                    SelectedYear.Value, SelectedMonthEnd.Value, SelectedYearEnd.Value, SelectedCombustibleType.Value,
                    SelectedAutoType.Value, Number, SelectedProject.Value, jtSorting);

                var autoCount = autos.Count;
                if (jtPageSize > 0)
                    autos = autos.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = autos, TotalRecordCount = autoCount, AutosReportLastUpdate = autosReportLastUpdate });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        public ActionResult GetEmployees(string query)
        {
            var result = _userAggregateRoot.GetEmployees(query);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SalariesReport(int? SelectedFilial, short? SelectedYear, short? SelectedMonth, short? SelectedYearEnd, 
            short? SelectedMonthEnd, short? SelectedCombustibleType, short? SelectedAutoType, short? SelectedDepartment, int? SelectedProject)
        {
            var dateTimeNow = DateTime.Now.AddMonths(-1);

            if (SelectedYear.HasValue == false) SelectedYear = (short)dateTimeNow.Year;
            var years = new List<short> { 2015, 2016, 2017, 2018, 2019 };
            ViewBag.SelectedYear = new SelectList(years, SelectedYear);

            if (SelectedMonth.HasValue == false) SelectedMonth = (short)dateTimeNow.Month;
            var moths = new List<short> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            ViewBag.SelectedMonth = new SelectList(moths, SelectedMonth);

            if (SelectedYearEnd.HasValue == false) SelectedYearEnd = (short)dateTimeNow.Year;
            ViewBag.SelectedYearEnd = new SelectList(years, SelectedYearEnd);

            if (SelectedMonthEnd.HasValue == false) SelectedMonthEnd = (short)dateTimeNow.Month;
            ViewBag.SelectedMonthEnd = new SelectList(moths, SelectedMonthEnd);

            if (SelectedFilial.HasValue == false) SelectedFilial = -1;
            var filials = _userAggregateRoot.GetAllFilialsForReport();
            ViewBag.SelectedFilial = new SelectList(filials, "FilialId", "Name", SelectedFilial);

            if (SelectedCombustibleType.HasValue == false) SelectedCombustibleType = -1;
            ViewBag.SelectedPersonType = new MultiSelectList(new[]{ 
                  new SelectListItem{ Text="Все",              Value="-1"},
                  new SelectListItem{ Text="Обычный",          Value="1"},
                  new SelectListItem{ Text="Поддержка продаж", Value="2"},
                  new SelectListItem{ Text="Администрация",    Value="3"}
                }, "Value", "Text");


            if (SelectedProject.HasValue == false) SelectedProject = 0;
            var projects = _userAggregateRoot.GetAllGetProjectsForReport();
            ViewBag.SelectedProject = new SelectList(projects, "ProjectId", "Name", SelectedProject);



            if (SelectedFilial.Value == 0)
                return View();

            return View();
        }


        [HttpPost]
        public JsonResult SalariesReport(int? SelectedFilial, int? SelectedDepartment, short? SelectedYear, short? SelectedMonth, 
            short? SelectedYearEnd, short? SelectedMonthEnd, short? SelectedPersonType, int? SelectedProject, string SelectedPost, string Employee, 
            int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {

            if (!SelectedFilial.HasValue)
                return Json(new { Result = "OK" });

            if (SelectedFilial.Value == -1)
                return Json(new { Result = "OK" });


            try
            {
                var salaries = _userAggregateRoot.GetSalariesReport(SelectedFilial.Value, SelectedDepartment.Value, SelectedMonth.Value,
                    SelectedYear.Value, SelectedMonthEnd.Value, SelectedYearEnd.Value, SelectedPersonType.Value, SelectedProject.Value,
                    SelectedPost, Employee, jtSorting);
        

                var salariesCount = salaries.Count;
                if (jtPageSize > 0)
                    salaries = salaries.Skip(jtStartIndex).Take(jtPageSize).ToList();
                var jsonResult = Json(new { Result = "OK", Records = salaries, TotalRecordCount = salariesCount });
                //jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult Avatar(/*FileUpload*/HttpPostedFileBase file)
        {
            string fileName = System.IO.Path.GetFileName(file.FileName);

            _userAggregateRoot.ResizeSaveImage(file.InputStream, fileName); 

            return Json(new { status = true, avatarImg = file.InputStream });
        }

        //[OutputCache(Location = OutputCacheLocation.None)]
        //public ActionResult Show(int id)
        //{
        //    ModelState.Clear();
        //    var user = _userAggregateRoot.GetUserById(id);
        //    return File(user.Avatar, "image/jpg");
        //}


        public ActionResult Maket(short? SelectedYear, short? SelectedMonth)
        {
            var dateTimeNow = DateTime.Now.AddMonths(-1);

            if (SelectedYear.HasValue == false) SelectedYear = (short)dateTimeNow.Year;
            var years = new List<short> { 2015, 2016 };
            ViewBag.SelectedYear = new SelectList(years, SelectedYear);

            if (SelectedMonth.HasValue == false) SelectedMonth = (short)dateTimeNow.Month;
            var moths = new List<short> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            ViewBag.SelectedMonth = new SelectList(moths, SelectedMonth);

            return View();
        }

        public ActionResult MaketLogistic(short? SelectedYear, short? SelectedMonth)
        {
            var dateTimeNow = DateTime.Now.AddMonths(-1);

            if (SelectedYear.HasValue == false) SelectedYear = (short)dateTimeNow.Year;
            var years = new List<short> { 2015, 2016 };
            ViewBag.SelectedYear = new SelectList(years, SelectedYear);

            if (SelectedMonth.HasValue == false) SelectedMonth = (short)dateTimeNow.Month;
            var moths = new List<short> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            ViewBag.SelectedMonth = new SelectList(moths, SelectedMonth);

            return View();
        }

        public ActionResult MaketLogisticDetails(short? SelectedYear, short? SelectedMonth, string SelectedFilial, string SelectedType)
        {
            var dateTimeNow = DateTime.Now.AddMonths(-1);

            if (SelectedYear.HasValue == false) SelectedYear = (short)dateTimeNow.Year;
            var years = new List<short> { 2015, 2016 };
            ViewBag.SelectedYear = new SelectList(years, SelectedYear);

            if (SelectedMonth.HasValue == false) SelectedMonth = (short)dateTimeNow.Month;
            var moths = new List<short> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            ViewBag.SelectedMonth = new SelectList(moths, SelectedMonth);

            var filials = new List<string> { "Chisinau", "Cahul" };
            ViewBag.SelectedFilial = new SelectList(filials, SelectedFilial);

            var types = new List<string> { "Легковая", "Грузовая" };
            ViewBag.SelectedType = new SelectList(types, SelectedType);

            return View();
        }

    }
}