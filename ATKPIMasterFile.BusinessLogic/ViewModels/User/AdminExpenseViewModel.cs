using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.ViewModels.User
{
    public class AdminExpenseGroupViewModel
    {
        public int FilialId { get; set; }

        public double PremisesRent { get; set; }

        public double MobileTelephony { get; set; }

        public double SalaryTax { get; set; }

        public double? TransportRent { get; set; }

        public double? WashTransport { get; set; }

    }
}
