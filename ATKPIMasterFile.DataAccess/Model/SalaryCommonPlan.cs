using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.Model
{
    [Table("SalaryCommonPlans")]
    public class SalaryCommonPlan
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SalaryPlanId { get; set; }


        public int FilialId { get; set; }
        [ForeignKey("FilialId")]
        public virtual Filial Filial { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        public short Month { get; set; }

        public short Year { get; set; }

        public double Card { get; set; }

        public double? Cash { get; set; }

        public double? Vacation { get; set; }

        public double? SickLeave { get; set; }

        public double? Tax { get; set; }

        public DateTime Date { get; set; }

    }
}
