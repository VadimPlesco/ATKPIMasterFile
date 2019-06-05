using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.ViewModels.User
{
    public class Post
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }


    public class KPIRow
    {
        public string Name { get; set; }
        public int Plan { get; set; }
        public int Fact { get; set; }
        public double Percent { get; set; }
        public bool Link { get; set; }
    }

    public class KPIRowT
    {
        public string Name { get; set; }
        public double Plan { get; set; }
        public double Fact { get; set; }
        public double Percent { get; set; }
    }

    public class KPIAutoPartRow
    {
        public List<KPIRow> Rows { get; set; }
    }

    public class KPISalePartRow
    {
        public List<KPIRow> Rows { get; set; }
    }

    public class KPISalaryPartRow
    {
        public List<KPIRow> Rows { get; set; }
    }

    public class KPIAdminPartRow
    {
        public List<KPIRow> Rows { get; set; }
    }

    public class KPITotalPartRow
    {
        public List<KPIRowT> Rows { get; set; }
    }

    public class KPIViewModel
    {
        //public List<KPIRow> Rows { get; set; }
        public KPISalaryPartRow SalaryPart { get; set; }
        public KPISalaryPartRow SalaryPartPast { get; set; }

        public KPIAutoPartRow AutoPart { get; set; }
        public KPIAutoPartRow AutoPartPast { get; set; }

        public KPISalePartRow SalePart { get; set; }

        public KPIAdminPartRow AdminPart { get; set; }
        public KPIAdminPartRow AdminPartPast { get; set; }

        public KPITotalPartRow TotalPart { get; set; }
    }

}
