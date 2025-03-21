using Microsoft.Extensions.Configuration.UserSecrets;

namespace Lexicon_Parking_App
{
    public class User
    {
        public int UserID { get; set; } = 0;
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Licenseplate { get; set; }
        public decimal Balance { get; set; } = 0m;

        // Create new user
        public User()
        {
            UserID = Backend.UniqueId();
        }

        // Load User from XML
        public User(int userid, string username, string password, string firstname, string lastname, string licenseplate, decimal balance)
        {
            UserID = userid;
            Username = username;
            Password = password;
            Firstname = firstname; 
            Lastname = lastname;
            Licenseplate = licenseplate;
            Balance = balance;
        }
    }
}
