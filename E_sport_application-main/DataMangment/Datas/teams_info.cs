using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataMangment.Datas
{
    // This class maps to the 'Team' table in your SQL script.
    // The DataAdapter will use SQL aliases to match these property names.
    public class teams_info
    {
        public int Team_id { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string PrimaryContact { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public int CompetitionPoints { get; set; }
        public string ContactEmail { get; set; } = string.Empty;
    }
}