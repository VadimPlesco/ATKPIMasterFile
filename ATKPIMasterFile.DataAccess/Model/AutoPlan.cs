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
   
    [Table("AutosPlans")]
    public class AutoPlan
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long AutoPlanId { get; set; }

        public int FilialId { get; set; }
        [ForeignKey("FilialId")]
        public virtual Filial Filial { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        public short Month { get; set; }

        public short Year { get; set; }

        public int Car { get; set; }

        public int Truck { get; set; }

        public double Сombustible { get; set; }

        public double Tires { get; set; }

        public double Accumulators { get; set; }

        public double MaintenanceCar { get; set; }

        public double MaintenanceTruck { get; set; }

        public double Spares { get; set; }

        public double Services { get; set; }

        public double? СarWash { get; set; }

        public double? LeaseTruck { get; set; }

        public DateTime Date { get; set; }

    }
}
