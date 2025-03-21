using System.Diagnostics;

namespace Lexicon_Parking_App
{
    public class Period
    {
        public int PeriodId { get; set; }
        public int UserID { get; set; }
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }
        public decimal Cost { get; set; } = 0m;

        // Create new period
        public Period(int accountid, DateTime startDate)
        {
            UserID = accountid;
            StartTime = startDate;
            EndTime = null;
            PeriodId = Backend.UniquePeriodId();
        }

        // Load Period from XML
        public Period(int periodid, int userid, DateTime starttime, DateTime endtime, decimal cost)
        {
            PeriodId = periodid;
            UserID = userid;
            StartTime = starttime;
            EndTime = endtime;
            Cost = cost;
        }

        public void Stop()
        {
            EndTime = DateTime.Now;
        }
    }
}
