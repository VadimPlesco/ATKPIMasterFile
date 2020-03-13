using ATKPIMasterFile.BusinessLogic.Commands;
using ATKPIMasterFile.BusinessLogic.Model;
using ATKPIMasterFile.BusinessLogic.Services;
using ATKPIMasterFile.BusinessLogic.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.Helpers;
using System.IO;
using System.Web.Hosting;
using System.Linq.Expressions;

namespace ATKPIMasterFile.BusinessLogic.Aggregators
{
    public class UserAggregateRoot
    {
        private readonly ATDbEntities _ade;
        private readonly UserCommands _userCommands;
        private readonly WebContextService _contextService;

        public UserAggregateRoot(ATDbEntities ade, UserCommands userCommands, WebContextService contextService)
        {
            _ade = ade;
            _userCommands = userCommands;
            _contextService = contextService;
        }

        public User GetUserById(long userId)
        {
            return _userCommands.GeUserById(_ade, userId);
        }

        public User GetUser(string email, string password)
        {
            return _userCommands.GetUser(_ade, email, password);
        }

        public UserViewModel GetUser()
        {
            var userId = _contextService.GetCurrentUserId();
            var user = GetUserById(userId);
            var name = user == null ? "" : user.UserName;

            var userAvatarPath = "";
            if (string.IsNullOrEmpty(user.AvatarPath))
            {
                userAvatarPath = user.Sex ? "/Content/Uploads/male-blank.gif" : "/Content/Uploads/female-blank.gif";
            }
            else
            {
                userAvatarPath = user.AvatarPath;
            }

            return new UserViewModel { AvatarUrl = userAvatarPath, AvatarWidth = 40, IsModel = false, Name = name, Url = "/", UserId = userId };
        }

        public List<Filial> GetAllFilials()
        {
            var filials = _userCommands.GetAllFilials(_ade);
            filials.RemoveAll(p => p.FilialId == 8);
            filials.Find(p => p.FilialId == 1).Name = "Региональный отдел (AquaTrade)";

            var userId = _contextService.GetCurrentUserId();
            var user = GetUserById(userId);
            var userRoles = _userCommands.GeUserRoleById(_ade, userId);

            var allowFilials = new List<int>();
            allowFilials.Add(user.Filial);
            if (user.Filial == 3)//Костыль
                allowFilials.Add(6);
            if (user.Filial == 4)//Костыль
                allowFilials.Add(5);
            if (user.Filial == 6)//Костыль
                allowFilials.Add(15);

            if (userRoles.Exists(p => p.RoleId == 3) && !(userRoles.Exists(p => p.RoleId == 1) || userRoles.Exists(p => p.RoleId == 2)))
            {
                //filials.RemoveAll(p => p.FilialId != user.Filial);
                filials.RemoveAll(p => !allowFilials.Contains(p.FilialId));
            }

            filials.Insert(0, new BusinessLogic.Model.Filial { FilialId = 0, Name = "-" });//

            return filials;
        }

        public List<Filial> GetAllFilialsForReport()
        {
            var filials = _userCommands.GetAllFilialsForReport(_ade);
            //filials.RemoveAll(p => p.FilialId == 8);
            //filials.Find(p => p.FilialId == 1).Name = "Региональный отдел (AquaTrade)";

            var userId = _contextService.GetCurrentUserId();
            var user = GetUserById(userId);
            var userRoles = _userCommands.GeUserRoleById(_ade, userId);

            var allowFilials = new List<int>();
            allowFilials.Add(user.Filial);
            if (user.Filial == 3)//Костыль
                allowFilials.Add(6);
            if (user.Filial == 4)//Костыль
                allowFilials.Add(5);
            if (user.Filial == 6)//Костыль
                allowFilials.Add(15);

            if (userRoles.Exists(p => p.RoleId == 3) && !(userRoles.Exists(p => p.RoleId == 1) || userRoles.Exists(p => p.RoleId == 2)))
            {
                //filials.RemoveAll(p => p.FilialId != user.Filial);
                if (user.Filial != 9)
                    filials.RemoveAll(p => !allowFilials.Contains(p.FilialId));
            }

            if (filials.Count > 2)
                filials.Insert(0, new BusinessLogic.Model.Filial { FilialId = 0, Name = "- Все - " });//

            filials.Insert(0, new BusinessLogic.Model.Filial { FilialId = -1, Name = "- Не выбран - " });//


            return filials;
        }

        public List<Department> GetDepartmentsByFilialId(int filialId)
        {
            var departments = new List<BusinessLogic.Model.Department>();

            if (filialId == -1)
                departments.Insert(0, new BusinessLogic.Model.Department { DepartmentId = -1, Name = "- Не выбран - " });
            else if (filialId == 0)
                departments.Insert(0, new BusinessLogic.Model.Department { DepartmentId = 0, Name = "- Все - " });
            else
            {
                departments = _userCommands.GetDepartmentsByFilialId(_ade, filialId);

                var userId = _contextService.GetCurrentUserId();
                var user = GetUserById(userId);
                var userRoles = _userCommands.GeUserRoleById(_ade, userId);

                if (userRoles.Exists(p => p.RoleId == 3) && !(userRoles.Exists(p => p.RoleId == 1) || userRoles.Exists(p => p.RoleId == 2)))
                {
                    if (user.Filial == 2)
                    {
                        departments.RemoveAll(p => p.DepartmentId != user.Department);
                    }
                }
            }

            if (departments.Count > 1)
                departments.Insert(0, new BusinessLogic.Model.Department { DepartmentId = 0, Name = "- Все - " });//

            return departments;
        }


        public List<Project> GetAllGetProjectsForReport()
        {
            var projects = _userCommands.GetProjects(_ade);
            //filials.RemoveAll(p => p.FilialId == 8);
            //filials.Find(p => p.FilialId == 1).Name = "Региональный отдел (AquaTrade)";

            var userId = _contextService.GetCurrentUserId();
            var user = GetUserById(userId);
            //var userRoles = _userCommands.GeUserRoleById(_ade, userId);

            //var allowFilials = new List<int>();
            //allowFilials.Add(user.Filial);
            //if (user.Filial == 3)//Костыль
            //    allowFilials.Add(6);

            //if (userRoles.Exists(p => p.RoleId == 3) && !(userRoles.Exists(p => p.RoleId == 1) || userRoles.Exists(p => p.RoleId == 2)))
            //{
            //    //filials.RemoveAll(p => p.FilialId != user.Filial);
            //    filials.RemoveAll(p => !allowFilials.Contains(p.FilialId));
            //}

            if (user.Filial == 9)
            {
                projects.RemoveAll(p => p.ProjectId != 2);
            }

            if (projects.Count > 1)
                projects.Insert(0, new BusinessLogic.Model.Project { ProjectId = 0, Name = "- Все - " });//

            //projects.Insert(0, new BusinessLogic.Model.Project { FilialId = -1, Name = "- Не выбран - " });//


            return projects;
        }

        public List<string> GetPostsByFilialDepartmentId(int filialId, int departmentId)
        {
            var posts = new List<string>();

            if (filialId == -1)
                posts.Insert(0, "- Не выбран - ");
            else if (filialId == 0 && departmentId == 0)
            {
                var allPosts = _userCommands.GetPosts(_ade);
                var groupAllPosts = allPosts.GroupBy(p => p.Post)
                    .Select(n => new
                    {
                        PostName = n.Key
                    })
                    .OrderBy(c => c.PostName)
                    .ToList();

                foreach (var post in groupAllPosts)
                    posts.Add(post.PostName);
            }
            else if (filialId > 0 && departmentId == 0)
            {
                var postsByFilial = _userCommands.GetPostsByFilialId(_ade, filialId);
                var groupPostsByFilial = postsByFilial.GroupBy(p => p.Post)
                    .Select(n => new
                    {
                        PostName = n.Key
                    })
                    .OrderBy(c => c.PostName)
                    .ToList();

                foreach (var post in groupPostsByFilial)
                    posts.Add(post.PostName);
            }
            else
            {
                var postsByDepartment = _userCommands.GetPostsByDepartmentId(_ade, departmentId).OrderBy(p => p.Post).ToList();

                foreach (var post in postsByDepartment)
                    posts.Add(post.Post);
            }

            if (posts.Count > 1)
                posts.Insert(0, "- Все - ");//

            return posts;
        }

        public KPIViewModel GetKPI(int filialId, short month, short year, short monthEnd, short yearEnd)
        {
            KPIViewModel kpi = new KPIViewModel();
            //kpi.Rows = new List<KPIRow>();

            kpi.SalaryPart = new KPISalaryPartRow();
            kpi.SalaryPart.Rows = new List<KPIRow>();
            kpi.SalaryPartPast = new KPISalaryPartRow();
            kpi.SalaryPartPast.Rows = new List<KPIRow>();

            kpi.AutoPart = new KPIAutoPartRow();
            kpi.AutoPart.Rows = new List<KPIRow>();
            kpi.AutoPartPast = new KPIAutoPartRow();
            kpi.AutoPartPast.Rows = new List<KPIRow>();

            kpi.SalePart = new KPISalePartRow();
            kpi.SalePart.Rows = new List<KPIRow>();

            kpi.AdminPart = new KPIAdminPartRow();
            kpi.AdminPart.Rows = new List<KPIRow>();
            kpi.AdminPartPast = new KPIAdminPartRow();
            kpi.AdminPartPast.Rows = new List<KPIRow>();

            kpi.TotalPart = new KPITotalPartRow();
            kpi.TotalPart.Rows = new List<KPIRowT>();

            /////////////////////////////////////////////////////////////////////////
            var goods = _userCommands.GetGoodsByFilial(_ade, filialId, month, year, monthEnd, yearEnd);

            var goodsAERPVodovoz = _userCommands.GetGoodsAERPVodovozByFilial(_ade, filialId, month, year, monthEnd, yearEnd);

            foreach (var gv in goodsAERPVodovoz)
            {
                var i = goods.FindIndex(p => p.BrendId == gv.BrendId);
                if (i == -1)
                {
                    goods.Add(gv);
                }
                else
                {
                    goods[i].Price += gv.Price;
                    goods[i].PrimeCost += gv.PrimeCost;
                    goods[i].Sum += gv.Sum;
                }
            }

            var primeCost = GetTotalPrimeCost(goods);
            var discount = GetTotalDiscount(goods);
            var discountPepsi = GetTotalPepsiDiscount(goods);
            ////////////////////////////////////////////////////////////////////////////////

            short yearPast = (short)(year - 1);
            short yearEndPast = (short)(yearEnd - 1);

            var adminExpenseGroup = GetAdminExpense(filialId, month, year, monthEnd, yearEnd);
            var adminExpenseGroupPast = GetAdminExpense(filialId, month, yearPast, monthEnd, yearEndPast);

            FillKPISales(filialId, month, year, kpi, goods, goodsAERPVodovoz, monthEnd, yearEnd);//закоментить

            List<Salary> curSalaries = new List<Salary>();

            FillKPISalaries(filialId, month, year, monthEnd, yearEnd, kpi.SalaryPart,
                adminExpenseGroup != null ? adminExpenseGroup.SalaryTax : 0, out curSalaries);
            FillKPISalariesPast(filialId, month, yearPast, monthEnd, yearEndPast, kpi.SalaryPart, kpi.SalaryPartPast,
                adminExpenseGroupPast != null ? adminExpenseGroupPast.SalaryTax : 0, curSalaries);

            FillKPIAutos(filialId, month, year, monthEnd, yearEnd, kpi.AutoPart,
                adminExpenseGroup != null ? adminExpenseGroup.WashTransport : 0,
                adminExpenseGroup != null ? adminExpenseGroup.TransportRent : 0);
            FillKPIAutosPast(filialId, month, yearPast, monthEnd, yearEndPast, kpi.AutoPart, kpi.AutoPartPast,
                adminExpenseGroupPast != null ? adminExpenseGroupPast.WashTransport : 0,
                adminExpenseGroupPast != null ? adminExpenseGroupPast.TransportRent : 0);

            var transportTotal = kpi.AutoPart.Rows.Find(p => p.Name == "Затраты на транспорт");
            var personalTotal = kpi.SalaryPart.Rows.Find(p => p.Name == "Затраты на персонал");
            FillKPIAdmins(filialId, month, year, monthEnd, yearEnd, kpi.AdminPart,
                transportTotal, personalTotal,
                adminExpenseGroup != null ? adminExpenseGroup.MobileTelephony : 0,
                adminExpenseGroup != null ? adminExpenseGroup.PremisesRent : 0
                /*, primeCost, discount, discountPepsi*/);
            var transportTotalPast = kpi.AutoPartPast.Rows.Find(p => p.Name == "Затраты на транспорт");
            var personalTotalPast = kpi.SalaryPartPast.Rows.Find(p => p.Name == "Затраты на персонал");
            FillKPIAdminsPast(filialId, month, yearPast, monthEnd, yearEndPast,
                kpi.AdminPart, kpi.AdminPartPast,
                transportTotalPast, personalTotalPast,
                adminExpenseGroupPast != null ? adminExpenseGroupPast.MobileTelephony : 0,
                adminExpenseGroupPast != null ? adminExpenseGroupPast.PremisesRent : 0
                /*, primeCost, discount, discountPepsi*/);


            // FillKPITotal(/*filialId, month, year,*/ kpi);

            return kpi;
        }


        public KPIViewModel KPIFilialPlans(int filialId, short month, short year, short monthEnd, short yearEnd, bool corr)
        {
            KPIViewModel kpi = new KPIViewModel();
            //kpi.Rows = new List<KPIRow>();

            kpi.SalaryPart = new KPISalaryPartRow();
            kpi.SalaryPart.Rows = new List<KPIRow>();
            kpi.SalaryPartPast = new KPISalaryPartRow();
            kpi.SalaryPartPast.Rows = new List<KPIRow>();

            kpi.AutoPart = new KPIAutoPartRow();
            kpi.AutoPart.Rows = new List<KPIRow>();
            kpi.AutoPartPast = new KPIAutoPartRow();
            kpi.AutoPartPast.Rows = new List<KPIRow>();

            //kpi.SalePart = new KPISalePartRow();
            //kpi.SalePart.Rows = new List<KPIRow>();

            kpi.AdminPart = new KPIAdminPartRow();
            kpi.AdminPart.Rows = new List<KPIRow>();
            kpi.AdminPartPast = new KPIAdminPartRow();
            kpi.AdminPartPast.Rows = new List<KPIRow>();

            //kpi.TotalPart = new KPITotalPartRow();
            //kpi.TotalPart.Rows = new List<KPIRowT>();

            //var goods = _userCommands.GetGoodsByFilial(_ade, filialId, month, year, monthEnd, yearEnd);

            //var goodsAERPVodovoz = _userCommands.GetGoodsAERPVodovozByFilial(_ade, filialId, month, year, monthEnd, yearEnd);

            //foreach (var gv in goodsAERPVodovoz)
            //{
            //    var i = goods.FindIndex(p => p.BrendId == gv.BrendId);
            //    if (i == -1)
            //    {
            //        goods.Add(gv);
            //    }
            //    else
            //    {
            //        goods[i].Price += gv.Price;
            //        goods[i].PrimeCost += gv.PrimeCost;
            //        goods[i].Sum += gv.Sum;
            //    }
            //}

            //var primeCost = GetTotalPrimeCost(goods);
            //var discount = GetTotalDiscount(goods);
            //var discountPepsi = GetTotalPepsiDiscount(goods);

            short yearPast = (short)(year - 1);
            short yearEndPast = (short)(yearEnd - 1);

            DateTime dateStart = new DateTime(yearPast, month, 1);
            DateTime dateEnd = new DateTime(yearEndPast, monthEnd, 1);
            //dateStart = dateStart.AddMonths(-1);
            //dateEnd = dateEnd.AddMonths(-1);

            var adminExpenseGroup = GetAdminExpense(filialId, (short)dateStart.Month, (short)dateStart.Year, (short)dateEnd.Month, (short)dateEnd.Year);
            //var adminExpenseGroupPast = GetAdminExpense(filialId, month, yearPast, monthEnd, yearEndPast);

            //FillKPISales(filialId, month, year, kpi, goods, goodsAERPVodovoz, monthEnd, yearEnd);//закоментить

            List<Salary> curSalaries = new List<Salary>();

            FillKPIPlansSalaries(filialId, month, year, monthEnd, yearEnd, kpi.SalaryPart,
                adminExpenseGroup != null ? adminExpenseGroup.SalaryTax : 0, corr, out curSalaries);
            //FillKPISalariesPast(filialId, month, yearPast, monthEnd, yearEndPast, kpi.SalaryPart, kpi.SalaryPartPast,
            //    adminExpenseGroupPast != null ? adminExpenseGroupPast.SalaryTax : 0, curSalaries);


            FillKPIPlansAutos(filialId, month, year, monthEnd, yearEnd, kpi.AutoPart,
                adminExpenseGroup != null ? adminExpenseGroup.WashTransport : 0,
                adminExpenseGroup != null ? adminExpenseGroup.TransportRent : 0);


            var transportTotal = kpi.AutoPart.Rows.Find(p => p.Name == "Затраты на транспорт");
            var personalTotal = kpi.SalaryPart.Rows.Find(p => p.Name == "Затраты на персонал");

            FillKPIPlansAdmins(filialId, month, year, monthEnd, yearEnd, kpi.AdminPart,
                transportTotal, personalTotal,
                adminExpenseGroup != null ? adminExpenseGroup.MobileTelephony : 0,
                adminExpenseGroup != null ? adminExpenseGroup.PremisesRent : 0
                /*, primeCost, discount, discountPepsi*/);


            return kpi;
        }


