using System.Diagnostics;

namespace Lexicon_Parking_App
{
    public class Period
    {
        public int PeriodId { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal PeriodCost { get; set; } = 0m;

        // Create new period
        public Period(int accountid, DateTime startDate)
        {
            UserId = accountid;
            StartTime = startDate;
            EndTime = null;
            PeriodId = Backend.UniquePeriodId();
        }

        // Load Period from XML
        public Period(int periodid, int userid, DateTime starttime, DateTime endtime, decimal cost)
        {
            PeriodId = periodid;
            UserId = userid;
            StartTime = starttime;
            EndTime = endtime;
            PeriodCost = cost;
        }

        public void Stop()
        {
            EndTime = DateTime.Now;
        }
    }
}
