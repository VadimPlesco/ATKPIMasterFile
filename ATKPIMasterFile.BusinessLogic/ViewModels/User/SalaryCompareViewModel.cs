using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.ViewModels.User
{
    public class SalariesGroupedByDepartment
    {
        public double SumSal { get; set; }

        public double SumVac { get; set; }

        public double SumTax { get; set; }

        public string Name { get; set; }

        public int Count { get; set; }

    }


    public class SalaryCompareViewModel
    {
        public string Name1 { get; set; }

        public double SumSal1 { get; set; }

        public double SumVac1 { get; set; }

        public double SumTax1 { get; set; }

        public int Count1 { get; set; }

        public string Delimiter { get; set; }

        public string Name2 { get; set; }

        public double SumSal2 { get; set; }

        public double SumVac2 { get; set; }

        public double SumTax2 { get; set; }

        public int Count2 { get; set; }

        public double Difference { get; set; }

    }
}
