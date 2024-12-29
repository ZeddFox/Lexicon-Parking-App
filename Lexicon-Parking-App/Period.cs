namespace Lexicon_Parking_App
{
    public class Period
    {
        public int AccountID { get; set; }
        public DateTime StartDate { get; set; }
        public string Licenseplate { get; set; }

        public Period(int accountid, string licenseplate, DateTime startDate)
        {
            AccountID = accountid;
            StartDate = startDate;
            Licenseplate = licenseplate;
        }
    }
}
