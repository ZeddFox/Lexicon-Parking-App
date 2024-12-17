namespace Lexicon_Parking_App
{
    public class Period
    {
        public int AccountID { get; set; }
        public DateTime StartDate { get; set; }

        public Period(int accountid, DateTime startDate)
        {
            AccountID = accountid;
            StartDate = startDate;
        }
    }
}
