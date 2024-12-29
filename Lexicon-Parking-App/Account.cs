namespace Lexicon_Parking_App
{
    public class Account
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Licenseplate { get; set; }
        public decimal Balance { get; set; }
        public bool ActivePeriod { get; set; }

        // Create new account
        public Account(string username, string password, string firstname, string lastname, string licenseplate)
        {
            Username = username;
            Password = password;
            Firstname = firstname;
            Lastname = lastname;
            Licenseplate = licenseplate;
            Balance = 0;
            ActivePeriod = false;

            ID = Backend.UniqueId();
        }

        // Create account type from file
        public Account(int id, string username, string password, string firstname, string lastname, string licenseplate, decimal balance, bool activeperiod)
        {
            ID = id;
            Username = username;
            Password = password;
            Firstname = firstname;
            Lastname = lastname;
            Licenseplate = licenseplate;
            Balance = balance;
            ActivePeriod = activeperiod;
        }
    }
}
