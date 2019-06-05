using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ATKPIMasterFile.BusinessLogic.Services
{
    public class WebContextService
    {

        public WebContextService()
        {
           
        }

        public bool IsAuthenticated()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        public string GetCurrentUserKey()
        {
            return HttpContext.Current.User.Identity.Name;
        }

        public long GetCurrentUserId()
        {
            if (HttpContext.Current == null || HttpContext.Current.User == null || !IsAuthenticated())
                return 0;

            long userId;
            if (!long.TryParse(HttpContext.Current.User.Identity.Name, out userId))
                throw new Exception("Cannot parse user name");

            return userId;
        }
    }
}