        public AdminExpenseGroupViewModel GetAdminExpense(int filialId, short month, short year, short monthEnd, short yearEnd)
        {
            var adminExpense = _userCommands.GetPublicAdminExpenseByFilial(_ade, filialId, month, year, monthEnd, yearEnd);

            return adminExpense
               .GroupBy(n => n.FilialId)
               .Select(n => new AdminExpenseGroupViewModel
               {
                   FilialId = n.Key,
                   PremisesRent = n.Sum(c => c.PremisesRent),
                   MobileTelephony = n.Sum(c => c.MobileTelephony),
                   SalaryTax = n.Sum(c => c.SalaryTax),
                   TransportRent = n.Sum(c => c.TransportRent),
                   WashTransport = n.Sum(c => c.WashTransport)
               })
               .FirstOrDefault();
        }


        public double? GetTotalPrimeCost(List<GoodViewModel> goods)
        {
            return goods.Sum(p => p.PrimeCost);
        }

        public double? GetTotalDiscount(List<GoodViewModel> goods)
        {
            return goods.Sum(p => p.Price) - goods.Sum(p => p.Sum);
        }

        public double? GetTotalPepsiDiscount(List<GoodViewModel> goods)
        {
            var pepsiBrendId = new long[] { 14, 15, 16, 17, 18, 27 };

            return goods.Where(x => pepsiBrendId.Contains(x.BrendId)).Sum(p => p.Price) - goods.Where(x => pepsiBrendId.Contains(x.BrendId)).Sum(p => p.Sum);
        }

        public double GetGoodsSumByFilial(int filialId, short month, short year, bool byYear)
        {
            var goodsSum = _userCommands.GetGoodsSumByFilial(_ade, filialId, month, year, byYear);

            return goodsSum ?? 0;
        }

        public void FillKPISales(int filialId, short month, short year, KPIViewModel kpi, List<GoodViewModel> goods, List<GoodViewModel> goodsVodovoz, short monthEnd, short yearEnd)
        {
            var brendPlans = _userCommands.GetBrendsPlansByFilial(_ade, filialId, month, year, monthEnd, yearEnd);
            //var goods = _userCommands.GetGoodsByFilial(_ade, filialId, month, year);

            double totalPlan = 0, totalFact = 0;
            double tempPlan = 0, tempFact = 0;
            int i = 0;
            foreach (var brendP in brendPlans)
            {
                var name = brendP.BrendName;
                double fact = 0;
                var good = goods.FirstOrDefault(p => p.BrendId == brendP.BrendId);
                if (good != null)
                {
                    fact = good.Sum;
                    goods.Remove(good);
                }


                //var goodV = goodsVodovoz.FirstOrDefault(p => p.BrendId == brendP.BrendId);

                //if (goodV != null)
                //    fact += goodV.Sum;

                var plan = brendP.Sum;

                totalPlan += plan;
                totalFact += fact;

                if (name == "Comecotech(Пакеты, Стаканчики)" || name == "Shanghai (Пакеты)" || name == "MaxiPac")
                {
                    name = "Пакеты + Стаканчики";
                }

                double per = (fact - plan) / plan * 100;

                var netProfit = kpi.SalePart.Rows.Find(p => p.Name == name);
                if (netProfit != null)
                {
                    netProfit.Fact += Convert.ToInt32(fact);
                    netProfit.Plan += Convert.ToInt32(plan);
                    netProfit.Percent = (double)(netProfit.Fact - netProfit.Plan) / netProfit.Plan * 100;
                }
                else
                    kpi.SalePart.Rows.Add(new KPIRow { Name = name, Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per, Link = true });
            }

            if (goods.Count > 0)
            {
                foreach (var good in goods)
                {
                    totalFact += good.Sum;
                    if (good.BrendName == "Shanghai (Пакеты)" || good.BrendName == "Comecotech(Пакеты, Стаканчики)" || good.BrendName == "MaxiPac")
                    {
                        var netProfit = kpi.SalePart.Rows.Find(p => p.Name == "Пакеты + Стаканчики");
                        if (netProfit != null)
                        {
                            netProfit.Fact += Convert.ToInt32(good.Sum);
                            netProfit.Percent = (double)(netProfit.Fact - netProfit.Plan) / netProfit.Plan * 100;
                        }
                        else
                            kpi.SalePart.Rows.Add(new KPIRow { Name = good.BrendName, Fact = Convert.ToInt32(good.Sum), Link = true });
                    }
                    else
                        kpi.SalePart.Rows.Add(new KPIRow { Name = good.BrendName, Fact = Convert.ToInt32(good.Sum), Link = true });
                }
            }

            double tatalPer = (totalFact - totalPlan) / totalPlan * 100;
            kpi.SalePart.Rows.Insert(0, new KPIRow { Name = "Объемы реализации", Plan = Convert.ToInt32(totalPlan), Fact = Convert.ToInt32(totalFact), Percent = tatalPer });
        }

        private int GetNumberMonth(short month, short year, short monthEnd, short yearEnd)
        {
            var date1 = new DateTime(year, month, 1);
            var date2 = new DateTime(yearEnd, monthEnd, 1);

            return Math.Abs(((date1.Year - date2.Year) * 12) + date1.Month - date2.Month) + 1;
        }

        public void FillKPIAutos(int filialId, short month, short year, short monthEnd, short yearEnd, KPIAutoPartRow kpiAuto, double? carWash, double? leaseTruck)
        {
            var departmentId = 0;
            if (filialId == 2)
                departmentId = 10;
            if (filialId == 1)
                departmentId = 7;

            var autos = _userCommands.GetAutosByFilial(_ade, filialId, month, year, monthEnd, yearEnd, departmentId);

            var autosPlansInit = _userCommands.GetAutosPlansByFilial(_ade, filialId, month, year, monthEnd, yearEnd, departmentId);
            if (autosPlansInit == null || autosPlansInit.Count == 0)
                return;
            var autosPlans = autosPlansInit
               .GroupBy(n => n.FilialId)
               .Select(n => new
               {
                   FilialId = n.Key,
                   Car = n.Sum(c => c.Car),
                   Truck = n.Sum(c => c.Truck),
                   Сombustible = n.Sum(c => c.Сombustible),
                   Tires = n.Sum(c => c.Tires),
                   Accumulators = n.Sum(c => c.Accumulators),
                   MaintenanceCar = n.Sum(c => c.MaintenanceCar),
                   MaintenanceTruck = n.Sum(c => c.MaintenanceTruck),
                   Spares = n.Sum(c => c.Spares),
                   Services = n.Sum(c => c.Services),
                   СarWash = n.Sum(c => c.СarWash),
                   LeaseTruck = n.Sum(c => c.LeaseTruck)
               })
               .First();

            var truck = autos.Where(p => p.Type == AutoType.Truck).ToList();
            var car = autos.Where(p => p.Type == AutoType.Car).ToList();
            var distrCar = autos.Where(p => p.Type == AutoType.Distrib).ToList();
            var admin = autos.Where(p => p.Type == AutoType.Admin).ToList();
            var none = autos.Where(p => p.Type == AutoType.None).ToList();

            var months = GetNumberMonth(month, year, monthEnd, yearEnd);
            var distrCarPlan = 31;

            var fact = autos.Count();
            //if (filialId == 3)
            //    fact += 4 * months;
            var plan = autosPlans.Car + autosPlans.Truck;
            if (filialId == 1)
                plan += distrCarPlan * months;
            double per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Кол-во авто", Plan = plan, Fact = fact, Percent = per });

            fact = truck.Count();
            //if (filialId == 3)
            //    fact += 4;
            plan = autosPlans.Truck; per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Грузовой транспорт", Plan = plan, Fact = fact, Percent = per, Link = true });

            fact = car.Count(); plan = autosPlans.Car; per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Легковой транспорт", Plan = plan, Fact = fact, Percent = per, Link = true });

            if (filialId == 1)
            {
                fact = distrCar.Count(); plan = distrCarPlan; per = (double)(fact - plan) / plan * 100;
                kpiAuto.Rows.Add(new KPIRow { Name = "Дистрибьюторский транспорт", Plan = plan, Fact = fact, Percent = per, Link = true });
            }

            kpiAuto.Rows.Add(new KPIRow { Name = "Админ транспорт", Plan = 0, Fact = admin.Count(), Percent = 0 });
            kpiAuto.Rows.Add(new KPIRow { Name = "Не распределенный транспорт(кары)", Plan = 0, Fact = none.Count(), Percent = 0 });


            var combustibleTotal = autos.Sum(p => p.Liters * p.СombustiblePrice);
            var tiresAccumulatorsTotal = (autos.Sum(p => p.Tires) + autos.Sum(p => p.Accumulators)) * 1.2;
            var sparesTotal = autos.Sum(p => p.Spares) * 1.2;
            var servicesTotal = (autos.Sum(p => p.Repairs) + autos.Sum(p => p.Services)) * 1.2;

            var maintenanceCar = (car.Sum(p => p.Expenses) + car.Sum(p => p.Lubricants) + car.Sum(p => p.TestareAuto)) * 1.2;
            var maintenanceTruck = (truck.Sum(p => p.Expenses) + truck.Sum(p => p.Lubricants) + truck.Sum(p => p.TestareAuto)) * 1.2;
            var maintenanceTotal = maintenanceCar + maintenanceTruck;

            var autoTotal = combustibleTotal + tiresAccumulatorsTotal + sparesTotal + maintenanceTotal + servicesTotal + (carWash ?? 0) + (leaseTruck ?? 0);
            var autoTotalPlan = autosPlans.Сombustible + autosPlans.Tires + autosPlans.Accumulators + autosPlans.Spares
                                + autosPlans.MaintenanceCar + autosPlans.MaintenanceTruck + autosPlans.Services
                                + (autosPlans.СarWash ?? 0) + (autosPlans.LeaseTruck ?? 0);


            fact = Convert.ToInt32(autoTotal); plan = Convert.ToInt32(autoTotalPlan); per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Затраты на транспорт", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(combustibleTotal); plan = Convert.ToInt32(autosPlans.Сombustible); per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Затраты на топливо", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(tiresAccumulatorsTotal + sparesTotal); plan = Convert.ToInt32(autosPlans.Tires + autosPlans.Accumulators + autosPlans.Spares); per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Запчасти для ремонтов авто", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(tiresAccumulatorsTotal); plan = Convert.ToInt32(autosPlans.Tires + autosPlans.Accumulators); per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Запасные части - шины и аккумуляторы", Plan = plan, Fact = fact, Percent = per, Link = true });

            fact = Convert.ToInt32(sparesTotal); plan = Convert.ToInt32(autosPlans.Spares); per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Запасные части - другие комплектующие авто", Plan = plan, Fact = fact, Percent = per, Link = true });


            fact = Convert.ToInt32(maintenanceTotal + servicesTotal) + Convert.ToInt32(carWash ?? 0);
            plan = Convert.ToInt32(autosPlans.MaintenanceCar + autosPlans.MaintenanceTruck + autosPlans.Services + (autosPlans.СarWash ?? 0));
            per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Обслуживание автомобилей", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(maintenanceTotal); plan = Convert.ToInt32(autosPlans.MaintenanceCar + autosPlans.MaintenanceTruck); per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "ТО", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(maintenanceTruck); plan = Convert.ToInt32(autosPlans.MaintenanceTruck); per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "ТО грузового транспорта", Plan = plan, Fact = fact, Percent = per, Link = true });

