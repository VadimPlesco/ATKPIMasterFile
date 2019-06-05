using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.Model
{
    [Table("PublicServiceTypes")]
    public class PublicServiceType
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PublicServiceTypeId { get; set; }

        public string Name { get; set; }

        public double VAT { get; set; }

    }
}
