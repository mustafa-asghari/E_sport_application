using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataMangment.Datas
{
    public class result_info
    {
        public int ResultID { get; set; }
        public int EventID { get; set; }
        public int GameID { get; set; }
        public int TeamID { get; set; }
        public int OpposingTeamID { get; set; }
        public string Result { get; set; } = string.Empty;

    }

}