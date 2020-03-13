using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.ViewModels.User
{
    public class AutoGroupedBy
    {
        public double SumСombustible { get; set; }

        public double SumExpenses { get; set; }

        public string Name { get; set; }

        public int Count { get; set; }

        public double Weight { get; set; }

    }


    public class AutoCompareViewModel
    {
        public string Name1 { get; set; }

        public double SumСombustible1 { get; set; }

        public double SumExpenses1 { get; set; }

        public int Count1 { get; set; }

        public double Weight1 { get; set; }

        public string Delimiter { get; set; }

        public string Name2 { get; set; }

        public double SumСombustible2 { get; set; }

        public double SumExpenses2 { get; set; }

        public int Count2 { get; set; }

        public double Weight2 { get; set; }

        public double Difference { get; set; }

    }
}
