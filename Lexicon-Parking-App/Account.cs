namespace Lexicon_Parking_App
{
    public class Account
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Car { get; set; }
        public decimal Balance { get; set; }
        public bool ActivePeriod { get; set; }

        // Create new account
        public Account(string username, string password, string firstname, string lastname, string car)
        {
            Username = username;
            Password = password;
            Firstname = firstname;
            Lastname = lastname;
            Car = car;
            Balance = 0;
            ActivePeriod = false;

            ID = Backend.UniqueId();
        }

        // Create account type from file
        public Account(int id, string username, string password, string firstname, string lastname, string car, decimal balance, bool activeperiod)
        {
            ID = id;
            Username = username;
            Password = password;
            Firstname = firstname;
            Lastname = lastname;
            Car = car;
            Balance = balance;
            ActivePeriod = activeperiod;
        }
    }
}
