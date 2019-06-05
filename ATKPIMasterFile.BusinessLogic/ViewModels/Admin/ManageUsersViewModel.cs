using ATKPIMasterFile.BusinessLogic.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.ViewModels.Admin
{
    public class ManageUsersViewModel
    {
        public List<ManageUsersViewModel> ManageUsers { get; set; }


        public long UserId { get; set; }

        [DisplayName("Имя")]
        [Required(ErrorMessage = "Укажите имя")]
        public string Name { get; set; }

        [DisplayName("Почта")]
        [Required(ErrorMessage = "Укажите почту")]
        public string Email { get; set; }

        [DisplayName("Пароль")]
        [Required(ErrorMessage = "Укажите пароль")]
        public string Password { get; set; }


        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public List<Department> Departments { get; set; }

        public int FilialId { get; set; }
        public string Filial { get; set; }
        public List<Filial> Filials { get; set; }

        public List<UserInRole> Roles { get; set; }

    }

    public class ManageUserViewModel
    {
        public long UserId { get; set; }
    }
}
