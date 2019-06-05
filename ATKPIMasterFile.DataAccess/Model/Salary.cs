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
    public enum PersonType : byte
    {
        [Description("-")]
        None,
        [Description("Обычный")]
        Usual,
        [Description("Поддержка продаж")]
        SalesSupport,
        [Description("Администрация")]
        Admin
    };

    [Table("Salaries")]
    public class Salary
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SalaryId { get; set; }

        public string Employee { get; set; }

        public int FilialId { get; set; }
        [ForeignKey("FilialId")]
        public virtual Filial Filial { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        public short Month { get; set; }

        public short Year { get; set; }

        public string Post { get; set; }

        public PersonType Type { get; set; }

        public double Card { get; set; }

        public double Cash { get; set; }

        public double Vacation { get; set; }

        public double SickLeave { get; set; }

        public DateTime Date { get; set; }

        public string CFR { get; set; }

        public long ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

    }
}
