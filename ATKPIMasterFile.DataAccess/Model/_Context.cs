using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.Model
{
    public class ATDbEntities : DbContext
    {
        public ATDbEntities()
            : base("ATKPIMFConnection")
        {
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Filial> Filials { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserInRole> UsersInRoles { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<SalaryPlan> SalariesPlans { get; set; }
        public DbSet<SalaryCommonPlan> SalaryCommonPlans { get; set; }
        public DbSet<Auto> Autos { get; set; }
        public DbSet<AutoPlan> AutosPlans { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Brend> Brends { get; set; }
        public DbSet<BrendPlan> BrendsPlans { get; set; }
        public DbSet<Good> Goods { get; set; }
        public DbSet<GoodAquaERP> GoodsAquaERP { get; set; }
        public DbSet<PublicServiceType> PublicServiceTypes { get; set; }
        public DbSet<PublicService> PublicServices { get; set; }
        public DbSet<PublicServicePlan> PublicServicesPlans { get; set; }
        public DbSet<PublicServiceSubtraction> PublicServicesSubtractions { get; set; }
        public DbSet<DefectDiscount> DefectsDiscounts { get; set; }
        public DbSet<DefectDiscountPlan> DefectsDiscounstPlans { get; set; }
        public DbSet<AdminExpense> AdminExpenses { get; set; }
        public DbSet<AdminExpensePlan> AdminExpensesPlans { get; set; }
        public DbSet<PostByFilial> PostsByFilials { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<History> Histories { get; set; }

        public IList<long> GetFederationRangeLowElements()
        {
            return new List<long> { 1 };
        }
    }
}
