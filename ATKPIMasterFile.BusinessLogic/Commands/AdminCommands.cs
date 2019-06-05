using ATKPIMasterFile.BusinessLogic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.Commands
{
    public partial class AdminCommands
    {
        public AdminCommands()
        {
     
        }

        public List<User> GetAllUsers(ATDbEntities atDbEntities)
        {
            return atDbEntities.Users.Include("FilialObj").Include("DepartmentObj").ToList();
        }

        public List<Filial> GetAllFilials(ATDbEntities atDbEntities)
        {
            return atDbEntities.Filials.ToList();
        }

        public Filial GetFilialById(ATDbEntities atDbEntities, int Id)
        {
            return atDbEntities.Filials.Where(p=>p.FilialId == Id).FirstOrDefault();
        }

        public List<Department> GetAllDepartments(ATDbEntities atDbEntities)
        {
            return atDbEntities.Departments.ToList();
        }

        public Department GetDepartmentById(ATDbEntities atDbEntities, int Id)
        {
            return atDbEntities.Departments.Where(p => p.DepartmentId == Id).FirstOrDefault();
        }
    
        public List<UserInRole> GetUserRoles(ATDbEntities atDbEntities, long userId)
        {
            return atDbEntities.UsersInRoles.Include("Role").Where(p=>p.UserId == userId).ToList();
        }
    }
}
