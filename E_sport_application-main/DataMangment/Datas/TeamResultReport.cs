using System;

namespace DataMangment.Datas
{
    /// <summary>
    /// This is a new class required for the reports.
    /// It holds the combined data for the "Team Results" report.
    /// </summary>
    public class TeamResultReport
    {
        public int ResultID { get; set; }
        public int EventID { get; set; }
        public int GameID { get; set; }
        public int TeamID { get; set; }
        public int OpposingTeamID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string GameName { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public string OpposingTeamName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
    }
}