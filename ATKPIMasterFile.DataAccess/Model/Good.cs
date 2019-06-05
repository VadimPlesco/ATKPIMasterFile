using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.Model
{
    [Table("Goods")]
    public class Good
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long GoodId { get; set; }

        public string Name { get; set; }

        public string Article { get; set; }

        public short Month { get; set; }

        public short Year { get; set; }

        public int Amount { get; set; }

        public double Sum { get; set; }

        public long BrendId { get; set; }
        [ForeignKey("BrendId")]
        public virtual Brend Brend { get; set; }

        public string Capacity { get; set; }

        public double? Price { get; set; }

        public double? PrimeCost { get; set; }

        public int FilialId { get; set; }
        [ForeignKey("FilialId")]
        public virtual Filial Filial { get; set; }

        public DateTime Date { get; set; }

    }
}
