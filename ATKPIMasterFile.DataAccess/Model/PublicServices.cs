﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.Model
{
    [Table("PublicServices")]
    public class PublicService
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PublicServiceId { get; set; }

        public long PublicServiceTypeId { get; set; }
        [ForeignKey("PublicServiceTypeId")]
        public virtual PublicServiceType PublicServiceType { get; set; }

        public int FilialId { get; set; }
        [ForeignKey("FilialId")]
        public virtual Filial Filial { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        public short Month { get; set; }

        public short Year { get; set; }

        public double Sum { get; set; }

        public string Сonsignment { get; set; }

        public DateTime Date { get; set; }

    }
}
