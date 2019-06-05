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
    [Table("DefectsDiscounts")]
    public class DefectDiscount
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long DefectDiscountId { get; set; }

        public int FilialId { get; set; }
        [ForeignKey("FilialId")]
        public virtual Filial Filial { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        public short Month { get; set; }

        public short Year { get; set; }

        public double Defect { get; set; }

        public double? DefectAdditional { get; set; }

        public double? Discount { get; set; }

        public double? DiscountAdditional { get; set; }

        public double? PrimeCost { get; set; }

        public double EventDistribution { get; set; }

        public double Representation { get; set; }

        public DateTime Date { get; set; }
    }
}
