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
    [Table("AdminExpenses")]
    public class AdminExpense
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long AdminExpenseId { get; set; }

        public int FilialId { get; set; }
        [ForeignKey("FilialId")]
        public virtual Filial Filial { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        public short Month { get; set; }

        public short Year { get; set; }

        public double PremisesRent { get; set; }

        public double MobileTelephony { get; set; }

        public double SalaryTax { get; set; }

        public double? TransportRent { get; set; }

        public double? WashTransport { get; set; }

        public DateTime Date { get; set; }

    }
}
