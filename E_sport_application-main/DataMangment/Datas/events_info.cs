using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMangment.Datas
{
    public class events_info
    {
        public int EventID { get; set; }
        public string EventName{ get; set; } = string.Empty;
        public string EventLocation{ get; set; } = string.Empty;
        public DateTime EventDate{ get; set; }


    }
}
