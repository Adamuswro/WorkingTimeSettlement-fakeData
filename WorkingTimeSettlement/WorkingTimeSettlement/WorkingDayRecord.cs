using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npoi.Mapper.Attributes;
using NPOI;

namespace WorkingTimeSettlement
{
    public class WorkingDayRecord
    {
        [Column("Data")]
        public string Date { get; set; }
        [Column("Bilans")]
        public string Balance { get; set; }
        [Column("Do odb.")]
        public string ToRecieve { get; set; }
        [Column("Wej.")]
        public string Entrance { get; set; }
        [Column("Wyj.")]
        public string Exit { get; set; }
    }
}
