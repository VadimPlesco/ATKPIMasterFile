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
   
    public enum СombustibleType : byte
    {
        [Description("-")]
        None,
        [Description("Бензин")]
        Benzine,
        [Description("Дизель")]
        Diesel
    };

    public enum AutoType : byte
    {
        [Description("-")]
        None,
        [Description("Легковая")]
        Car,
        [Description("Грузовая")]
        Truck,
        [Description("Админ")]
        Admin,
        [Description("Магистральный")]
        Magistral,
        [Description("Дистрибьютерский")]
        Distrib,
        [Description("Все")]
        All
    };


    public enum AutoRent : byte
    {
        [Description("-")]
        None,
        [Description("Да")]
        Yes,
        [Description("Нет")]
        No
    };

    [Table("Autos")]
    public class Auto
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long AutoId { get; set; }

        public int FilialId { get; set; }
        [ForeignKey("FilialId")]
        public virtual Filial Filial { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        public short Month { get; set; }

        public short Year { get; set; }

        public string Number { get; set; }

        public string Person { get; set; }

        public string Brand { get; set; }

        public AutoType Type { get; set; }

        public СombustibleType Сombustible { get; set; }

        public AutoRent Rent { get; set; }

        public double СombustiblePrice { get; set; }

        public double Km { get; set; }

        public double Liters { get; set; }

        public double LitersWork { get; set; }

        public double Tires { get; set; }

        public double Accumulators { get; set; }

        public double TestareAuto { get; set; }

        public double Spares { get; set; }

        public double Lubricants { get; set; }

        public double Services { get; set; }

        public double Expenses { get; set; }

        public double Repairs { get; set; }

        public double? TransportServiceOther { get; set; }

        public double? Other { get; set; }

        public DateTime Date { get; set; }

        public double? Weight { get; set; }

        public int? Routes { get; set; }

        [NotMapped]
        public double SalaryCard { get; set; }

        [NotMapped]
        public double TiresAccumulators { get; set; }

        public long ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public double? KmWorkAm { get; set; }
        public double? KmPersonalAm { get; set; }
        public double? KmHomeOfficeAm { get; set; }
        public double? NormaAm { get; set; }
        public double? LitersAm { get; set; }
        public double? ForPaymentAm { get; set; }

    }
}
