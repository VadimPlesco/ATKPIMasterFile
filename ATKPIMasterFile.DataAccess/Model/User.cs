using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.Model
{
    [Table("Users")]
    public class User
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long UserId { get; set; }

        public string UserName { get; set; }
        
        public string Email { get; set; }

        public string Password { get; set; }

        public int Filial { get; set; }
        [ForeignKey("Filial")]
        public virtual Filial FilialObj { get; set; }

        public int Department { get; set; }
        [ForeignKey("Department")]
        public virtual Department DepartmentObj { get; set; }

        public string AvatarPath { get; set; }

        public bool Sex { get; set; }

    }
}