            fact = Convert.ToInt32(maintenanceCar); plan = Convert.ToInt32(autosPlans.MaintenanceCar); per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "ТО легкового транспорта", Plan = plan, Fact = fact, Percent = per, Link = true });

            fact = Convert.ToInt32(servicesTotal); plan = Convert.ToInt32(autosPlans.Services); per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Услуги по ремонтам", Plan = plan, Fact = fact, Percent = per, Link = true });

            fact = Convert.ToInt32(carWash ?? 0); plan = Convert.ToInt32(autosPlans.СarWash ?? 0);
            if (plan > 0)
                per = (double)(fact - plan) / plan * 100;
            else
                per = 0;
            kpiAuto.Rows.Add(new KPIRow { Name = "Мойка", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(leaseTruck ?? 0); plan = Convert.ToInt32(autosPlans.LeaseTruck ?? 0);
            //if (plan > 0)
            per = (double)(fact - plan) / plan * 100;
            //else
            //    per = 0;
            kpiAuto.Rows.Add(new KPIRow { Name = "Аренда грузового транспорта", Plan = plan, Fact = fact, Percent = per });


        }


        public void FillKPIPlansAutos(int filialId, short month, short year, short monthEnd, short yearEnd, KPIAutoPartRow kpiAuto, double? carWash, double? leaseTruck)
        {
            var departmentId = 0;
            if (filialId == 2)
                departmentId = 10;
            if (filialId == 1)
                departmentId = 7;

            short yearPast = (short)(year - 1);
            short yearEndPast = (short)(yearEnd - 1);

            DateTime dateStart = new DateTime(yearPast, month, 1);
            DateTime dateEnd = new DateTime(yearEndPast, monthEnd, 1);
            //dateStart = dateStart.AddMonths(-1);
            //dateEnd = dateEnd.AddMonths(-1);

            var autos = _userCommands.GetAutosByFilial(_ade, filialId, (short)dateStart.Month, (short)dateStart.Year, (short)dateEnd.Month, (short)dateEnd.Year, departmentId);

            var autosPlansInit = _userCommands.GetAutosPlansByFilial(_ade, filialId, month, year, monthEnd, yearEnd, departmentId);
            if (autosPlansInit == null || autosPlansInit.Count == 0)
                return;
            var autosPlans = autosPlansInit
               .GroupBy(n => n.FilialId)
               .Select(n => new
               {
                   FilialId = n.Key,
                   Car = n.Sum(c => c.Car),
                   Truck = n.Sum(c => c.Truck),
                   Сombustible = n.Sum(c => c.Сombustible),
                   Tires = n.Sum(c => c.Tires),
                   Accumulators = n.Sum(c => c.Accumulators),
                   MaintenanceCar = n.Sum(c => c.MaintenanceCar),
                   MaintenanceTruck = n.Sum(c => c.MaintenanceTruck),
                   Spares = n.Sum(c => c.Spares),
                   Services = n.Sum(c => c.Services),
                   СarWash = n.Sum(c => c.СarWash),
                   LeaseTruck = n.Sum(c => c.LeaseTruck)
               })
               .First();

            var truck = autos.Where(p => p.Type == AutoType.Truck).ToList();
            var car = autos.Where(p => p.Type == AutoType.Car).ToList();
            var distrCar = autos.Where(p => p.Type == AutoType.Distrib).ToList();
            var admin = autos.Where(p => p.Type == AutoType.Admin).ToList();
            var none = autos.Where(p => p.Type == AutoType.None).ToList();

            //var months = GetNumberMonth(month, year, monthEnd, yearEnd);
            //var distrCarPlan = 31;

            var fact = autosPlans.Car + autosPlans.Truck;
            //if (filialId == 3)
            //    fact += 4 * months;
            var plan = autos.Count();
            //if (filialId == 1)
            //    plan += distrCarPlan * months;

            double per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Кол-во авто", Plan = plan, Fact = fact, Percent = per });

            fact = autosPlans.Truck;
            //if (filialId == 3)
            //    fact += 4;
            plan = truck.Count(); per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Грузовой транспорт", Plan = plan, Fact = fact, Percent = per, Link = false });

            fact = autosPlans.Car; plan = car.Count(); per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Легковой транспорт", Plan = plan, Fact = fact, Percent = per, Link = false });

            //if (filialId == 1)
            //{
            //    fact = distrCar.Count(); plan = distrCarPlan; per = (double)(fact - plan) / plan * 100;
            //    kpiAuto.Rows.Add(new KPIRow { Name = "Дистрибьюторский транспорт", Plan = plan, Fact = fact, Percent = per, Link = true });
            //}

            kpiAuto.Rows.Add(new KPIRow { Name = "Админ транспорт", Plan = admin.Count(), Fact = 0, Percent = -100 });
            kpiAuto.Rows.Add(new KPIRow { Name = "Не распределенный транспорт(кары)", Plan = none.Count(), Fact = 0, Percent = -100 });


            var combustibleTotal = autos.Sum(p => p.Liters * p.СombustiblePrice);
            var tiresAccumulatorsTotal = (autos.Sum(p => p.Tires) + autos.Sum(p => p.Accumulators)) * 1.2;
            var sparesTotal = autos.Sum(p => p.Spares) * 1.2;
            var servicesTotal = (autos.Sum(p => p.Repairs) + autos.Sum(p => p.Services)) * 1.2;

            var maintenanceCar = (car.Sum(p => p.Expenses) + car.Sum(p => p.Lubricants) + car.Sum(p => p.TestareAuto)) * 1.2;
            var maintenanceTruck = (truck.Sum(p => p.Expenses) + truck.Sum(p => p.Lubricants) + truck.Sum(p => p.TestareAuto)) * 1.2;
            var maintenanceTotal = maintenanceCar + maintenanceTruck;

            var autoTotal = combustibleTotal + tiresAccumulatorsTotal + sparesTotal + maintenanceTotal + servicesTotal + (carWash ?? 0) + (leaseTruck ?? 0);
            var autoTotalPlan = autosPlans.Сombustible + autosPlans.Tires + autosPlans.Accumulators + autosPlans.Spares
                                + autosPlans.MaintenanceCar + autosPlans.MaintenanceTruck + autosPlans.Services
                                + (autosPlans.СarWash ?? 0) + (autosPlans.LeaseTruck ?? 0);


            fact = Convert.ToInt32(autoTotalPlan); plan = Convert.ToInt32(autoTotal); per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Затраты на транспорт", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(autosPlans.Сombustible); plan = Convert.ToInt32(combustibleTotal); per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Затраты на топливо", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(autosPlans.Tires + autosPlans.Accumulators + autosPlans.Spares);
            plan = Convert.ToInt32(tiresAccumulatorsTotal + sparesTotal);
            per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Запчасти для ремонтов авто", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(autosPlans.Tires + autosPlans.Accumulators); plan = Convert.ToInt32(tiresAccumulatorsTotal); per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Запасные части - шины и аккумуляторы", Plan = plan, Fact = fact, Percent = per, Link = false });

            fact = Convert.ToInt32(autosPlans.Spares); plan = Convert.ToInt32(sparesTotal); per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Запасные части - другие комплектующие авто", Plan = plan, Fact = fact, Percent = per, Link = false });


            fact = Convert.ToInt32(autosPlans.MaintenanceCar + autosPlans.MaintenanceTruck + autosPlans.Services + (autosPlans.СarWash ?? 0));
            plan = Convert.ToInt32(maintenanceTotal + servicesTotal) + Convert.ToInt32(carWash ?? 0);
            per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Обслуживание автомобилей", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(autosPlans.MaintenanceCar + autosPlans.MaintenanceTruck);
            plan = Convert.ToInt32(maintenanceTotal);
            per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "ТО", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(autosPlans.MaintenanceTruck);
            plan = Convert.ToInt32(maintenanceTruck);
            per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "ТО грузового транспорта", Plan = plan, Fact = fact, Percent = per, Link = false });

            fact = Convert.ToInt32(autosPlans.MaintenanceCar);
            plan = Convert.ToInt32(maintenanceCar);
            per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "ТО легкового транспорта", Plan = plan, Fact = fact, Percent = per, Link = false });

            fact = Convert.ToInt32(autosPlans.Services);
            plan = Convert.ToInt32(servicesTotal);
            per = (double)(fact - plan) / plan * 100;
            kpiAuto.Rows.Add(new KPIRow { Name = "Услуги по ремонтам", Plan = plan, Fact = fact, Percent = per, Link = false });

            fact = Convert.ToInt32(autosPlans.СarWash ?? 0);
            plan = Convert.ToInt32(carWash ?? 0);
            if (plan > 0)
                per = (double)(fact - plan) / plan * 100;
            else
                per = 0;
            kpiAuto.Rows.Add(new KPIRow { Name = "Мойка", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(autosPlans.LeaseTruck ?? 0);
            plan = Convert.ToInt32(leaseTruck ?? 0);
            //if (plan > 0)
            per = (double)(fact - plan) / plan * 100;
            //else
            //    per = 0;
            kpiAuto.Rows.Add(new KPIRow { Name = "Аренда грузового транспорта", Plan = plan, Fact = fact, Percent = per });


        }


        public int GetFactKPIAutosByKey(KPIAutoPartRow kpiAuto, string key)
        {
            var keyPosition = kpiAuto.Rows.Find(p => p.Name == key);
            return keyPosition != null ? keyPosition.Fact : 0;
        }

        public int GetFactKPISalariesByKey(KPISalaryPartRow kpiSalaries, string key)
        {
            var keyPosition = kpiSalaries.Rows.Find(p => p.Name == key);
            return keyPosition != null ? keyPosition.Fact : 0;
        }

        public int GetFactKPIAminsByKey(KPIAdminPartRow kpiAdmins, string key)
        {
            var keyPosition = kpiAdmins.Rows.Find(p => p.Name == key);
            return keyPosition != null ? keyPosition.Fact : 0;
        }

        public void FillKPIAutosPast(int filialId, short month, short year, short monthEnd, short yearEnd, KPIAutoPartRow kpiAuto, KPIAutoPartRow kpiAutoPast, double? carWash, double? leaseTruck)
        {
            var departmentId = 0;
            if (filialId == 2)
                departmentId = 10;
            if (filialId == 1)
                departmentId = 7;

            var autos = _userCommands.GetAutosByFilial(_ade, filialId, month, year, monthEnd, yearEnd, departmentId);

            var truck = autos.Where(p => p.Type == AutoType.Truck).ToList();
            var car = autos.Where(p => p.Type == AutoType.Car).ToList();
            var distrCar = autos.Where(p => p.Type == AutoType.Distrib).ToList();
            var admin = autos.Where(p => p.Type == AutoType.Admin).ToList();
            var none = autos.Where(p => p.Type == AutoType.None).ToList();

            var months = GetNumberMonth(month, year, monthEnd, yearEnd);
            var distrCarPlan = 31;
            //////////////////////////////
            var fact = autos.Count();
            //if (filialId == 3)
            //    fact += 4 * months;
            var plan = GetFactKPIAutosByKey(kpiAuto, "Кол-во авто");
            if (filialId == 1)
                plan += distrCarPlan * months;
            double per = (double)(plan - fact) / fact * 100;
            kpiAutoPast.Rows.Add(new KPIRow { Name = "Кол-во авто", Plan = plan, Fact = fact, Percent = per });
            //////////////////////////////
            fact = truck.Count();
            //if (filialId == 3)
            //    fact += 4;
            plan = GetFactKPIAutosByKey(kpiAuto, "Грузовой транспорт");
            per = (double)(plan - fact) / fact * 100;
            kpiAutoPast.Rows.Add(new KPIRow { Name = "Грузовой транспорт", Plan = plan, Fact = fact, Percent = per, Link = true });
            /////////////////////////////
            fact = car.Count();
            plan = GetFactKPIAutosByKey(kpiAuto, "Легковой транспорт");
            per = (double)(plan - fact) / fact * 100;
            kpiAutoPast.Rows.Add(new KPIRow { Name = "Легковой транспорт", Plan = plan, Fact = fact, Percent = per, Link = true });

            if (filialId == 1)
            {
                fact = distrCar.Count(); plan = distrCarPlan; per = (double)(plan - fact) / fact * 100;
                kpiAutoPast.Rows.Add(new KPIRow { Name = "Дистрибьюторский транспорт", Plan = plan, Fact = fact, Percent = per, Link = true });
            }

            kpiAutoPast.Rows.Add(new KPIRow { Name = "Админ транспорт", Plan = 0, Fact = admin.Count(), Percent = 0 });
            kpiAutoPast.Rows.Add(new KPIRow { Name = "Не распределенный транспорт(кары)", Plan = 0, Fact = none.Count(), Percent = 0 });
            /////////////////////////////

            var combustibleTotal = autos.Sum(p => p.Liters * p.СombustiblePrice);
            var tiresAccumulatorsTotal = (autos.Sum(p => p.Tires) + autos.Sum(p => p.Accumulators)) * 1.2;
            var sparesTotal = autos.Sum(p => p.Spares) * 1.2;
            var servicesTotal = (autos.Sum(p => p.Repairs) + autos.Sum(p => p.Services)) * 1.2;

            var maintenanceCar = (car.Sum(p => p.Expenses) + car.Sum(p => p.Lubricants) + car.Sum(p => p.TestareAuto)) * 1.2;
            var maintenanceTruck = (truck.Sum(p => p.Expenses) + truck.Sum(p => p.Lubricants) + truck.Sum(p => p.TestareAuto)) * 1.2;
            var maintenanceTotal = maintenanceCar + maintenanceTruck;

            var autoTotal = combustibleTotal + tiresAccumulatorsTotal + sparesTotal + maintenanceTotal
                + servicesTotal + (carWash ?? 0) + (leaseTruck ?? 0);
            /////////////////////////////
            var autoTotalPlan = GetFactKPIAutosByKey(kpiAuto, "Затраты на транспорт");
            var autosPlansСombustible = GetFactKPIAutosByKey(kpiAuto, "Затраты на топливо");
            var autosPlansTiresAccumulatorsSpares = GetFactKPIAutosByKey(kpiAuto, "Запчасти для ремонтов авто");
            var autosPlansTiresAccumulators = GetFactKPIAutosByKey(kpiAuto, "Запасные части - шины и аккумуляторы");
            var autosPlansSpares = GetFactKPIAutosByKey(kpiAuto, "Запасные части - другие комплектующие авто");
            var autosPlansMaintenanceServices = GetFactKPIAutosByKey(kpiAuto, "Обслуживание автомобилей");
            var autosPlansMaintenance = GetFactKPIAutosByKey(kpiAuto, "ТО");
            var autosPlansMaintenanceTruck = GetFactKPIAutosByKey(kpiAuto, "ТО грузового транспорта");
            var autosPlansMaintenanceCar = GetFactKPIAutosByKey(kpiAuto, "ТО легкового транспорта");
            var autosPlansServices = GetFactKPIAutosByKey(kpiAuto, "Услуги по ремонтам");
            var autosPlansСarWash = GetFactKPIAutosByKey(kpiAuto, "Мойка");
            var autosPlansLeaseTruck = GetFactKPIAutosByKey(kpiAuto, "Аренда грузового транспорта");
            /////////////////////////////
            //var autoTotalPlan = autosPlans.Сombustible + autosPlans.Tires + autosPlans.Accumulators + autosPlans.Spares
            //                    + autosPlans.MaintenanceCar + autosPlans.MaintenanceTruck + autosPlans.Services
            //                    + (autosPlans.СarWash ?? 0) + (autosPlans.LeaseTruck ?? 0);

            /////////////////////////////
            fact = Convert.ToInt32(autoTotal); plan = autoTotalPlan; per = (double)(plan - fact) / fact * 100;
            kpiAutoPast.Rows.Add(new KPIRow { Name = "Затраты на транспорт", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(combustibleTotal); plan = autosPlansСombustible; per = (double)(plan - fact) / fact * 100;
            kpiAutoPast.Rows.Add(new KPIRow { Name = "Затраты на топливо", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(tiresAccumulatorsTotal + sparesTotal); plan = autosPlansTiresAccumulatorsSpares; per = (double)(plan - fact) / fact * 100;
            kpiAutoPast.Rows.Add(new KPIRow { Name = "Запчасти для ремонтов авто", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(tiresAccumulatorsTotal); plan = autosPlansTiresAccumulators; per = (double)(plan - fact) / fact * 100;
            kpiAutoPast.Rows.Add(new KPIRow { Name = "Запасные части - шины и аккумуляторы", Plan = plan, Fact = fact, Percent = per, Link = true });

            fact = Convert.ToInt32(sparesTotal); plan = autosPlansSpares; per = (double)(plan - fact) / fact * 100;
            kpiAutoPast.Rows.Add(new KPIRow { Name = "Запасные части - другие комплектующие авто", Plan = plan, Fact = fact, Percent = per, Link = true });


            fact = Convert.ToInt32(maintenanceTotal + servicesTotal) + Convert.ToInt32(carWash ?? 0);
            plan = autosPlansMaintenanceServices;
            per = (double)(plan - fact) / fact * 100;
            kpiAutoPast.Rows.Add(new KPIRow { Name = "Обслуживание автомобилей", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(maintenanceTotal); plan = autosPlansMaintenance; per = (double)(plan - fact) / fact * 100;
            kpiAutoPast.Rows.Add(new KPIRow { Name = "ТО", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(maintenanceTruck); plan = autosPlansMaintenanceTruck; per = (double)(plan - fact) / fact * 100;
            kpiAutoPast.Rows.Add(new KPIRow { Name = "ТО грузового транспорта", Plan = plan, Fact = fact, Percent = per, Link = true });

            fact = Convert.ToInt32(maintenanceCar); plan = autosPlansMaintenanceCar; per = (double)(plan - fact) / fact * 100;
            kpiAutoPast.Rows.Add(new KPIRow { Name = "ТО легкового транспорта", Plan = plan, Fact = fact, Percent = per, Link = true });

            fact = Convert.ToInt32(servicesTotal); plan = autosPlansServices; per = (double)(plan - fact) / fact * 100;
            kpiAutoPast.Rows.Add(new KPIRow { Name = "Услуги по ремонтам", Plan = plan, Fact = fact, Percent = per, Link = true });

            fact = Convert.ToInt32(carWash ?? 0); plan = autosPlansСarWash;
            if (plan > 0)
                per = (double)(plan - fact) / fact * 100;
            else
                per = 0;
            kpiAutoPast.Rows.Add(new KPIRow { Name = "Мойка", Plan = plan, Fact = fact, Percent = per });

            fact = Convert.ToInt32(leaseTruck ?? 0); plan = autosPlansLeaseTruck;
            if (plan > 0)
                per = (double)(plan - fact) / fact * 100;
            else
                per = 0;
            kpiAutoPast.Rows.Add(new KPIRow { Name = "Аренда грузового транспорта", Plan = plan, Fact = fact, Percent = per });

        }

        public void FillKPISalaries(int filialId, short month, short year, short monthEnd, short yearEnd, KPISalaryPartRow kpiSalaryPart,
            double salaryTax, out List<Salary> forPast)
        {
            var departmentId = 0;
            if (filialId == 2)
                departmentId = 10;
            if (filialId == 1)
                departmentId = 7;


            var salaries = _userCommands.GetSalariesByFilial(_ade, filialId, month, year, monthEnd, yearEnd, departmentId);

            forPast = salaries;
            //if (salaries.Count == 0)
            //    return;
            var postsFact = salaries
            .GroupBy(n => n.Post.Trim())
            .Select(n => new Post
            { Name = n.Key, Count = n.Count() })
            .OrderBy(n => n.Name).ToList();


            var salariesPlans = _userCommands.GetSalariesPlansByFilial(_ade, filialId, month, year, monthEnd, yearEnd, departmentId);
            //if (salariesPlans.Count == 0)
            //    return;
            var postsPlan = salariesPlans
                .GroupBy(n => n.Post)
                .Select(n => new Post
                { Name = n.Key, Count = n.Sum(c => c.Amount) })
                .OrderBy(n => n.Name).ToList();

            var plan = postsPlan.Sum(p => p.Count);
            var fact = salaries.Count;
            var per = (double)(fact - plan) / plan * 100;
            kpiSalaryPart.Rows.Add(new KPIRow { Name = "Количество сотрудников", Plan = plan, Fact = fact, Percent = per });


            List<KPIRow> rows = new List<KPIRow>();
            foreach (var post in postsFact)
            {
                rows.Add(new KPIRow
                {
                    Name = post.Name,
                    Fact = post.Count,
                    Link = true
                });
            }


            foreach (var post in postsPlan)
            {
                var existingPost = rows.FirstOrDefault(x => x.Name.Trim() == post.Name.Trim());
                if (existingPost != null)
                {
                    existingPost.Plan = post.Count;
                    existingPost.Percent = (double)(existingPost.Fact - post.Count) / post.Count * 100;
                }
                else
                {
                    rows.Add(new KPIRow
                    {
                        Name = post.Name,
                        Plan = post.Count,
                        Percent = (double)(0 - post.Count) / post.Count * 100
                    });
                }
            }

            kpiSalaryPart.Rows.AddRange(rows);

            var fics = salaries.Sum(c => c.Card);
            var bon = salaries.Sum(c => c.Cash);
            var tax = salaryTax;
            var vac = salaries.Sum(c => c.Vacation);
            ///var sumSal = fics + bon + tax + vac;

            ////////////////////
            var ded = salaries.Sum(c => c.Deduction);
            if (ded.HasValue)
                fics += ded.Value;

            var health = salaries.Sum(c => c.HealthInsurance ?? 0);
            var income = salaries.Sum(c => c.IncomeTax ?? 0);
            var pension = salaries.Sum(c => c.PensionFund ?? 0);
            var social = salaries.Sum(c => c.SocialFund ?? 0);

            if(health > 0 && income > 0 && pension > 0 && social > 0)
                tax += Math.Round(pension + health * 2 + income + social, 2);
            ///////////////

            var sumSal = fics + bon + tax + vac;

            var salaryCommonPlans = _userCommands.GetSalaryCommonPlansByFilial(_ade, filialId, month, year, monthEnd, yearEnd, departmentId);

            var salaryCommonPlansGroup = salaryCommonPlans
                .GroupBy(n => n.FilialId)
                .Select(n => new
                { FilialId = n.Key, Card = n.Sum(c => c.Card), Cash = n.Sum(c => c.Cash), Vacation = n.Sum(c => c.Vacation), Tax = n.Sum(c => c.Tax) })
                .FirstOrDefault();

            if (salaryCommonPlansGroup == null)
            {
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Затраты на персонал", Fact = Convert.ToInt32(sumSal) });
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Фиксированная часть", Fact = Convert.ToInt32(fics) });
                if (bon > 0)
                    kpiSalaryPart.Rows.Add(new KPIRow { Name = "Бонусная часть", Fact = Convert.ToInt32(bon) });
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Налоги по зарплате", Fact = Convert.ToInt32(tax) });
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Отпускные", Fact = Convert.ToInt32(vac) });
            }
            else
            {
                var sumSalPlan = salaryCommonPlansGroup.Card
                                + (salaryCommonPlansGroup.Cash ?? 0)
                                + (salaryCommonPlansGroup.Vacation ?? 0)
                                + (salaryCommonPlansGroup.Tax ?? 0);
                var percent = (sumSal - sumSalPlan) / sumSalPlan * 100;
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Затраты на персонал", Fact = Convert.ToInt32(sumSal), Plan = Convert.ToInt32(sumSalPlan), Percent = percent });

                var ficsPlan = salaryCommonPlansGroup.Card; percent = (fics - ficsPlan) / ficsPlan * 100;
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Фиксированная часть", Fact = Convert.ToInt32(fics), Plan = Convert.ToInt32(ficsPlan), Percent = percent });

                var bonPlan = salaryCommonPlansGroup.Cash ?? 0; percent = (bon - bonPlan) / bonPlan * 100;
                if (bon > 0 || bonPlan > 0)
                    kpiSalaryPart.Rows.Add(new KPIRow { Name = "Бонусная часть", Fact = Convert.ToInt32(bon), Plan = Convert.ToInt32(bonPlan), Percent = percent });

                var taxPlan = salaryCommonPlansGroup.Tax ?? 0; percent = (tax - taxPlan) / taxPlan * 100;
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Налоги по зарплате", Fact = Convert.ToInt32(tax), Plan = Convert.ToInt32(taxPlan), Percent = percent });

                var vacPlan = salaryCommonPlansGroup.Vacation ?? 0; percent = (vac - vacPlan) / vacPlan * 100;
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Отпускные", Fact = Convert.ToInt32(vac), Plan = Convert.ToInt32(vacPlan), Percent = percent });
            }

        }


        public void FillKPIPlansSalaries(int filialId, short month, short year, short monthEnd, short yearEnd, KPISalaryPartRow kpiSalaryPart,
           double salaryTax, bool corr, out List<Salary> forPast)
        {
            var departmentId = 0;
            if (filialId == 2)
                departmentId = 10;
            if (filialId == 1)
                departmentId = 7;

            short yearPast = (short)(year - 1);
            short yearEndPast = (short)(yearEnd - 1);

            DateTime dateStart = new DateTime(yearPast, month, 1);
            DateTime dateEnd = new DateTime(yearEndPast, monthEnd, 1);
            //dateStart = dateStart.AddMonths(-1);
            //dateEnd = dateEnd.AddMonths(-1);

            var salaries = _userCommands.GetSalariesByFilial(_ade, filialId, (short)dateStart.Month, (short)dateStart.Year, (short)dateEnd.Month, (short)dateEnd.Year, departmentId);

            forPast = salaries;
            //if (salaries.Count == 0)
            //    return;
            var postsPlan = salaries
            .GroupBy(n => n.Post.Trim())
            .Select(n => new Post
            { Name = n.Key, Count = n.Count() })
            .OrderBy(n => n.Name).ToList();


            var salariesPlans = _userCommands.GetSalariesPlansByFilial(_ade, filialId, month, year, monthEnd, yearEnd, departmentId);
            //if (salariesPlans.Count == 0)
            //    return;
            var postsFact = salariesPlans
                .GroupBy(n => n.Post)
                .Select(n => new Post
                { Name = n.Key, Count = n.Sum(c => c.Amount) })
                .OrderBy(n => n.Name).ToList();

            var fact = postsFact.Sum(p => p.Count);
            var plan = salaries.Count;
            var per = (double)(fact - plan) / plan * 100;
            kpiSalaryPart.Rows.Add(new KPIRow { Name = "Количество сотрудников", Plan = plan, Fact = fact, Percent = per });


            List<KPIRow> rows = new List<KPIRow>();
            foreach (var post in postsFact)
            {
                rows.Add(new KPIRow
                {
                    Name = post.Name,
                    Fact = post.Count,
                    Link = false
                });
            }


            foreach (var post in postsPlan)
            {
                var existingPost = rows.FirstOrDefault(x => x.Name == post.Name);
                if (existingPost != null)
                {
                    existingPost.Plan = post.Count;
                    existingPost.Percent = (double)(existingPost.Fact - post.Count) / post.Count * 100;
                }
                else
                {
                    rows.Add(new KPIRow
                    {
                        Name = post.Name,
                        Plan = post.Count,
                        Percent = (double)(0 - post.Count) / post.Count * 100
                    });
                }
            }

            kpiSalaryPart.Rows.AddRange(rows);

            var ficsPlan = salaries.Sum(c => c.Card);
            var bonPlan = salaries.Sum(c => c.Cash);
            var vacPlan = salaries.Sum(c => c.Vacation);
            var taxPlan = salaryTax;
            if (corr)
                taxPlan = (ficsPlan + vacPlan) * 0.51;
            var sumSalPlan = ficsPlan + bonPlan + taxPlan + vacPlan;

            var salaryCommonPlans = _userCommands.GetSalaryCommonPlansByFilial(_ade, filialId, month, year, monthEnd, yearEnd, departmentId);

            var salaryCommonPlansGroup = salaryCommonPlans
                .GroupBy(n => n.FilialId)
                .Select(n => new
                { FilialId = n.Key, Card = n.Sum(c => c.Card), Cash = n.Sum(c => c.Cash), Vacation = n.Sum(c => c.Vacation), Tax = n.Sum(c => c.Tax) })
                .FirstOrDefault();

            if (salaryCommonPlansGroup == null)
            {
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Затраты на персонал", Plan = Convert.ToInt32(sumSalPlan) });
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Фиксированная часть", Plan = Convert.ToInt32(ficsPlan) });
                if (bonPlan > 0)
                    kpiSalaryPart.Rows.Add(new KPIRow { Name = "Бонусная часть", Plan = Convert.ToInt32(bonPlan) });
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Налоги по зарплате", Plan = Convert.ToInt32(taxPlan) });
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Отпускные", Plan = Convert.ToInt32(vacPlan) });
            }
            else
            {
                var sumSal = salaryCommonPlansGroup.Card
                                + (salaryCommonPlansGroup.Cash ?? 0)
                                + (salaryCommonPlansGroup.Vacation ?? 0)
                                + (salaryCommonPlansGroup.Tax ?? 0);
                var percent = (sumSal - sumSalPlan) / sumSalPlan * 100;
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Затраты на персонал", Fact = Convert.ToInt32(sumSal), Plan = Convert.ToInt32(sumSalPlan), Percent = percent });

                var fics = salaryCommonPlansGroup.Card; percent = (fics - ficsPlan) / ficsPlan * 100;
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Фиксированная часть", Fact = Convert.ToInt32(fics), Plan = Convert.ToInt32(ficsPlan), Percent = percent });

                var bon = salaryCommonPlansGroup.Cash ?? 0; percent = (bon - bonPlan) / bonPlan * 100;
                if (bon > 0 || bonPlan > 0)
                    kpiSalaryPart.Rows.Add(new KPIRow { Name = "Бонусная часть", Fact = Convert.ToInt32(bon), Plan = Convert.ToInt32(bonPlan), Percent = percent });

                var tax = salaryCommonPlansGroup.Tax ?? 0; percent = (tax - taxPlan) / taxPlan * 100;
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Налоги по зарплате", Fact = Convert.ToInt32(tax), Plan = Convert.ToInt32(taxPlan), Percent = percent });

                var vac = salaryCommonPlansGroup.Vacation ?? 0; percent = (vac - vacPlan) / vacPlan * 100;
                kpiSalaryPart.Rows.Add(new KPIRow { Name = "Отпускные", Fact = Convert.ToInt32(vac), Plan = Convert.ToInt32(vacPlan), Percent = percent });
            }

        }

        public void FillKPISalariesPast(int filialId, short month, short year, short monthEnd, short yearEnd, KPISalaryPartRow kpiSalaryPart,
            KPISalaryPartRow kpiSalaryPartPast, double salaryTax, List<Salary> curSal)
        {
            var departmentId = 0;
            if (filialId == 2)
                departmentId = 10;
            if (filialId == 1)
                departmentId = 7;


            var salaries = _userCommands.GetSalariesByFilial(_ade, filialId, month, year, monthEnd, yearEnd, departmentId);

            var postsFact = salaries
            .GroupBy(n => n.Post.Trim())
            .Select(n => new Post
            { Name = n.Key, Count = n.Count() })
            .OrderBy(n => n.Name).ToList();

            var postsPlan = curSal
                .GroupBy(n => n.Post.Trim())
                .Select(n => new Post
                { Name = n.Key, Count = n.Count() })
                .OrderBy(n => n.Name).ToList();

            var plan = postsPlan.Sum(p => p.Count);
            var fact = salaries.Count;
            var per = (double)(plan - fact) / fact * 100;
            kpiSalaryPartPast.Rows.Add(new KPIRow { Name = "Количество сотрудников", Plan = plan, Fact = fact, Percent = per });


            List<KPIRow> rows = new List<KPIRow>();
            foreach (var post in postsPlan)
            {
                rows.Add(new KPIRow
                {
                    Name = post.Name,
                    //Fact = post.Count,
                    Plan = post.Count,
                    Percent = (double)(post.Count - 0) / 0 * 100,
                });
            }


            foreach (var post in postsFact)
            {
                var existingPost = rows.FirstOrDefault(x => x.Name.Trim() == post.Name.Trim());
                if (existingPost != null)
                {
                    //existingPost.Plan = post.Count;
                    existingPost.Fact = post.Count;
                    existingPost.Percent = (double)(existingPost.Plan - existingPost.Fact) / existingPost.Fact * 100;
                    existingPost.Link = true;
                }
                else
                {
                    rows.Add(new KPIRow
                    {
                        Name = post.Name,
                        //Plan = post.Count,
                        Fact = post.Count,
                        Percent = (double)(0 - post.Count) / post.Count * 100,
                        Link = true
                    });
                }
            }

            kpiSalaryPartPast.Rows.AddRange(rows);

            var fics = salaries.Sum(c => c.Card);
            var bon = salaries.Sum(c => c.Cash);
            var tax = salaryTax;
            var vac = salaries.Sum(c => c.Vacation);
            var sumSal = fics + bon + tax + vac;

            var salaryCommonPlansGroupCard = GetFactKPISalariesByKey(kpiSalaryPart, "Фиксированная часть");
            var salaryCommonPlansGroupCash = GetFactKPISalariesByKey(kpiSalaryPart, "Бонусная часть");

            var salaryCommonPlansGroupTax = GetFactKPISalariesByKey(kpiSalaryPart, "Налоги по зарплате");
            var salaryCommonPlansGroupVacation = GetFactKPISalariesByKey(kpiSalaryPart, "Отпускные");

            var sumSalPlan = salaryCommonPlansGroupCard
                            + salaryCommonPlansGroupCash
                            + salaryCommonPlansGroupVacation
                            + salaryCommonPlansGroupTax;
            var percent = (sumSalPlan - sumSal) / sumSal * 100;
            kpiSalaryPartPast.Rows.Add(new KPIRow { Name = "Затраты на персонал", Fact = Convert.ToInt32(sumSal), Plan = Convert.ToInt32(sumSalPlan), Percent = percent });

            var ficsPlan = salaryCommonPlansGroupCard;
            percent = (ficsPlan - fics) / fics * 100;
            kpiSalaryPartPast.Rows.Add(new KPIRow { Name = "Фиксированная часть", Fact = Convert.ToInt32(fics), Plan = Convert.ToInt32(ficsPlan), Percent = percent });

            var bonPlan = salaryCommonPlansGroupCash;
            percent = (bonPlan - bon) / bon * 100;
            if (bon > 0 || bonPlan > 0)
                kpiSalaryPartPast.Rows.Add(new KPIRow { Name = "Бонусная часть", Fact = Convert.ToInt32(bon), Plan = Convert.ToInt32(bonPlan), Percent = percent });

            var taxPlan = salaryCommonPlansGroupTax;
            percent = (taxPlan - tax) / tax * 100;
            kpiSalaryPartPast.Rows.Add(new KPIRow { Name = "Налоги по зарплате", Fact = Convert.ToInt32(tax), Plan = Convert.ToInt32(taxPlan), Percent = percent });

            var vacPlan = salaryCommonPlansGroupVacation;
            percent = (vacPlan - vac) / vac * 100;
            kpiSalaryPartPast.Rows.Add(new KPIRow { Name = "Отпускные", Fact = Convert.ToInt32(vac), Plan = Convert.ToInt32(vacPlan), Percent = percent });

            //if (kpiSalaryPartPast.Rows.Count > kpiSalaryPart.Rows.Count)
            //{
            int i = 0;
            foreach (var row in kpiSalaryPartPast.Rows)
            {
                if (!kpiSalaryPart.Rows.Exists(p => p.Name.Trim() == row.Name.Trim()))
                {
                    kpiSalaryPart.Rows.Insert(i, new KPIRow
                    {
                        Name = row.Name,
                        Plan = 0,
                        Fact = 0,
                        Percent = 0
                    });
                }
                i++;
            }
            //}

            //if (kpiSalaryPart.Rows.Count > kpiSalaryPartPast.Rows.Count)
            //{
            i = 0;
            foreach (var row in kpiSalaryPart.Rows)
            {
                if (!kpiSalaryPartPast.Rows.Exists(p => p.Name.Trim() == row.Name.Trim()))
                {
                    kpiSalaryPartPast.Rows.Insert(i, new KPIRow
                    {
                        Name = row.Name,
                        Plan = 0,
                        Fact = 0,
                        Percent = 0
                    });
                }
                i++;
            }
            // }

            kpiSalaryPart.Rows.OrderBy(p => p.Name);

        }

        public void FillKPIAdmins(int filialId, short month, short year, short monthEnd, short yearEnd,
            KPIAdminPartRow kpiAdminPart, KPIRow transportTotal, KPIRow personalTotal, double mobileTelephony, double premisesRent
            /*, double? primeCost, double? discount, double? discountPepsi*/)
        {
            var defectDiscounInit = _userCommands.GetPublicDefectDiscountByFilial(_ade, filialId, month, year, monthEnd, yearEnd);
            var defectDiscoun = defectDiscounInit
              .GroupBy(n => n.FilialId)
              .Select(n => new
              {
                  FilialId = n.Key,
                  Defect = n.Sum(c => c.Defect),
                  DefectAdditional = n.Sum(c => c.DefectAdditional),
                  Discount = n.Sum(c => c.Discount),
                  DiscountAdditional = n.Sum(c => c.DiscountAdditional),
                  EventDistribution = n.Sum(c => c.EventDistribution),
                  PrimeCost = n.Sum(c => c.PrimeCost),
                  Representation = n.Sum(c => c.Representation)
              })
              .FirstOrDefault();

            var defectDiscountPlanInit = _userCommands.GetPublicDefectDiscountPlanByFilial(_ade, filialId, month, year, monthEnd, yearEnd);
            var defectDiscountPlan = defectDiscountPlanInit
            .GroupBy(n => n.FilialId)
            .Select(n => new
            {
                FilialId = n.Key,
                Defect = n.Sum(c => c.Defect),
                DefectAdditional = n.Sum(c => c.DefectAdditional),
                Discount = n.Sum(c => c.Discount),
                DiscountAdditional = n.Sum(c => c.DiscountAdditional),
                EventDistribution = n.Sum(c => c.EventDistribution),
                Representation = n.Sum(c => c.Representation)
            })
            .FirstOrDefault();

            //var netProfit = kpi.SalePart.Rows.Find(p => p.Name == "Объемы реализации");
            //double grossProfitFact = (double)(netProfit != null ? netProfit.Fact : 0);
            //double grossProfitPlan = (double)(netProfit != null ? netProfit.Plan : 0);

            double fact = 0.0; double plan = 0.0; double per = 0.0;
            var totalCurrentExpensesFact = 0.0; var totalCurrentExpensesPlan = 0.0;

            if (defectDiscountPlan != null)
            {
                //double discFact = 0.0;
                //if (discount.HasValue)
                //    discFact = discount.Value;// - (discountPepsi.HasValue ? discountPepsi.Value : 0);
                //else
                //    discFact = defectDiscoun != null ? (defectDiscoun.Discount ?? 0) : 0;

                //fact = (netProfit != null ? netProfit.Fact : 0) + discFact;//(defectDiscoun != null ? (defectDiscoun.Discount ?? 0) : 0) + (defectDiscoun != null ? (defectDiscoun.DiscountAdditional ?? 0):0);
                //plan = (netProfit != null ? netProfit.Plan : 0) + (defectDiscountPlan.Discount ?? 0) + (defectDiscountPlan.DiscountAdditional ?? 0);
                //per = (fact - plan) / plan * 100;
                //kpi.AdminPart.Rows.Add(new KPIRow { Name = "Валовый доход от продаж", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });


                //fact = discFact;
                //plan = defectDiscountPlan.Discount ?? 0; per = (fact - plan) / plan * 100;
                //kpi.AdminPart.Rows.Add(new KPIRow { Name = "Скидки", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });

                ////if (discountPepsi.HasValue)
                ////    fact = discountPepsi.Value;
                ////else
                ////    fact = defectDiscoun != null ? (defectDiscoun.DiscountAdditional ?? 0) : 0;
                ////plan = defectDiscountPlan.DiscountAdditional ?? 0; per = plan != 0 ? (fact - plan) / plan * 100 : 0;
                ////kpi.AdminPart.Rows.Add(new KPIRow { Name = "Скидки PepsiCo", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });

                //kpi.AdminPart.Rows.Add(new KPIRow { Name = "Чистый доход от продаж", Plan = netProfit.Plan, Fact = netProfit.Fact, Percent = netProfit.Percent });

                //if (primeCost.HasValue)
                //    fact = primeCost.Value * 1.2;
                //else
                //    fact = defectDiscoun != null ? (defectDiscoun.PrimeCost ?? 0) * 1.2 : 0;
                //plan = (netProfit.Fact == 0 || fact == 0) ? 0 : (double)netProfit.Plan / (double)netProfit.Fact * fact; per = plan == 0 ? 0 : (fact - plan) / plan * 100;
                //kpi.AdminPart.Rows.Add(new KPIRow { Name = "Себестоймость", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
                //grossProfitFact -= fact; grossProfitPlan -= plan;


                fact = defectDiscoun != null ? defectDiscoun.Defect : 0; plan = defectDiscountPlan.Defect; per = plan != 0 ? (fact - plan) / plan * 100 : 0;
                kpiAdminPart.Rows.Add(new KPIRow { Name = "Брак", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
                totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;
                //grossProfitFact -= fact; grossProfitPlan -= plan;

                fact = defectDiscoun != null ? defectDiscoun.DefectAdditional ?? 0 : 0; //fact /= 1.27; fact += fact * 0.1;//22.03.17
                plan = defectDiscountPlan.DefectAdditional ?? 0; per = plan != 0 ? (fact - plan) / plan * 100 : 0;
                kpiAdminPart.Rows.Add(new KPIRow { Name = "Брак Другое"/*"Брак PepsiCo"*/, Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
                totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;
                //grossProfitFact -= fact; grossProfitPlan -= plan;

                //fact = grossProfitFact; plan = grossProfitPlan; per = plan != 0 ? (fact - plan) / plan * 100 : 0;
                //kpi.AdminPart.Rows.Add(new KPIRow { Name = "Валовая прибыль", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });

                ////fact = defectDiscoun != null ? defectDiscoun.EventDistribution : 0; plan = defectDiscountPlan.EventDistribution; per = (fact - plan) / plan * 100;//per = plan != 0 ? (fact - plan) / plan * 100 : 0;
                ////kpi.AdminPart.Rows.Add(new KPIRow { Name = "Акционная раздача воды", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
                ////totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;

                fact = defectDiscoun != null ? defectDiscoun.Representation : 0; plan = defectDiscountPlan.Representation; per = plan != 0 ? (fact - plan) / plan * 100 : 0;
                kpiAdminPart.Rows.Add(new KPIRow { Name = "Расходы по филиалу", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
                totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;
            }

            var adminExpensePlanInit = _userCommands.GetPublicAdminExpensePlanPlanByFilial(_ade, filialId, month, year, monthEnd, yearEnd);
            var adminExpensePlan = adminExpensePlanInit
           .GroupBy(n => n.FilialId)
           .Select(n => new
           {
               FilialId = n.Key,
               MobileTelephony = n.Sum(c => c.MobileTelephony),
               PremisesRent = n.Sum(c => c.PremisesRent),
               TransportRent = n.Sum(c => c.TransportRent),
               WashTransport = n.Sum(c => c.WashTransport)
           })
           .FirstOrDefault();

            if (adminExpensePlan != null)
            {
                fact = mobileTelephony; plan = adminExpensePlan.MobileTelephony; per = plan != 0 ? (fact - plan) / plan * 100 : 0;
                kpiAdminPart.Rows.Add(new KPIRow { Name = "Затраты на мобильную тефонию", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
                totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;
            }

            var publicServices = _userCommands.GetPublicServicesByFilial(_ade, filialId, month, year, monthEnd, yearEnd);
            var publicServicesPlansInit = _userCommands.GetPublicServicesPlansByFilial(_ade, filialId, month, year, monthEnd, yearEnd);
            var publicServicesPlans = publicServicesPlansInit
           .GroupBy(n => n.PublicServiceTypeId)
           .Select(n => new
           {
               PublicServiceTypeId = n.Key,
               Sum = n.Sum(c => c.Sum)
           })
           .ToList();
            var publicServicesSubtractions = _userCommands.GetPublicServicesSubtractionsByFilial(_ade, filialId, month, year, monthEnd, yearEnd);

            double totalPublicServices = 0;
            foreach (var publicService in publicServices)
            {
                totalPublicServices += publicService.Sum * publicService.PublicServiceType.VAT;
            }

            double totalPublicServicesSubtractions = 0;
            foreach (var publicServiceSubtraction in publicServicesSubtractions)
            {
                totalPublicServicesSubtractions += publicServiceSubtraction.Sum * publicServiceSubtraction.PublicServiceType.VAT;
            }

            totalPublicServices -= totalPublicServicesSubtractions;

            var publicServicesFact = totalPublicServices;
            var publicServicesPlan = 0.0;

            if (publicServicesPlans.Count == 1)
            {
                if (publicServicesPlans[0].PublicServiceTypeId == 9)
                {
                    publicServicesPlan = publicServicesPlans[0].Sum;
                }
            }
            else
            {
                ///позже если план будет по категориям
                publicServicesPlan = publicServicesPlans.Sum(p => p.Sum);
            }

            fact = premisesRent * 1.2 + publicServicesFact;
            plan = (adminExpensePlan != null ? adminExpensePlan.PremisesRent : 0) + publicServicesPlan;
            per = plan != 0 ? (fact - plan) / plan * 100 : 0;
            kpiAdminPart.Rows.Add(new KPIRow { Name = "Административные затраты", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
            totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;

            fact = premisesRent * 1.2;
            plan = adminExpensePlan != null ? adminExpensePlan.PremisesRent : 0;
            per = plan != 0 ? (fact - plan) / plan * 100 : 0;
            kpiAdminPart.Rows.Add(new KPIRow { Name = "Аренда помещений", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });

            fact = publicServicesFact; plan = publicServicesPlan; per = (fact - plan) / plan * 100;
            kpiAdminPart.Rows.Add(new KPIRow { Name = "Коммунальные платежи", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per, Link = true });


            //var transportTotal = kpi.AutoPart.Rows.Find(p => p.Name == "Затраты на транспорт");
            totalCurrentExpensesFact += (double)(transportTotal != null ? transportTotal.Fact : 0); totalCurrentExpensesPlan += (double)(transportTotal != null ? transportTotal.Plan : 0);
            //var personalTotal = kpi.SalaryPart.Rows.Find(p => p.Name == "Затраты на персонал");
            totalCurrentExpensesFact += (double)(personalTotal != null ? personalTotal.Fact : 0); totalCurrentExpensesPlan += (double)(personalTotal != null ? personalTotal.Plan : 0);

            fact = totalCurrentExpensesFact; plan = totalCurrentExpensesPlan; per = (fact - plan) / plan * 100;
            kpiAdminPart.Rows.Add(new KPIRow { Name = "Суммарно затраты ЦФО", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });

            //fact = grossProfitFact - totalCurrentExpensesFact; plan = grossProfitPlan - totalCurrentExpensesPlan; per = (fact - plan) / plan * 100;
            //kpi.AdminPart.Rows.Add(new KPIRow { Name = "Прибыль торгового отдела", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
        }

        public void FillKPIAdminsPast(int filialId, short month, short year, short monthEnd, short yearEnd, KPIAdminPartRow kpiAdminPart,
            KPIAdminPartRow kpiAdminPartPast, KPIRow transportTotal, KPIRow personalTotal, double mobileTelephony, double premisesRent
            /*, double? primeCost, double? discount, double? discountPepsi*/)
        {
            var defectDiscounInit = _userCommands.GetPublicDefectDiscountByFilial(_ade, filialId, month, year, monthEnd, yearEnd);
            var defectDiscoun = defectDiscounInit
              .GroupBy(n => n.FilialId)
              .Select(n => new
              {
                  FilialId = n.Key,
                  Defect = n.Sum(c => c.Defect),
                  DefectAdditional = n.Sum(c => c.DefectAdditional),
                  Discount = n.Sum(c => c.Discount),
                  DiscountAdditional = n.Sum(c => c.DiscountAdditional),
                  EventDistribution = n.Sum(c => c.EventDistribution),
                  PrimeCost = n.Sum(c => c.PrimeCost),
                  Representation = n.Sum(c => c.Representation)
              })
              .FirstOrDefault();


            //var netProfit = kpi.SalePart.Rows.Find(p => p.Name == "Объемы реализации");
            //double grossProfitFact = (double)(netProfit != null ? netProfit.Fact : 0);
            //double grossProfitPlan = (double)(netProfit != null ? netProfit.Plan : 0);

            double fact = 0.0; double plan = 0.0; double per = 0.0;
            var totalCurrentExpensesFact = 0.0; var totalCurrentExpensesPlan = 0.0;

            //if (defectDiscoun != null)
            //{
            //double discFact = 0.0;
            //if (discount.HasValue)
            //    discFact = discount.Value;// - (discountPepsi.HasValue ? discountPepsi.Value : 0);
            //else
            //    discFact = defectDiscoun != null ? (defectDiscoun.Discount ?? 0) : 0;

            //fact = (netProfit != null ? netProfit.Fact : 0) + discFact;//(defectDiscoun != null ? (defectDiscoun.Discount ?? 0) : 0) + (defectDiscoun != null ? (defectDiscoun.DiscountAdditional ?? 0):0);
            //plan = (netProfit != null ? netProfit.Plan : 0) + (defectDiscountPlan.Discount ?? 0) + (defectDiscountPlan.DiscountAdditional ?? 0);
            //per = (fact - plan) / plan * 100;
            //kpi.AdminPart.Rows.Add(new KPIRow { Name = "Валовый доход от продаж", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });


            //fact = discFact;
            //plan = defectDiscountPlan.Discount ?? 0; per = (fact - plan) / plan * 100;
            //kpi.AdminPart.Rows.Add(new KPIRow { Name = "Скидки", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });

            ////if (discountPepsi.HasValue)
            ////    fact = discountPepsi.Value;
            ////else
            ////    fact = defectDiscoun != null ? (defectDiscoun.DiscountAdditional ?? 0) : 0;
            ////plan = defectDiscountPlan.DiscountAdditional ?? 0; per = plan != 0 ? (fact - plan) / plan * 100 : 0;
            ////kpi.AdminPart.Rows.Add(new KPIRow { Name = "Скидки PepsiCo", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });

            //kpi.AdminPart.Rows.Add(new KPIRow { Name = "Чистый доход от продаж", Plan = netProfit.Plan, Fact = netProfit.Fact, Percent = netProfit.Percent });

            //if (primeCost.HasValue)
            //    fact = primeCost.Value * 1.2;
            //else
            //    fact = defectDiscoun != null ? (defectDiscoun.PrimeCost ?? 0) * 1.2 : 0;
            //plan = (netProfit.Fact == 0 || fact == 0) ? 0 : (double)netProfit.Plan / (double)netProfit.Fact * fact; per = plan == 0 ? 0 : (fact - plan) / plan * 100;
            //kpi.AdminPart.Rows.Add(new KPIRow { Name = "Себестоймость", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
            //grossProfitFact -= fact; grossProfitPlan -= plan;



            fact = defectDiscoun != null ? defectDiscoun.Defect : 0;
            plan = GetFactKPIAminsByKey(kpiAdminPart, "Брак");
            per = (double)(plan - fact) / fact * 100;
            kpiAdminPartPast.Rows.Add(new KPIRow { Name = "Брак", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
            totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;
            //grossProfitFact -= fact; grossProfitPlan -= plan;

            fact = defectDiscoun != null ? defectDiscoun.DefectAdditional ?? 0 : 0; //fact /= 1.27; fact += fact * 0.1;//22.03.17
            plan = GetFactKPIAminsByKey(kpiAdminPart, "Брак Другое");
            per = (double)(plan - fact) / fact * 100;
            kpiAdminPartPast.Rows.Add(new KPIRow { Name = "Брак Другое"/*"Брак PepsiCo"*/, Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
            totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;
            //grossProfitFact -= fact; grossProfitPlan -= plan;

            //fact = grossProfitFact; plan = grossProfitPlan; per = plan != 0 ? (fact - plan) / plan * 100 : 0;
            //kpi.AdminPart.Rows.Add(new KPIRow { Name = "Валовая прибыль", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });

            ////fact = defectDiscoun != null ? defectDiscoun.EventDistribution : 0; plan = defectDiscountPlan.EventDistribution; per = (fact - plan) / plan * 100;//per = plan != 0 ? (fact - plan) / plan * 100 : 0;
            ////kpi.AdminPart.Rows.Add(new KPIRow { Name = "Акционная раздача воды", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
            ////totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;

            fact = defectDiscoun != null ? defectDiscoun.Representation : 0;
            plan = GetFactKPIAminsByKey(kpiAdminPart, "Расходы по филиалу");
            per = (double)(plan - fact) / fact * 100;
            kpiAdminPartPast.Rows.Add(new KPIRow { Name = "Расходы по филиалу", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
            totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;
            //}




            fact = mobileTelephony;
            plan = GetFactKPIAminsByKey(kpiAdminPart, "Затраты на мобильную тефонию");
            per = (double)(plan - fact) / fact * 100;
            kpiAdminPartPast.Rows.Add(new KPIRow { Name = "Затраты на мобильную тефонию", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
            totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;


            var publicServices = _userCommands.GetPublicServicesByFilial(_ade, filialId, month, year, monthEnd, yearEnd);

            var publicServicesSubtractions = _userCommands.GetPublicServicesSubtractionsByFilial(_ade, filialId, month, year, monthEnd, yearEnd);

            double totalPublicServices = 0;
            foreach (var publicService in publicServices)
            {
                totalPublicServices += publicService.Sum * publicService.PublicServiceType.VAT;
            }

            double totalPublicServicesSubtractions = 0;
            foreach (var publicServiceSubtraction in publicServicesSubtractions)
            {
                totalPublicServicesSubtractions += publicServiceSubtraction.Sum * publicServiceSubtraction.PublicServiceType.VAT;
            }

            totalPublicServices -= totalPublicServicesSubtractions;

            var publicServicesFact = totalPublicServices;


            fact = premisesRent * 1.2 + publicServicesFact;
            plan = GetFactKPIAminsByKey(kpiAdminPart, "Административные затраты");
            per = (double)(plan - fact) / fact * 100;
            kpiAdminPartPast.Rows.Add(new KPIRow { Name = "Административные затраты", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
            totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;

            fact = premisesRent * 1.2;
            plan = GetFactKPIAminsByKey(kpiAdminPart, "Аренда помещений");
            per = (double)(plan - fact) / fact * 100;
            kpiAdminPartPast.Rows.Add(new KPIRow { Name = "Аренда помещений", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });

            fact = publicServicesFact;
            plan = GetFactKPIAminsByKey(kpiAdminPart, "Коммунальные платежи");
            per = (double)(plan - fact) / fact * 100;
            kpiAdminPartPast.Rows.Add(new KPIRow { Name = "Коммунальные платежи", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per, Link = true });


            //var transportTotal = kpi.AutoPart.Rows.Find(p => p.Name == "Затраты на транспорт");
            totalCurrentExpensesFact += (double)(transportTotal != null ? transportTotal.Fact : 0);
            //totalCurrentExpensesPlan += (double)(transportTotal != null ? transportTotal.Plan : 0);
            //var personalTotal = kpi.SalaryPart.Rows.Find(p => p.Name == "Затраты на персонал");
            totalCurrentExpensesFact += (double)(personalTotal != null ? personalTotal.Fact : 0);
            //totalCurrentExpensesPlan += (double)(personalTotal != null ? personalTotal.Plan : 0);

            fact = totalCurrentExpensesFact;
            //plan = totalCurrentExpensesPlan;
            //per = (fact - plan) / plan * 100;
            plan = GetFactKPIAminsByKey(kpiAdminPart, "Суммарно затраты ЦФО");
            per = (double)(plan - fact) / fact * 100;
            kpiAdminPartPast.Rows.Add(new KPIRow { Name = "Суммарно затраты ЦФО", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });

            //fact = grossProfitFact - totalCurrentExpensesFact; plan = grossProfitPlan - totalCurrentExpensesPlan; per = (fact - plan) / plan * 100;
            //kpi.AdminPart.Rows.Add(new KPIRow { Name = "Прибыль торгового отдела", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
        }

        public void FillKPIPlansAdmins(int filialId, short month, short year, short monthEnd, short yearEnd,
            KPIAdminPartRow kpiAdminPart, KPIRow transportTotal, KPIRow personalTotal, double mobileTelephony, double premisesRent
            /*, double? primeCost, double? discount, double? discountPepsi*/)
        {
            short yearPast = (short)(year - 1);
            short yearEndPast = (short)(yearEnd - 1);

            DateTime dateStart = new DateTime(yearPast, month, 1);
            DateTime dateEnd = new DateTime(yearEndPast, monthEnd, 1);
            //dateStart = dateStart.AddMonths(-1);
            //dateEnd = dateEnd.AddMonths(-1);

            var defectDiscounInit = _userCommands.GetPublicDefectDiscountByFilial(_ade, filialId, (short)dateStart.Month, (short)dateStart.Year, (short)dateEnd.Month, (short)dateEnd.Year);
            var defectDiscoun = defectDiscounInit
              .GroupBy(n => n.FilialId)
              .Select(n => new
              {
                  FilialId = n.Key,
                  Defect = n.Sum(c => c.Defect),
                  DefectAdditional = n.Sum(c => c.DefectAdditional),
                  Discount = n.Sum(c => c.Discount),
                  DiscountAdditional = n.Sum(c => c.DiscountAdditional),
                  EventDistribution = n.Sum(c => c.EventDistribution),
                  PrimeCost = n.Sum(c => c.PrimeCost),
                  Representation = n.Sum(c => c.Representation)
              })
              .FirstOrDefault();

            var defectDiscountPlanInit = _userCommands.GetPublicDefectDiscountPlanByFilial(_ade, filialId, month, year, monthEnd, yearEnd);
            var defectDiscountPlan = defectDiscountPlanInit
            .GroupBy(n => n.FilialId)
            .Select(n => new
            {
                FilialId = n.Key,
                Defect = n.Sum(c => c.Defect),
                DefectAdditional = n.Sum(c => c.DefectAdditional),
                Discount = n.Sum(c => c.Discount),
                DiscountAdditional = n.Sum(c => c.DiscountAdditional),
                EventDistribution = n.Sum(c => c.EventDistribution),
                Representation = n.Sum(c => c.Representation)
            })
            .FirstOrDefault();

            //var netProfit = kpi.SalePart.Rows.Find(p => p.Name == "Объемы реализации");
            //double grossProfitFact = (double)(netProfit != null ? netProfit.Fact : 0);
            //double grossProfitPlan = (double)(netProfit != null ? netProfit.Plan : 0);

            double fact = 0.0; double plan = 0.0; double per = 0.0;
            var totalCurrentExpensesFact = 0.0; var totalCurrentExpensesPlan = 0.0;

            if (defectDiscountPlan != null)
            {

                fact = defectDiscountPlan.Defect;
                plan = defectDiscoun != null ? defectDiscoun.Defect : 0;
                per = plan != 0 ? (fact - plan) / plan * 100 : 0;
                kpiAdminPart.Rows.Add(new KPIRow { Name = "Брак", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
                totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;
                //grossProfitFact -= fact; grossProfitPlan -= plan;

                fact = defectDiscountPlan.DefectAdditional ?? 0;//fact /= 1.27; fact += fact * 0.1;//22.03.17
                plan = defectDiscoun != null ? defectDiscoun.DefectAdditional ?? 0 : 0;
                per = plan != 0 ? (fact - plan) / plan * 100 : 0;
                kpiAdminPart.Rows.Add(new KPIRow { Name = "Брак Другое"/*"Брак PepsiCo"*/, Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
                totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;
                //grossProfitFact -= fact; grossProfitPlan -= plan;

                //fact = grossProfitFact; plan = grossProfitPlan; per = plan != 0 ? (fact - plan) / plan * 100 : 0;
                //kpi.AdminPart.Rows.Add(new KPIRow { Name = "Валовая прибыль", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });

                ////fact = defectDiscoun != null ? defectDiscoun.EventDistribution : 0; plan = defectDiscountPlan.EventDistribution; per = (fact - plan) / plan * 100;//per = plan != 0 ? (fact - plan) / plan * 100 : 0;
                ////kpi.AdminPart.Rows.Add(new KPIRow { Name = "Акционная раздача воды", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
                ////totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;

                fact = defectDiscountPlan.Representation;
                plan = defectDiscoun != null ? defectDiscoun.Representation : 0;
                per = plan != 0 ? (fact - plan) / plan * 100 : 0;
                kpiAdminPart.Rows.Add(new KPIRow { Name = "Расходы по филиалу", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
                totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;
            }

            var adminExpensePlanInit = _userCommands.GetPublicAdminExpensePlanPlanByFilial(_ade, filialId, month, year, monthEnd, yearEnd);
            var adminExpensePlan = adminExpensePlanInit
           .GroupBy(n => n.FilialId)
           .Select(n => new
           {
               FilialId = n.Key,
               MobileTelephony = n.Sum(c => c.MobileTelephony),
               PremisesRent = n.Sum(c => c.PremisesRent),
               TransportRent = n.Sum(c => c.TransportRent),
               WashTransport = n.Sum(c => c.WashTransport)
           })
           .FirstOrDefault();

            if (adminExpensePlan != null)
            {
                fact = adminExpensePlan.MobileTelephony;
                plan = mobileTelephony;
                per = plan != 0 ? (fact - plan) / plan * 100 : 0;
                kpiAdminPart.Rows.Add(new KPIRow { Name = "Затраты на мобильную тефонию", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
                totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;
            }

            var publicServices = _userCommands.GetPublicServicesByFilial(_ade, filialId, (short)dateStart.Month, (short)dateStart.Year, (short)dateEnd.Month, (short)dateEnd.Year);
            var publicServicesPlansInit = _userCommands.GetPublicServicesPlansByFilial(_ade, filialId, month, year, monthEnd, yearEnd);
            var publicServicesPlans = publicServicesPlansInit
           .GroupBy(n => n.PublicServiceTypeId)
           .Select(n => new
           {
               PublicServiceTypeId = n.Key,
               Sum = n.Sum(c => c.Sum)
           })
           .ToList();
            var publicServicesSubtractions = _userCommands.GetPublicServicesSubtractionsByFilial(_ade, filialId, (short)dateStart.Month, (short)dateStart.Year, (short)dateEnd.Month, (short)dateEnd.Year);

            double totalPublicServices = 0;
            foreach (var publicService in publicServices)
            {
                totalPublicServices += publicService.Sum * publicService.PublicServiceType.VAT;
            }

            double totalPublicServicesSubtractions = 0;
            foreach (var publicServiceSubtraction in publicServicesSubtractions)
            {
                totalPublicServicesSubtractions += publicServiceSubtraction.Sum * publicServiceSubtraction.PublicServiceType.VAT;
            }

            totalPublicServices -= totalPublicServicesSubtractions;

            var publicServicesFact = totalPublicServices;
            var publicServicesPlan = 0.0;

            if (publicServicesPlans.Count == 1)
            {
                if (publicServicesPlans[0].PublicServiceTypeId == 9)
                {
                    publicServicesPlan = publicServicesPlans[0].Sum;
                }
            }
            else
            {
                ///позже если план будет по категориям
                publicServicesPlan = publicServicesPlans.Sum(p => p.Sum);
            }

            fact = (adminExpensePlan != null ? adminExpensePlan.PremisesRent : 0) + publicServicesPlan;
            plan = premisesRent * 1.2 + publicServicesFact;
            per = plan != 0 ? (fact - plan) / plan * 100 : 0;
            kpiAdminPart.Rows.Add(new KPIRow { Name = "Административные затраты", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
            totalCurrentExpensesFact += fact; totalCurrentExpensesPlan += plan;

            fact = adminExpensePlan != null ? adminExpensePlan.PremisesRent : 0;
            plan = premisesRent * 1.2;
            per = plan != 0 ? (fact - plan) / plan * 100 : 0;
            kpiAdminPart.Rows.Add(new KPIRow { Name = "Аренда помещений", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });

            fact = publicServicesPlan;
            plan = publicServicesFact;
            per = (fact - plan) / plan * 100;
            kpiAdminPart.Rows.Add(new KPIRow { Name = "Коммунальные платежи", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per, Link = false });


            //var transportTotal = kpi.AutoPart.Rows.Find(p => p.Name == "Затраты на транспорт");
            totalCurrentExpensesFact += (double)(transportTotal != null ? transportTotal.Fact : 0); totalCurrentExpensesPlan += (double)(transportTotal != null ? transportTotal.Plan : 0);
            //var personalTotal = kpi.SalaryPart.Rows.Find(p => p.Name == "Затраты на персонал");
            totalCurrentExpensesFact += (double)(personalTotal != null ? personalTotal.Fact : 0); totalCurrentExpensesPlan += (double)(personalTotal != null ? personalTotal.Plan : 0);

            fact = totalCurrentExpensesFact; plan = totalCurrentExpensesPlan; per = (fact - plan) / plan * 100;
            kpiAdminPart.Rows.Add(new KPIRow { Name = "Суммарно затраты ЦФО", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });

            //fact = grossProfitFact - totalCurrentExpensesFact; plan = grossProfitPlan - totalCurrentExpensesPlan; per = (fact - plan) / plan * 100;
            //kpi.AdminPart.Rows.Add(new KPIRow { Name = "Прибыль торгового отдела", Plan = Convert.ToInt32(plan), Fact = Convert.ToInt32(fact), Percent = per });
        }

        public void FillKPITotal(/*int filialId1, short month1, short year1,*/ KPIViewModel kpi)
        {
            var totalExpenses = kpi.AdminPart.Rows.Find(p => p.Name == "Суммарно затраты ЦФО");
            var totalSalaries = kpi.SalaryPart.Rows.Find(p => p.Name == "Количество сотрудников");

            double fact = 0.0, plan = 0.0;

            if (totalExpenses != null && totalSalaries != null)
            {
                fact = (double)totalExpenses.Fact / totalSalaries.Fact; plan = (double)totalExpenses.Plan / totalSalaries.Plan;
                kpi.TotalPart.Rows.Add(new KPIRowT { Name = "Затраты на сотрудника суммарно", Plan = plan, Fact = fact });
            }

            var totalSalary = kpi.SalaryPart.Rows.Find(p => p.Name == "Затраты на персонал");

            if (totalSalary != null && totalSalaries != null)
            {
                fact = (double)totalSalary.Fact / totalSalaries.Fact; plan = (double)totalSalary.Plan / totalSalaries.Plan;
                kpi.TotalPart.Rows.Add(new KPIRowT { Name = "Средняя зарплата на сотрудника", Plan = plan, Fact = fact });
            }

            var totalCombustible = kpi.AutoPart.Rows.Find(p => p.Name == "Затраты на топливо");
            var totalSales = kpi.SalePart.Rows.Find(p => p.Name == "Объемы реализации");

            if (totalCombustible != null && totalSales != null)
            {
                fact = (double)totalCombustible.Fact / totalSales.Fact; plan = (double)totalCombustible.Plan / totalSales.Plan;
                kpi.TotalPart.Rows.Add(new KPIRowT { Name = "Затраты на топливо на единицу реализации", Plan = plan, Fact = fact });
            }

            var totalSpares = kpi.AutoPart.Rows.Find(p => p.Name == "Запчасти для ремонтов авто");

            if (totalSpares != null && totalSales != null)
            {
                fact = (double)totalSpares.Fact / totalSales.Fact; plan = (double)totalSpares.Plan / totalSales.Plan;
                kpi.TotalPart.Rows.Add(new KPIRowT { Name = "Запчасти для авто на единицу реализации", Plan = plan, Fact = fact });
            }

            var totalMaintenance = kpi.AutoPart.Rows.Find(p => p.Name == "Обслуживание автомобилей");

            if (totalMaintenance != null && totalSales != null)
            {
                fact = (double)totalMaintenance.Fact / totalSales.Fact; plan = (double)totalMaintenance.Plan / totalSales.Plan;
                kpi.TotalPart.Rows.Add(new KPIRowT { Name = "Обслуживание автомобилей на единицу реализации", Plan = plan, Fact = fact });
            }

            var totalMobileTelephony = kpi.AdminPart.Rows.Find(p => p.Name == "Затраты на мобильную тефонию");

            if (totalMobileTelephony != null && totalSalaries != null)
            {
                fact = (double)totalMobileTelephony.Fact / totalSalaries.Fact; plan = (double)totalMobileTelephony.Plan / totalSalaries.Plan;
                kpi.TotalPart.Rows.Add(new KPIRowT { Name = "Затраты на мобильную телефонию на сотрудника", Plan = plan, Fact = fact });
            }

            var totalRepresentation = kpi.AdminPart.Rows.Find(p => p.Name == "Представительские");

            if (totalRepresentation != null && totalSalaries != null)
            {
                fact = (double)totalRepresentation.Fact / totalSalaries.Fact; plan = (double)totalRepresentation.Plan / totalSalaries.Plan;
                kpi.TotalPart.Rows.Add(new KPIRowT { Name = "Представительские на сотрудника", Plan = plan, Fact = fact });
            }

            var totalGrossProfit = kpi.AdminPart.Rows.Find(p => p.Name == "Прибыль торгового отдела");

            if (totalGrossProfit != null && totalSalaries != null)
            {
                fact = (double)totalGrossProfit.Fact / totalSalaries.Fact; plan = (double)totalGrossProfit.Plan / totalSalaries.Plan;
                kpi.TotalPart.Rows.Add(new KPIRowT { Name = "Прибыль на сотрудника", Plan = plan, Fact = fact });
            }

            var totalNetProfit = kpi.AdminPart.Rows.Find(p => p.Name == "Чистый доход от продаж");

            if (totalGrossProfit != null && totalNetProfit != null)
            {
                fact = (double)totalGrossProfit.Fact / totalNetProfit.Fact * 100; plan = (double)totalGrossProfit.Plan / totalNetProfit.Plan * 100;
                kpi.TotalPart.Rows.Add(new KPIRowT { Name = "Рентабельность филиала", Plan = plan, Fact = fact });
            }
        }


        public List<Salary> GetSalariesDetails(int filialId, short month, short year, short monthEnd, short yearEnd, string post)
        {
            var departmentId = 0;
            if (filialId == 2)
                departmentId = 10;
            if (filialId == 1)
                departmentId = 7;


            return _userCommands.GetSalariesByPost(_ade, filialId, month, year, monthEnd, yearEnd, post, departmentId);
        }

        public List<Auto> GetAutosDetails(int filialId, short month, short year, short monthEnd, short yearEnd, string autosName)
        {
            var departmentId = 0;
            if (filialId == 2)
                departmentId = 10;
            if (filialId == 1)
                departmentId = 7;

            var type = AutoType.All;
            if (autosName.ToLower().Contains("грузовой") || autosName.ToLower().Contains("грузового"))
                type = AutoType.Truck;
            else if (autosName.ToLower().Contains("легковой") || autosName.ToLower().Contains("легкового"))
                type = AutoType.Car;
            else if (autosName.ToLower().Contains("дистрибьюторский"))
                type = AutoType.Distrib;

            var autos = _userCommands.GetAutosByType(_ade, type, filialId, month, year, monthEnd, yearEnd, departmentId);

            if (autosName.ToLower().Contains("шины") && autosName.ToLower().Contains("аккумуляторы"))
                autos = autos.Where(p => p.Tires > 0 || p.Accumulators > 0).ToList();
            if (autosName == "Запасные части - другие комплектующие авто")
                autos = autos.Where(p => p.Spares > 0).ToList();
            if (autosName == "ТО грузового транспорта" || autosName == "ТО легкового транспорта")
                autos = autos.Where(p => p.Expenses > 0 || p.Lubricants > 0 || p.TestareAuto > 0).ToList();
            if (autosName == "Услуги по ремонтам")
                autos = autos.Where(p => p.Repairs > 0 || p.Services > 0).ToList();

            return autos;
        }

        public List<Auto> GetAutosReport(int filialId, int departmentId, short month, short year, short monthEnd, short yearEnd,
            short combustibleType, short autoType, string number, int project, string jtSorting)
        {
            //var departmentId = 0;
            //if (filialId == 2)
            //    departmentId = 10;
            //if (filialId == 1)
            //    departmentId = 7;
            if (filialId == -1)
                return new List<Auto>();

            var tempAutos =
                _userCommands.GetAutos(_ade, filialId, departmentId, month, year, monthEnd, yearEnd, combustibleType, autoType, number, project);

            //var tempAutosPerson = tempAutos.Select(p => p.Person.Trim()).Distinct().ToList();
            //tempAutosPerson.RemoveAll(p => p == "");

            var tempSalaries =
               _userCommands.GetSalaries(_ade, filialId, departmentId, month, year, monthEnd, yearEnd, -1, 0, "- Все - ", string.Empty);

            foreach (var sal in tempSalaries)
            {
                sal.Employee = sal.Employee.Replace(" ", string.Empty);
            }

            foreach (var auto in tempAutos)
            {
                auto.Accumulators = Math.Round(auto.Accumulators * 1.2, 2);
                auto.Expenses = Math.Round(auto.Expenses * 1.2, 2);
                auto.Lubricants = Math.Round(auto.Lubricants * 1.2, 2);
                auto.Repairs = Math.Round(auto.Repairs * 1.2, 2);
                auto.Services = Math.Round(auto.Services * 1.2, 2);
                auto.Spares = Math.Round(auto.Spares * 1.2, 2);
                auto.TestareAuto = Math.Round(auto.TestareAuto * 1.2, 2);
                auto.Tires = Math.Round(auto.Tires * 1.2, 2);
                if (auto.Weight.HasValue)
                    auto.Weight = Math.Round(auto.Weight.Value, 2);

                auto.TiresAccumulators = auto.Tires + auto.Accumulators;

                var autoPerson = auto.Person.Replace(" ", string.Empty);
                if (autoPerson.Length > 0)
                {
                    

                    var tempSalariesInAuto =
                        tempSalaries.FirstOrDefault(p => p.Employee.Contains(autoPerson) && p.Year == auto.Year && p.Month == auto.Month);

                    if (tempSalariesInAuto != null)
                        auto.SalaryCard
                            = Math.Round(tempSalariesInAuto.Card
                                        + tempSalariesInAuto.Cash
                                        + tempSalariesInAuto.Vacation
                                        + tempSalariesInAuto.SickLeave, 0);
                }
            }


            switch (jtSorting)
            {
                case "Filial ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Filial.Name).ToList();
                    break;
                case "Filial DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Filial.Name).ToList();
                    break;
                case "Department ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Department.Name).ToList();
                    break;
                case "Department DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Department.Name).ToList();
                    break;
                case "Month ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Month).ToList();
                    break;
                case "Month DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Month).ToList();
                    break;
                case "Year ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Year).ToList();
                    break;
                case "Year DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Year).ToList();
                    break;
                case "Number ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Number).ToList();
                    break;
                case "Number DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Number).ToList();
                    break;
                case "Person ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Person).ToList();
                    break;
                case "Person DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Person).ToList();
                    break;

                case "SalaryCard ASC":
                    tempAutos = tempAutos.OrderBy(t => t.SalaryCard).ToList();
                    break;
                case "SalaryCard DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.SalaryCard).ToList();
                    break;

                case "Brand ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Brand).ToList();
                    break;
                case "Brand DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Brand).ToList();
                    break;
                case "Type ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Type).ToList();
                    break;
                case "Type DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Type).ToList();
                    break;
                case "Сombustible ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Сombustible).ToList();
                    break;
                case "Сombustible DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Сombustible).ToList();
                    break;
                case "СombustiblePrice ASC":
                    tempAutos = tempAutos.OrderBy(t => t.СombustiblePrice).ToList();
                    break;
                case "СombustiblePrice DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.СombustiblePrice).ToList();
                    break;
                case "Km ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Km).ToList();
                    break;
                case "Km DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Km).ToList();
                    break;
                case "Liters ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Liters).ToList();
                    break;
                case "Liters DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Liters).ToList();
                    break;

                //case "Tires ASC":
                //    tempAutos = tempAutos.OrderBy(t => t.Tires).ToList();
                //    break;
                //case "Tires DESC":
                //    tempAutos = tempAutos.OrderByDescending(t => t.Tires).ToList();
                //    break;

                case "TiresAccumulators ASC":
                    tempAutos = tempAutos.OrderBy(t => t.TiresAccumulators).ToList();
                    break;
                case "TiresAccumulators DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.TiresAccumulators).ToList();
                    break;

                //case "Accumulators ASC":
                //    tempAutos = tempAutos.OrderBy(t => t.Accumulators).ToList();
                //    break;
                //case "Accumulators DESC":
                //    tempAutos = tempAutos.OrderByDescending(t => t.Accumulators).ToList();
                //    break;

                case "Spares ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Spares).ToList();
                    break;
                case "Spares DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Spares).ToList();
                    break;
                case "TestareAuto ASC":
                    tempAutos = tempAutos.OrderBy(t => t.TestareAuto).ToList();
                    break;
                case "TestareAuto DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.TestareAuto).ToList();
                    break;
                case "Lubricants ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Lubricants).ToList();
                    break;
                case "Lubricants DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Lubricants).ToList();
                    break;
                case "Services ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Services).ToList();
                    break;
                case "Services DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Services).ToList();
                    break;
                case "Expenses ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Expenses).ToList();
                    break;
                case "Expenses DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Expenses).ToList();
                    break;
                case "Repairs ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Repairs).ToList();
                    break;
                case "Repairs DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Repairs).ToList();
                    break;
                case "Weight ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Weight).ToList();
                    break;
                case "Weight DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Weight).ToList();
                    break;
                case "Routes ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Routes).ToList();
                    break;
                case "Routes DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Routes).ToList();
                    break;
            }


            return tempAutos;
        }

        public double GetAutosWeightByFilial(int filialId, short month, short year, bool byYear)
        {
           var weightSum = _userCommands.GetAutosWeightByFilial(_ade, filialId, month, year, byYear);

            if (weightSum.HasValue)
                return weightSum.Value;
            else
                return 0;
        }

        public List<string> GetAutoNumbers(string query)
        {
            return _userCommands.GetAutoNumbers(_ade, query);
        }

        public List<string> GetEmployees(string query)
        {
            return _userCommands.GetEmployees(_ade, query);
        }

        public List<Salary> GetSalariesReport(int filialId, int departmentId, short month, short year, short monthEnd, short yearEnd,
            short personType, int project, string post, string employee, string jtSorting)
        {
            //var departmentId = 0;
            //if (filialId == 2)
            //    departmentId = 10;
            //if (filialId == 1)
            //    departmentId = 7;
            if (filialId == -1)
                return new List<Salary>();

            var tempSalaries =
                _userCommands.GetSalaries(_ade, filialId, departmentId, month, year, monthEnd, yearEnd, personType, project, post, employee);

            foreach(var sal in tempSalaries)
            {
                sal.TotalTax = Math.Round((sal.PensionFund ?? 0.0) + (sal.HealthInsurance ?? 0.0) * 2 
                    + (sal.IncomeTax ?? 0.0) + (sal.SocialFund ?? 0.0), 2);
            }


            switch (jtSorting)
            {
                case "Filial ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.Filial.Name).ToList();
                    break;
                case "Filial DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.Filial.Name).ToList();
                    break;

                case "Department ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.Department.Name).ToList();
                    break;
                case "Department DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.Department.Name).ToList();
                    break;

                case "Month ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.Month).ToList();
                    break;
                case "Month DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.Month).ToList();
                    break;

                case "Year ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.Year).ToList();
                    break;
                case "Year DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.Year).ToList();
                    break;

                case "Employee ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.Employee).ToList();
                    break;
                case "Employee DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.Employee).ToList();
                    break;

                case "Post ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.Post).ToList();
                    break;
                case "Post DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.Post).ToList();
                    break;

                case "CFR ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.CFR).ToList();
                    break;
                case "CFR DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.CFR).ToList();
                    break;

                case "Type ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.Type).ToList();
                    break;
                case "Type DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.Type).ToList();
                    break;

                case "Card ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.Card).ToList();
                    break;
                case "Card DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.Card).ToList();
                    break;

                case "Vacation ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.Vacation).ToList();
                    break;
                case "Vacation DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.Vacation).ToList();
                    break;

                case "SickLeave ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.SickLeave).ToList();
                    break;
                case "SickLeave DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.SickLeave).ToList();
                    break;

                case "TotalTax ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.TotalTax).ToList();
                    break;
                case "TotalTax DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.TotalTax).ToList();
                    break;

                case "Deduction ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.Deduction).ToList();
                    break;
                case "Deduction DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.Deduction).ToList();
                    break;

            }

            return tempSalaries;
        }


        public List<SalaryCompareViewModel> GetSalariesCompareReport(int filialId, int departmentId, short month1, short year1, short month2, short year2,
            short type, int project, bool byYear, bool isTax, string jtSorting)
        {

            if (filialId == -1)
                return new List<SalaryCompareViewModel>();

            var salariesGroupedByDepartment1 = new List<SalariesGroupedByDepartment>();
            var salariesGroupedByDepartment2 = new List<SalariesGroupedByDepartment>();

            if (type == 1)
            {
                salariesGroupedByDepartment1 =
                   _userCommands.GetGroupedByDepartmentSalaries(_ade, filialId, departmentId, month1, year1, project, byYear);

                salariesGroupedByDepartment2 =
                    _userCommands.GetGroupedByDepartmentSalaries(_ade, filialId, departmentId, month2, year2, project, byYear);
            }
            else if (type == 2)
            {
                salariesGroupedByDepartment1 =
                   _userCommands.GetGroupedByPostSalaries(_ade, filialId, departmentId, month1, year1, project, byYear);

                salariesGroupedByDepartment2 =
                    _userCommands.GetGroupedByPostSalaries(_ade, filialId, departmentId, month2, year2, project, byYear);
            }

            var tempSalaries = GetSalaryCompareViewModel(salariesGroupedByDepartment1, salariesGroupedByDepartment2, isTax);

            switch (jtSorting)
            {
                case "Difference ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.Difference).ToList();
                    break;
                case "Difference DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.Difference).ToList();
                    break;

                case "Name1 ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.Name1).ToList();
                    break;
                case "Name1 DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.Name1).ToList();
                    break;

                case "Name2 ASC":
                    tempSalaries = tempSalaries.OrderBy(t => t.Name2).ToList();
                    break;
                case "Name2 DESC":
                    tempSalaries = tempSalaries.OrderByDescending(t => t.Name2).ToList();
                    break;
            }

            return tempSalaries;
        }

        public List<SalaryCompareViewModel> GetSalaryCompareViewModel(List<SalariesGroupedByDepartment> salariesGrouped1, 
            List<SalariesGroupedByDepartment> salariesGrouped2, bool isTax)
        {
            List<SalaryCompareViewModel> salaryCompare = new List<SalaryCompareViewModel>();

            var tax = 0.0;

            foreach (var sal in salariesGrouped1)
            {
                if (isTax) tax = sal.SumTax;

                salaryCompare.Add(new SalaryCompareViewModel
                {
                    Name1 = sal.Name,
                    SumSal1 = Math.Round(sal.SumSal, 2),
                    SumVac1 = Math.Round(sal.SumVac, 2),
                    SumTax1 = Math.Round(tax, 2),
                    Count1 = sal.Count,
                    Difference = Math.Round(0 - sal.SumVac - sal.SumSal - tax, 2)
                });
            }

            tax = 0.0;

            foreach (var sal in salariesGrouped2)
            {
                if (isTax) tax = sal.SumTax;

                var existingSal = salaryCompare.FirstOrDefault(x => x.Name1 == sal.Name);
                if (existingSal != null)
                {
                    existingSal.Name2 = sal.Name;
                    existingSal.SumVac2 = Math.Round(sal.SumVac, 2);
                    existingSal.SumSal2 = Math.Round(sal.SumSal, 2);
                    existingSal.SumTax2 = Math.Round(tax, 2);
                    existingSal.Count2 = sal.Count;
                    existingSal.Difference = Math.Round(existingSal.SumSal2 + existingSal.SumVac2 + existingSal.SumTax2 - existingSal.SumSal1 - existingSal.SumVac1 - existingSal.SumTax1, 2);
                    //existingPost.Percent = (double)(existingPost.Fact - post.Count) / post.Count * 100;
                }
                else
                {
                    salaryCompare.Add(new SalaryCompareViewModel
                    {
                        Name2 = sal.Name,
                        SumSal2 = Math.Round(sal.SumSal, 2),
                        SumVac2 = Math.Round(sal.SumVac, 2),
                        SumTax2 = Math.Round(tax, 2),
                        Count2 = sal.Count,
                        Difference = Math.Round(sal.SumSal + sal.SumVac + tax, 2)
                    });
                }
            }

            return salaryCompare;
        }


        public List<AutoCompareViewModel> GetAutosCompareReport(int filialId, int departmentId, short month1, short year1, short month2, short year2,
            short type, short autoType, int project, bool byYear, string jtSorting)
        {

            if (filialId == -1)
                return new List<AutoCompareViewModel>();

            var autosGroupedByDepartment1 = new List<AutoGroupedBy>();
            var autosGroupedByDepartment2 = new List<AutoGroupedBy>();

            Expression<Func<Auto, object>> autoGroupingProperty 
                = p => p.Department.Name;

            if (type == 2)
            {
                autoGroupingProperty = p => p.Brand;
            }
            else if (type == 3)
            {
                autoGroupingProperty = p => p.Type;
            }

            autosGroupedByDepartment1 =
                _userCommands.GetGroupedAutos(_ade, filialId, departmentId, month1, year1, project, autoType, byYear, autoGroupingProperty);

            autosGroupedByDepartment2 =
               _userCommands.GetGroupedAutos(_ade, filialId, departmentId, month2, year2, project, autoType, byYear, autoGroupingProperty);

            var tempAutos = GetAutoCompareViewModel(autosGroupedByDepartment1, autosGroupedByDepartment2);

            switch (jtSorting)
            {
                case "Difference ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Difference).ToList();
                    break;
                case "Difference DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Difference).ToList();
                    break;

                case "Name1 ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Name1).ToList();
                    break;
                case "Name1 DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Name1).ToList();
                    break;

                case "Name2 ASC":
                    tempAutos = tempAutos.OrderBy(t => t.Name2).ToList();
                    break;
                case "Name2 DESC":
                    tempAutos = tempAutos.OrderByDescending(t => t.Name2).ToList();
                    break;
            }

            return tempAutos;
        }

        public List<AutoCompareViewModel> GetAutoCompareViewModel(List<AutoGroupedBy> autosGrouped1, List<AutoGroupedBy> autosGrouped2)
        {
            List<AutoCompareViewModel> autoCompare = new List<AutoCompareViewModel>();

            foreach (var auto in autosGrouped1)
            {
                autoCompare.Add(new AutoCompareViewModel
                {
                    Name1 = auto.Name,
                    SumСombustible1 = Math.Round(auto.SumСombustible, 2),
                    SumExpenses1 = Math.Round(auto.SumExpenses, 2),
                    Count1 = auto.Count,
                    Weight1 = Math.Round(auto.Weight, 2),
                    Difference = Math.Round(0 - auto.SumСombustible - auto.SumExpenses, 2)
                });
            }


            foreach (var auto in autosGrouped2)
            {
                var existingSal = autoCompare.FirstOrDefault(x => x.Name1 == auto.Name);
                if (existingSal != null)
                {
                    existingSal.Name2 = auto.Name;
                    existingSal.SumСombustible2 = Math.Round(auto.SumСombustible, 2);
                    existingSal.SumExpenses2 = Math.Round(auto.SumExpenses, 2);
                    existingSal.Count2 = auto.Count;
                    existingSal.Weight2 = Math.Round(auto.Weight, 2); ;
                    existingSal.Difference = Math.Round(existingSal.SumСombustible2 + existingSal.SumExpenses2 
                        - existingSal.SumСombustible1 - existingSal.SumExpenses1, 2);
                }
                else
                {
                    autoCompare.Add(new AutoCompareViewModel
                    {
                        Name2 = auto.Name,
                        SumСombustible2 = Math.Round(auto.SumСombustible, 2),
                        SumExpenses2 = Math.Round(auto.SumExpenses, 2),
                        Count2 = auto.Count,
                        Weight2 = Math.Round(auto.Weight, 2),
                        Difference = Math.Round(auto.SumСombustible + auto.SumExpenses, 2)
                    });
                }
            }

            return autoCompare;
        }

        public List<U> GetCompareViewModel<U,T>(List<T> autosGrouped1, List<T> autosGrouped2) 
            where U : class, new()
            where T : class
        {
            List<U> autoCompare = new List<U>();

            U uTemp = new U();
            var uProperties = uTemp.GetType().GetProperties();
            var tProperties = autosGrouped1[0].GetType().GetProperties();

            foreach (var auto in autosGrouped1)
            {
                uTemp = new U();

                foreach (var tp in tProperties)
                {
                    foreach (var up in uProperties)
                    {
                        if (up.Name == tp.Name+"1")
                        {
                            up.SetValue(uTemp, Convert.ChangeType(tp.GetValue(auto), up.PropertyType));
                        }
                    }
                }

                //autoCompare.Add(new U
                //{
                //    Name1 = auto.Name,
                //    SumСombustible1 = Math.Round(auto.SumСombustible, 2),
                //    SumExpenses1 = Math.Round(auto.SumExpenses, 2),
                //    Count1 = auto.Count,
                //    Difference = Math.Round(0 - auto.SumСombustible - auto.SumExpenses, 2)
                //});
            }


            //foreach (var auto in autosGrouped2)
            //{
            //    var existingSal = autoCompare.FirstOrDefault(x => x.Name1 == auto.Name);
            //    if (existingSal != null)
            //    {
            //        existingSal.Name2 = auto.Name;
            //        existingSal.SumСombustible2 = Math.Round(auto.SumСombustible, 2);
            //        existingSal.SumExpenses2 = Math.Round(auto.SumExpenses, 2);
            //        existingSal.Count2 = auto.Count;
            //        existingSal.Difference = Math.Round(existingSal.SumСombustible2 + existingSal.SumExpenses2
            //            - existingSal.SumСombustible1 - existingSal.SumExpenses1, 2);
            //    }
            //    else
            //    {
            //        autoCompare.Add(new AutoCompareViewModel
            //        {
            //            Name2 = auto.Name,
            //            SumСombustible2 = Math.Round(auto.SumСombustible, 2),
            //            SumExpenses2 = Math.Round(auto.SumExpenses, 2),
            //            Count2 = auto.Count,
            //            Difference = Math.Round(auto.SumСombustible + auto.SumExpenses, 2)
            //        });
            //    }
            //}

            return autoCompare;
        }


        public List<PublicServiceViewModel> GetPublicServicesDetails(int filialId, short month, short year, short monthEnd, short yearEnd)
        {
            List<PublicServiceViewModel> publicServiceViewModels = new List<PublicServiceViewModel>();

            var publicServices = _userCommands.GetPublicServicesByFilial(_ade, filialId, month, year, monthEnd, yearEnd);
            var publicServicesSubtractions = _userCommands.GetPublicServicesSubtractionsByFilial(_ade, filialId, month, year, monthEnd, yearEnd);

            foreach (var publicService in publicServices)
            {
                var i = publicServicesSubtractions.FindIndex(p => p.PublicServiceTypeId == publicService.PublicServiceTypeId && p.Date.Date == publicService.Date.Date);

                if (i != -1)
                    publicService.Sum -= publicServicesSubtractions[i].Sum;
            }

            var publicServicesGroupFact = publicServices
                                            .GroupBy(n => n.PublicServiceTypeId)
                                               .Select(n => new
                                               {
                                                   Name = n.First().PublicServiceType.Name,
                                                   Sum = n.Sum(c => c.Sum),
                                                   VAT = n.First().PublicServiceType.VAT,
                                                   Сonsignment = n.Select(s => s.Сonsignment).Aggregate((a, b)
                                                     =>
                                                   {
                                                       if (a != null && b != null)
                                                           return (a + "; " + b);
                                                       else
                                                           return "";
                                                   })
                                               }).ToList();

            var publicServicesPlan = _userCommands.GetPublicServicesPlansByFilial(_ade, filialId, month, year, monthEnd, yearEnd);
            var publicServicesGroupPlan = publicServicesPlan
                                            .GroupBy(n => n.PublicServiceTypeId)
                                               .Select(n => new
                                               {
                                                   Name = n.First().PublicServiceType.Name,
                                                   Sum = n.Sum(c => c.Sum),
                                               }).ToList();

            foreach (var psf in publicServicesGroupFact)
            {
                var ps = new PublicServiceViewModel();

                ps.Name = psf.Name;
                ps.SumFact = psf.Sum;
                ps.VAT = psf.VAT;
                ps.Сonsignment = psf.Сonsignment;
                var psP = publicServicesGroupPlan.Find(p => p.Name == psf.Name);
                ps.SumPlan = psP != null ? psP.Sum : 0;
                if (psP != null) publicServicesGroupPlan.Remove(psP);

                //if (ps.SumFact == 0 || ps.SumPlan == 0)
                //    continue;

                publicServiceViewModels.Add(ps);
            }

            if (publicServicesGroupPlan.Count > 0)
            {
                foreach (var psP in publicServicesGroupPlan)
                {
                    var ps = new PublicServiceViewModel();

                    ps.Name = psP.Name;
                    ps.SumPlan = psP.Sum;
                    ps.SumFact = 0;

                    if (ps.SumPlan > 0)
                        publicServiceViewModels.Add(ps);
                }
            }

            return publicServiceViewModels;

            //return publicServices
            //        .GroupBy(n => n.PublicServiceTypeId)
            //           .Select(n => new PublicServiceViewModel
            //           {
            //               Name = n.First().PublicServiceType.Name,
            //               Sum = n.Sum(c => c.Sum),
            //               VAT = n.First().PublicServiceType.VAT,
            //               Сonsignment = n.Select(s=>s.Сonsignment).Aggregate((a, b) 
            //               => {
            //                   if (a != null && b != null)
            //                       return (a + "; " + b);
            //                   else
            //                       return "";
            //               })
            //           }).ToList();
        }


        public List<GoodDetailViewModel> GetGoodsDetails(int filialId, string brendName, short month, short year, short monthEnd, short yearEnd)
        {
            return _userCommands.GetGoodsDetailsByBrend(_ade, filialId, brendName, month, year, monthEnd, yearEnd);
        }

        //public void UploadAvatar(System.IO.Stream inputStream, int contentLength)
        //{
        //    var userId = _contextService.GetCurrentUserId();
        //    var user = GetUserById(userId);

        //    using (var reader = new System.IO.BinaryReader(inputStream))
        //    {
        //        user.Avatar = reader.ReadBytes(contentLength);
        //    }

        //    _ade.SaveChanges();
        //}

        public void UploadAvatar(string avatarPath)
        {
            var userId = _contextService.GetCurrentUserId();
            var user = GetUserById(userId);

            RemoveOldAvatarFromServer(user.AvatarPath);

            user.AvatarPath = avatarPath;
            _ade.SaveChanges();
        }

        public UserViewModel GetUserViewModel()
        {
            var userId = _contextService.GetCurrentUserId();
            var user = GetUserById(userId);



            UserViewModel userViewModel = new UserViewModel();
            userViewModel.UserId = user.UserId;
            userViewModel.Name = user.UserName;
            userViewModel.Age = 30;
            userViewModel.City = user.DepartmentObj.Name;
            userViewModel.TextStatus = "All is OK";
            userViewModel.IsMy = true;
            userViewModel.IsInMaskMode = false;
            userViewModel.MaskUrl = "";
            userViewModel.ImageIsNull = false;
            userViewModel.IsDeleted = false;
            userViewModel.IsAskAvatar = false;

            if (string.IsNullOrEmpty(user.AvatarPath))
            {
                userViewModel.ImageUrlNormal = user.Sex ? "/Content/Uploads/male-blank.gif" : "/Content/Uploads/female-blank.gif";
                userViewModel.ImageIsNull = true;
            }
            else
            {
                userViewModel.ImageUrlNormal = user.AvatarPath;
            }

            WebImage userAvatar = new WebImage(@"~\" + userViewModel.ImageUrlNormal);
            userViewModel.ImagetHeight = userAvatar.Height;

            return userViewModel;
        }


        public static Image ResizeImage(Image image, int minSize, bool stretchIfSmall = false)
        {
            if ((image.Height <= minSize && image.Width <= minSize && !stretchIfSmall))
                return (Image)image.Clone();

            int destWidth = minSize, destHeight = minSize;

            if (image.Height < image.Width)
                destWidth = (int)(image.Width * (minSize / (float)image.Height));
            else
                destHeight = (int)(image.Height * (minSize / (float)image.Width));

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(image, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

        public void ResizeSaveImage(Stream imageStream, string fileName)
        {
            string filePath = "/Content/Uploads/" + Guid.NewGuid().ToString() + "_" + fileName;
            WebImage im = new WebImage(imageStream);
            var newIm = ResizeImage(im, 258);
            newIm.Save(@"~\" + filePath);

            UploadAvatar(filePath);
        }

        private void RemoveOldAvatarFromServer(string avatarPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(avatarPath))
                {
                    string fullPath = HostingEnvironment.MapPath("~" + avatarPath);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            catch (Exception e)
            {
                
            }
        }

        public WebImage ResizeImage(WebImage image, int minSize, bool stretchIfSmall = false)
        {
            if ((image.Height <= minSize && image.Width <= minSize && !stretchIfSmall))
                return (WebImage)image.Clone();

            int destWidth = minSize, destHeight = minSize;

            if (image.Height < image.Width)
                destWidth = (int)(image.Width * (minSize / (float)image.Height));
            else
                destHeight = (int)(image.Height * (minSize / (float)image.Width));


            var newImage = image.Resize(width: destWidth, height: destHeight);
            //,preserveAspectRatio: true,
            // preventEnlarge: true);

            return (WebImage)newImage;
        }


        public History GetLastHistoryBySourceAction(ActionHistoryType action, SourceHistoryType source)
        {
            return _userCommands.GetLastHistoryBySourceAction(_ade, action, source);
        }

    }
}
