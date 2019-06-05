using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.Model
{
    public enum ActionHistoryType : byte
    {
        None,
        UpdateData
    };

    public enum SourceHistoryType : byte
    {
        None,
        AutosReport
    };

    [Table("History")]
    public class History
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long HistoryId { get; set; }

        public long UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public ActionHistoryType Action { get; set; }

        public SourceHistoryType Source { get; set; }

        public DateTime Date { get; set; }

    }
}
