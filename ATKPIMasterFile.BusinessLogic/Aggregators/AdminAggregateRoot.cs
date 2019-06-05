using ATKPIMasterFile.BusinessLogic.Commands;
using ATKPIMasterFile.BusinessLogic.Model;
using ATKPIMasterFile.BusinessLogic.ViewModels.Admin;
using ATKPIMasterFile.BusinessLogic.ViewModels.User;
using ATKPIMasterFile.BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.Aggregators
{
    public class AdminAggregateRoot
    {
        private readonly ATDbEntities _ade;
        private readonly AdminCommands _adminCommands;
        private readonly WebContextService _contextService;

        public AdminAggregateRoot(ATDbEntities ade, AdminCommands adminCommands, WebContextService contextService)
        {
            _ade = ade;
            _adminCommands = adminCommands;
            _contextService = contextService;
        }


        public ManageUsersViewModel GetAllUsers()
        {
            var users = _adminCommands.GetAllUsers(_ade);
            ManageUsersViewModel manageUsers = new ManageUsersViewModel();
            List<ManageUsersViewModel> manageUsersList = new List<ManageUsersViewModel>();
            foreach (var user in users)
            {
                manageUsersList.Add(new ManageUsersViewModel
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Name = user.UserName,
                    Password = user.Password,
                    DepartmentId = user.Department,
                    Department = user.DepartmentObj.Name,
                    FilialId = user.Filial,
                    Filial = user.FilialObj.Name,
                    Roles = _adminCommands.GetUserRoles(_ade, user.UserId)
                });
            }

            manageUsers.ManageUsers = manageUsersList;

            return manageUsers;
        }

        public long GetCurrentUserId()
        {
            return _contextService.GetCurrentUserId();
        }

    }
}
