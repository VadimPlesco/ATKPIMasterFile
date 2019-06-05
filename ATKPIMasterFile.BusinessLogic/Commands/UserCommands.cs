using ATKPIMasterFile.BusinessLogic.Model;
using ATKPIMasterFile.BusinessLogic.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.Commands
{
    public partial class UserCommands
    {
        public UserCommands()
        {

        }

        public User GeUserById(ATDbEntities atDbEntities, long userId)
        {
            return atDbEntities.Users.Include("FilialObj").Include("DepartmentObj").FirstOrDefault(p => p.UserId == userId);
        }

        public User GetUser(ATDbEntities atDbEntities, string email, string password)
        {
            return atDbEntities.Users.FirstOrDefault(p => p.Email == email && p.Password == password);
        }

        public List<Filial> GetAllFilials(ATDbEntities atDbEntities)
        {
            
            //return atDbEntities.Filials.Where(p=>p.FilialId < 9).ToList();
            return atDbEntities.Filials.Where(p => p.ShowMode == 1).ToList();
        }

        public List<Filial> GetAllFilialsForReport(ATDbEntities atDbEntities)
        {
            //return atDbEntities.Filials.Where(p => p.FilialId < 10).ToList();
            return atDbEntities.Filials.Where(p => p.ShowMode == 1 || p.ShowMode == 2).ToList();
        }

        public List<Department> GetDepartmentsByFilialId(ATDbEntities atDbEntities, int filialId)
        {
            return atDbEntities.Departments.Where(p => p.FilialId == filialId).ToList();
        }

        public List<PostByFilial> GetPostsByDepartmentId(ATDbEntities atDbEntities, int departmentId)
        {
            return atDbEntities.PostsByFilials.Where(p => p.DepartmentId == departmentId).ToList();
        }

        public List<PostByFilial> GetPostsByFilialId(ATDbEntities atDbEntities, int filialtId)
        {
            return atDbEntities.PostsByFilials.Where(p => p.FilialId == filialtId).ToList();
        }

        public List<PostByFilial> GetPosts(ATDbEntities atDbEntities)
        {
            return atDbEntities.PostsByFilials.ToList();
        }

        public List<Project> GetProjects(ATDbEntities atDbEntities)
        {
            return atDbEntities.Projects.ToList();
        }

        public List<UserInRole> GeUserRoleById(ATDbEntities atDbEntities, long userId)
        {
            return atDbEntities.UsersInRoles.Include("Role").Where(p => p.UserId == userId).ToList();
        }

        public List<Salary> GetSalariesByFilial(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd, int departmentId = 0)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            if (departmentId > 0)
                return atDbEntities.Salaries.Where(p => p.FilialId == filialId && p.DepartmentId == departmentId && p.Date >= startD && p.Date <= endD).ToList();

            return atDbEntities.Salaries.Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD).ToList();
        }

        public List<Salary> GetSalariesIn(ATDbEntities atDbEntities, short month, short year, short monthEnd, short yearEnd, List<string> persons)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            return atDbEntities.Salaries.Where(p => p.Date >= startD && p.Date <= endD && persons.Contains(p.Employee.Trim())).ToList();
        }

        public List<Salary> GetSalariesByPost(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd, string post, int departmentId = 0)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            if (departmentId > 0)
                return atDbEntities.Salaries.Where(p => p.FilialId == filialId && p.DepartmentId == departmentId && p.Date >= startD && p.Date <= endD && p.Post == post).OrderBy(p => p.Employee).ToList();

            return atDbEntities.Salaries.Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD && p.Post == post).OrderBy(p=>p.Employee).ToList();
        }

        public List<SalaryPlan> GetSalariesPlansByFilial(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd, int departmentId = 0)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            if (departmentId > 0)
                return atDbEntities.SalariesPlans.Where(p => p.FilialId == filialId && p.DepartmentId == departmentId && p.Date >= startD && p.Date <= endD).ToList();

            return atDbEntities.SalariesPlans.Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD).ToList();
        }

        public List<SalaryCommonPlan> GetSalaryCommonPlansByFilial(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd, int departmentId = 0)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            if (departmentId > 0)
                return atDbEntities.SalaryCommonPlans.Where(p => p.FilialId == filialId && p.DepartmentId == departmentId && p.Date >= startD && p.Date <= endD).ToList();

            return atDbEntities.SalaryCommonPlans.Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD).ToList();
        }

        public List<Auto> GetAutosByFilial(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd, int departmentId = 0)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            if (departmentId > 0)
                return atDbEntities.Autos.Where(p => p.FilialId == filialId && p.DepartmentId == departmentId && p.Date >= startD && p.Date <= endD).ToList();

            return atDbEntities.Autos.Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD).ToList();
        }

        public List<Auto> GetAutos(ATDbEntities atDbEntities, int filialId, int departmentId, short month, short year, short monthEnd, 
            short yearEnd, short combustibleType, short autoType, string number, int project)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            //if (departmentId > 0)
            //    return atDbEntities.Autos.Include("Filial").Include("Department").Where(p => p.FilialId == filialId && p.DepartmentId == departmentId && p.Date >= startD && p.Date <= endD).ToList();

            var stocks = atDbEntities.Autos.Include("Filial").Include("Department")
                .Where(p => p.Date >= startD && p.Date <= endD)
                .AsQueryable();

            if (filialId > 0) stocks = stocks.Where(s => s.FilialId == filialId);
            if (departmentId > 0) stocks = stocks.Where(s => s.DepartmentId == departmentId);
            if (combustibleType >= 0) stocks = stocks.Where(s => s.Сombustible == (СombustibleType)combustibleType);
            if (autoType >= 0) stocks = stocks.Where(s => s.Type == (AutoType)autoType);
            if (number.Trim() != string.Empty) stocks = stocks.Where(s => s.Number.Contains(number.Trim()));
            if (project > 0) stocks = stocks.Where(s => s.ProjectId == project);

            //return atDbEntities.Autos.Include("Filial").Include("Department")
            //    .Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD)
            //    .Where(s => s.DepartmentId == departmentId || departmentId <= 0)
            //    .Where(s => s.Сombustible == (СombustibleType)combustibleType || combustibleType < 0)
            //    .ToList();

            return stocks.ToList();
        }

        public List<string> GetAutoNumbers(ATDbEntities atDbEntities, string query)
        {
            return atDbEntities.Autos
                    .Where(p => p.Number.Contains(query))
                    .Select(p=>p.Number).Distinct()
                    .Take(15)
                    .ToList();
        }


        public List<string> GetEmployees(ATDbEntities atDbEntities, string query)
        {
            return atDbEntities.Salaries
                    .Where(p => p.Employee.Contains(query))
                    .Select(p => p.Employee).Distinct()
                    .Take(15)
                    .ToList();
        }


        public List<Salary> GetSalaries(ATDbEntities atDbEntities, int filialId, int departmentId, short month, short year, short monthEnd,
            short yearEnd, short personType, int project, string post, string employee)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            var stocks = atDbEntities.Salaries.Include("Filial").Include("Department")
                .Where(p => p.Date >= startD && p.Date <= endD)
                .AsQueryable();

            if (filialId > 0) stocks = stocks.Where(s => s.FilialId == filialId);
            if (departmentId > 0) stocks = stocks.Where(s => s.DepartmentId == departmentId);
            if (personType >= 0) stocks = stocks.Where(s => s.Type == (PersonType)personType);
            if (project > 0) stocks = stocks.Where(s => s.ProjectId == project);
            if (employee.Trim() != string.Empty) stocks = stocks.Where(s => s.Employee.Contains(employee.Trim()));
            if (post != "- Все - ") stocks = stocks.Where(s => s.Post == post);

            //return atDbEntities.Autos.Include("Filial").Include("Department")
            //    .Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD)
            //    .Where(s => s.DepartmentId == departmentId || departmentId <= 0)
            //    .Where(s => s.Сombustible == (СombustibleType)combustibleType || combustibleType < 0)
            //    .ToList();

            return stocks.ToList();
        }


        public List<AutoPlan> GetAutosPlansByFilial(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd, int departmentId = 0)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            if (departmentId > 0)
                return atDbEntities.AutosPlans.Where(p => p.FilialId == filialId && p.DepartmentId == departmentId && p.Date >= startD && p.Date <= endD).ToList();

            return atDbEntities.AutosPlans.Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD).ToList();
        }

        public List<Auto> GetAutosByType(ATDbEntities atDbEntities, AutoType type, int filialId, short month, short year, short monthEnd, short yearEnd, int departmentId = 0)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            if (departmentId > 0)
                if (type == AutoType.All)
                    return atDbEntities.Autos.Where(p => p.FilialId == filialId && p.DepartmentId == departmentId && p.Date >= startD && p.Date <= endD).OrderBy(p=>p.Number).ToList();
                else
                    return atDbEntities.Autos.Where(p => p.FilialId == filialId && p.DepartmentId == departmentId && p.Date >= startD && p.Date <= endD && p.Type == type).OrderBy(p => p.Number).ToList();

            return type == AutoType.All ? atDbEntities.Autos.Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD).OrderBy(p => p.Number).ToList()
                : atDbEntities.Autos.Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD && p.Type == type).OrderBy(p => p.Number).ToList();
        }

        public List<BrendsPlanViewModel> GetBrendsPlansByFilial(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);
            //return atDbEntities.BrendsPlans.Include("Brend")
            //    .Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD)
            //    .OrderBy(p=>p.Brend.OrderByFlag)
            //    .GroupBy(p=>p.BrendId)
            //    .SelectMany(gr => gr)
            //    .ToList();

            return atDbEntities.BrendsPlans.Include("Brend")
               .Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD)// && p.BrendId != 10 && p.BrendId != 11 && p.BrendId != 20)
               .GroupBy(p => new { p.BrendId, p.Brend.Name, p.Brend.OrderByFlag })
                .Select(n => new BrendsPlanViewModel
                {
                     BrendId = n.Key.BrendId,
                     BrendName = n.Key.Name,
                     OrderByFlag = n.Key.OrderByFlag,
                     Sum = n.Sum(c => c.Sum)
                }
                )
                .OrderBy(p => p.OrderByFlag)
                .ToList();
        }


        public List<GoodViewModel> GetGoodsByFilial(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);
            return atDbEntities.Goods.Include("Brend")
                .Where(p => p.FilialId == filialId && p.Date >=startD && p.Date <= endD )
                .GroupBy(p => new { p.BrendId, p.Brend.Name })
                .Select(n => new GoodViewModel
                {
                    BrendId = n.Key.BrendId,
                    BrendName = n.Key.Name,
                    Price = n.Sum(c => c.Price),
                    PrimeCost = n.Sum(c => c.PrimeCost),
                    Sum = n.Sum(c => c.Sum)
                }
                )
                .ToList();
            //return atDbEntities.Goods.Include("Brend")
            //    .Where(p => p.FilialId == filialId && p.Month == month && p.Year == year)
            //    .GroupBy(p => new { p.BrendId, p.Brend.Name })
            //    .Select(n => new GoodViewModel
            //    {
            //        BrendId = n.Key.BrendId,
            //        BrendName = n.Key.Name,
            //        Price = n.Sum(c => c.Price),
            //        PrimeCost = n.Sum(c => c.PrimeCost),
            //        Sum = n.Sum(c => c.Sum)
            //    }
            //    )
            //    .ToList();
        }

        public List<GoodViewModel> GetGoodsAERPVodovozByFilial(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd)
        {
            List<GoodViewModel> goodViewModelVodovoz = new List<GoodViewModel>();

            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);
            var filialV = GetVodovozFilialId(filialId);
            var filialData = atDbEntities.GoodsAquaERP.Where(p => p.FilialId == filialV && p.Date >= startD && p.Date <= endD).ToList();

            if (filialData.Count == 0)
                return goodViewModelVodovoz;

            var filialData19l = filialData.Where(p => p.Capacity == "19l");
            goodViewModelVodovoz.Add(
                new GoodViewModel
                {
                    BrendId = 10,
                    BrendName = "Вода (OM, Familia, Populara)19л",
                    Price = filialData19l.Select(x=>x.Price).Sum(),
                    PrimeCost = filialData19l.Select(x => x.PrimeCost).Sum(),
                    Sum = filialData19l.Select(c => c.Sum ?? 0).Sum()
                });

            var filialDataCuler = filialData.Where(p => p.BrendId == 20);
            goodViewModelVodovoz.Add(
                new GoodViewModel
                {
                    BrendId = 20,
                    BrendName = "Кулера",
                    Price = filialDataCuler.Select(x => x.Price).Sum(),
                    PrimeCost = filialDataCuler.Select(x => x.PrimeCost).Sum(),
                    Sum = filialDataCuler.Select(c => c.Sum ?? 0).Sum()
                });


            goodViewModelVodovoz.Add(
                new GoodViewModel
                {
                    BrendId = 11,
                    BrendName = "Доп. водавоз",
                    Price = filialData.Select(x => x.Price).Sum(),
                    PrimeCost = filialData.Select(x => x.PrimeCost).Sum(),
                    Sum = filialData.Select(c => c.Sum ?? 0).Sum()
                });

            goodViewModelVodovoz[2].Price = goodViewModelVodovoz[2].Price - goodViewModelVodovoz[0].Price - goodViewModelVodovoz[1].Price;
            goodViewModelVodovoz[2].PrimeCost = goodViewModelVodovoz[2].PrimeCost - goodViewModelVodovoz[0].PrimeCost - goodViewModelVodovoz[1].PrimeCost;
            goodViewModelVodovoz[2].Sum = goodViewModelVodovoz[2].Sum - goodViewModelVodovoz[0].Sum - goodViewModelVodovoz[1].Sum;

            return goodViewModelVodovoz;
        }

        private int GetVodovozFilialId(int filialId)
        {
            int filialVodovoz = 0;

            switch (filialId)
            {
                case 2:
                    filialVodovoz = 9;
                    break;
                case 3:
                    filialVodovoz = 11;
                    break;
                case 4:
                    filialVodovoz = 10;
                    break;
                case 5:
                    filialVodovoz = 12;
                    break;
                case 6:
                    filialVodovoz = 13;
                    break;
                case 7:
                    filialVodovoz = 14;
                    break;
                default:
                    break;
            }
            return filialVodovoz;
        }

        public List<GoodDetailViewModel> GetGoodsDetailsByBrend(ATDbEntities atDbEntities, int filialId, string brendName, short month, short year, short monthEnd, short yearEnd)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            if(brendName == "Пакеты + Стаканчики")
                return atDbEntities.Goods.Include("Brend")
                .Where(p => p.FilialId == filialId && (p.Brend.Name == "Shanghai (Пакеты)" || p.Brend.Name == "Comecotech(Пакеты, Стаканчики)" || p.Brend.Name == "MaxiPac") && p.Date >= startD && p.Date <= endD)
                .GroupBy(p => new { p.Name, p.Capacity })
                .Select(n => new GoodDetailViewModel
                {
                    Name = n.Key.Name,
                    Capacity = n.Key.Capacity,
                    Amount = n.Sum(c=> c.Amount),
                    Sum = n.Sum(c => c.Sum)
                })
                .OrderBy(p => p.Name).ThenBy(n => n.Capacity)
                .ToList();

            return atDbEntities.Goods.Include("Brend")
                .Where(p => p.FilialId == filialId && p.Brend.Name == brendName && p.Date >= startD && p.Date <= endD)
                .GroupBy(p => new { p.Name, p.Capacity })
                .Select(n => new GoodDetailViewModel
                {
                    Name = n.Key.Name,
                    Capacity = n.Key.Capacity,
                    Amount = n.Sum(c => c.Amount),
                    Sum = n.Sum(c => c.Sum)
                })
                .OrderBy(p => p.Name).ThenBy(n => n.Capacity)
                .ToList();
        }

        public List<PublicService> GetPublicServicesByFilial(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd, int departmentId = 0)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            if (departmentId > 0)
                return atDbEntities.PublicServices.Include("PublicServiceType").Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD && p.DepartmentId == departmentId).ToList();

            return atDbEntities.PublicServices.Include("PublicServiceType").Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD).ToList();
        }

        public List<PublicServicePlan> GetPublicServicesPlansByFilial(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd, int departmentId = 0)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            if (departmentId > 0)
                return atDbEntities.PublicServicesPlans.Include("PublicServiceType").Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD && p.DepartmentId == departmentId).ToList();

            return atDbEntities.PublicServicesPlans.Include("PublicServiceType").Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD).ToList();
        }

        public List<PublicServiceSubtraction> GetPublicServicesSubtractionsByFilial(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd, int departmentId = 0)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            if (departmentId > 0)
                return atDbEntities.PublicServicesSubtractions.Include("PublicServiceType").Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD && p.DepartmentId == departmentId).ToList();

            return atDbEntities.PublicServicesSubtractions.Include("PublicServiceType").Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD).ToList();
        }

        public List<DefectDiscount> GetPublicDefectDiscountByFilial(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd, int departmentId = 0)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            if (departmentId > 0)
                return atDbEntities.DefectsDiscounts.Where(p => p.FilialId == filialId && p.DepartmentId == departmentId && p.Date >= startD && p.Date <= endD).ToList();

            return atDbEntities.DefectsDiscounts.Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD).ToList();
        }

        public List<DefectDiscountPlan> GetPublicDefectDiscountPlanByFilial(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd, int departmentId = 0)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            if (departmentId > 0)
                return atDbEntities.DefectsDiscounstPlans.Where(p => p.FilialId == filialId && p.DepartmentId == departmentId && p.Date >= startD && p.Date <= endD).ToList();

            return atDbEntities.DefectsDiscounstPlans.Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD).ToList();
        }

        public List<AdminExpense> GetPublicAdminExpenseByFilial(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd, int departmentId = 0)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            if (departmentId > 0)
                return atDbEntities.AdminExpenses.Where(p => p.FilialId == filialId && p.DepartmentId == departmentId && p.Date >= startD && p.Date <= endD).ToList();

            return atDbEntities.AdminExpenses.Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD).ToList();
        }

        public List<AdminExpensePlan> GetPublicAdminExpensePlanPlanByFilial(ATDbEntities atDbEntities, int filialId, short month, short year, short monthEnd, short yearEnd, int departmentId = 0)
        {
            var startD = new DateTime(year, month, 1);
            var endD = new DateTime(yearEnd, monthEnd, 1);

            if (departmentId > 0)
                return atDbEntities.AdminExpensesPlans.Where(p => p.FilialId == filialId && p.DepartmentId == departmentId && p.Date >= startD && p.Date <= endD).ToList();

            return atDbEntities.AdminExpensesPlans.Where(p => p.FilialId == filialId && p.Date >= startD && p.Date <= endD).ToList();
        }


        public History GetLastHistoryBySourceAction(ATDbEntities atDbEntities, ActionHistoryType action, SourceHistoryType source)
        {
            return atDbEntities.Histories.Where(p => p.Action == action && p.Source == source).OrderByDescending(p => p.Date).FirstOrDefault();
        }


    }
}
