using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.ViewModels.User
{
    public class GoodViewModel
    {
        public double Sum { get; set; }

        public long BrendId { get; set; }
        
        public string BrendName { get; set; }

        public double? Price { get; set; }

        public double? PrimeCost { get; set; }
    }

    public class GoodDetailViewModel
    {
        public double Sum { get; set; }

        public string Name { get; set; }

        public string Capacity { get; set; }

        public int Amount { get; set; }

    }
}
