using System;
using System.Linq;
using System.Security.Principal;
using System.Xml;
using System.Xml.Linq;

namespace Lexicon_Parking_App
{
    public class Backend
    {
        public string programPath = "C:\\Users\\zeddf\\source\\repos\\Lexicon-Parking-App\\Lexicon-Parking-App\\";
        public string usersFilename = "Users.xml";
        public string periodsFilename = "Periods.xml";

        public string userPath;
        public string periodsPath;

        XDocument xDocUsers;
        XDocument xDocPeriods;

        List<User> Users { get; set; }
        List<Period> Periods { get; set; }

        public decimal daytimeCost = 14;
        public decimal nighttimeCost = 8;

        static int newID = 0;
        static int newPeriodID = 0;

        public string startPeriodMessage = "";
        public string endPeriodMessage = "";
        public string currentPeriodMessage = "";
        public string previousPeriodsMessage = "";
        public string registerMessage = "";
        public string loginMessage = "";
        public string userBalanceMessage = "";
        public string userDetailsMessage = "";

        public Backend()
        {
            userPath = programPath + usersFilename;
            periodsPath = programPath + periodsFilename;

            xDocUsers = new XDocument();
            xDocPeriods = new XDocument();

            Users = new List<User>();
            Periods = new List<Period>();

            LoadUsersXML(programPath + usersFilename);
            LoadPeriodsXML(programPath + periodsFilename);
        }

        public Period? StartPeriod(int userID)
        {
            startPeriodMessage = "";

            User currentUser = null;

            // Find user with provided userID
            try
            {
                currentUser = Users.Find(user => user.UserID == userID);

                Period foundPeriod = Periods.Find(period => period.UserID == userID && period.EndTime == null);

                if (foundPeriod != null)
                {
                    startPeriodMessage = "Active session already exists";
                    Console.WriteLine(startPeriodMessage);
                    return null;
                }
                else
                {
                    Period newPeriod = new Period(userID, DateTime.Now);

                    Periods.Add(newPeriod);
                    SavePeriodsXML(periodsPath);

                    startPeriodMessage = "Session started successfully";
                    Console.WriteLine(startPeriodMessage);
                    return newPeriod;
                }
            }
            // If user is not found, return message
            catch
            {
                startPeriodMessage = "User does not exist.";
                Console.WriteLine(startPeriodMessage);
                return null;
            }
            startPeriodMessage = "Error while starting period.";
            Console.WriteLine(startPeriodMessage);
            return null;
        }
        public Period? EndPeriod(int userID)
        {
            endPeriodMessage = "";

            User currentUser = null;

            // Find user with provided userID
            try
            {
                currentUser = Users.Find(user => user.UserID == userID);

                // Find active period connected to userID
                Period currentPeriod = Periods.Find(period => period.UserID == userID && period.EndTime == null);

                if (currentPeriod == null)
                {
                    endPeriodMessage = "No active session found";
                    Console.WriteLine(endPeriodMessage);
                    return null;
                }
                else
                {
                    currentPeriod.Stop();
                    currentUser.Balance += CalculateCost(currentPeriod);

                    endPeriodMessage = "Session ended successfully";
                    Console.WriteLine(endPeriodMessage);
                    return currentPeriod;
                }
            }
            // If user is not found, return message
            catch
            {
                endPeriodMessage = "User does not exist.";
                Console.WriteLine(endPeriodMessage);
                return null;
            }
            endPeriodMessage = "Error while stopping period.";
            Console.WriteLine(endPeriodMessage);
            return null;
        }
        public Period? GetSession(int userID)
        {
            currentPeriodMessage = "";
            User? tempUser = Users.Find(item => item.UserID == userID);

            if (tempUser == null)
            {
                currentPeriodMessage = $"No user with id: {userID} was found.";
                Console.WriteLine(currentPeriodMessage);
                return null;
            }
            else
            {
                Period? currentPeriod = Periods.Find(period => period.UserID == userID && period.EndTime == null);

                if (currentPeriod == null)
                {
                    currentPeriodMessage = $"No period found for {tempUser.Firstname + " " + tempUser.Lastname}";
                    Console.WriteLine(currentPeriodMessage);
                    return null;
                }
                else
                {
                    currentPeriodMessage = "Period obtained successfully";
                    Console.WriteLine(currentPeriodMessage);
                    return currentPeriod;
                }
            }
        }
        public List<Period>? GetPreviousSessions(int userID)
        {
            previousPeriodsMessage = "";

            User currentUser = null;

            // Find user with provided userID
            try
            {
                currentUser = Users.Find(user => user.UserID == userID);

                // Find active periods connected to userID
                try
                {
                    List<Period>? previousPeriods = Periods.Where(period => period.UserID == userID && period.EndTime != null).ToList();

                    previousPeriodsMessage = "Sessions listed successfully";
                    Console.WriteLine(previousPeriodsMessage);
                    return previousPeriods;
                }
                // If periods not found, return message
                catch
                {
                    previousPeriodsMessage = "No sessions found";
                    Console.WriteLine(previousPeriodsMessage);
                    return null;
                }
            }
            // If user is not found, return message
            catch
            {
                previousPeriodsMessage = "User does not exist.";
                Console.WriteLine(previousPeriodsMessage);
                return null;
            }
            previousPeriodsMessage = "Error while stopping period.";
            Console.WriteLine(previousPeriodsMessage);
            return null;
        }
        public User? Login(string username, string password)
        {
            loginMessage = "";

            User tempUser = Users.Find(i => i.Username == username);

            if (tempUser == null)
            {
                loginMessage = "User not found";
                Console.WriteLine(loginMessage);
                return null;
            }
            else
            {
                if (tempUser.Password != password)
                {
                    loginMessage = "Incorrect password";
                    Console.WriteLine(loginMessage);
                    return null;
                }
                else
                {
                    loginMessage = "Login successful";
                    Console.WriteLine(loginMessage);
                    return tempUser;
                }
            }

        }
        public bool RegisterNewUser(User? user)
        {
            registerMessage = "";

            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Licenseplate))
            {
                registerMessage = "Invalid data";
                Console.WriteLine(registerMessage);
                return false;
            }
            else
            {
                User newUser = Users.Find(i => i.Username == user.Username);

                if (newUser != null)
                {
                    registerMessage = "User with that username already exists";
                    Console.WriteLine(registerMessage);
                    return false;
                }
                else
                {
                    newUser = new User();

                    newUser.Username = user.Username;
                    newUser.Password = user.Password;
                    newUser.Firstname = user.Firstname;
                    newUser.Lastname = user.Lastname;
                    newUser.Licenseplate = user.Licenseplate;

                    Users.Add(newUser);
                    SaveUsersXML(userPath);

                    registerMessage = "User registered successfully";
                    Console.WriteLine(registerMessage);

                    return true;
                }
            }
        }
        public decimal? UserBalance(int userID)
        {
            // Get balance from account by using uniqueID
            return Users.Find(i => i.UserID == userID).Balance;
        }
        public User? UserDetails(int userID)
        {
            // Get details from account by using uniqueID
            User user = Users.Find(i => i.UserID == userID);

            return user;
        }

        public decimal CalculateCost(Period period)
        {
            decimal cost = 0;
            DateTime? stopDate;

            if (period.EndTime == null)
            {
                stopDate = DateTime.Now;
            }
            else
            {
                stopDate = period.EndTime;
            }

            DateTime currentTime = period.StartTime;

            while (currentTime < stopDate)
            {
                decimal hourCost;

                if (currentTime.Hour >= 8 && currentTime.Hour < 18)
                {
                    hourCost = daytimeCost;
                }
                else
                {
                    hourCost = nighttimeCost;
                }

                cost += hourCost;
                currentTime = currentTime.AddHours(1);
            }

                return cost;
        }

        void LoadUsersXML(string filePath)
        {
            var users = xDocUsers.Descendants("user")

                .Select(userElement => new User(

                    int.Parse(userElement.Element("userid").Value),
                    userElement.Element("username").Value,
                    userElement.Element("password").Value,
                    userElement.Element("firstname").Value,
                    userElement.Element("lastname").Value,
                    userElement.Element("licenseplate").Value,
                    decimal.Parse(userElement.Element("balance").Value)

                    )).ToList();

            Users.AddRange(users);

            foreach (var user in Users)
            {
                if (user.UserID > newID)
                {
                    newID = user.UserID;
                }
            }

            Console.WriteLine("Successfully loaded Users");
        }
        void LoadPeriodsXML(string filePath)
        {
            var periods = xDocUsers.Descendants("period")

            .Select(periodElement => new Period(

                int.Parse(periodElement.Element("periodid").Value),
                int.Parse(periodElement.Element("userid").Value),
                DateTime.Parse(periodElement.Element("starttime").Value),
                DateTime.Parse(periodElement.Element("endtime").Value),
                decimal.Parse(periodElement.Element("cost").Value)
                )).ToList();

            Periods.AddRange(periods);
            Console.WriteLine("Successfully loaded Periods");
        }

        public void SaveUsersXML(string filePath)
        {
            xDocUsers = new XDocument(
                new XElement("users",
                    Users.Select(user =>
                        new XElement("user",
                            new XElement("userid", user.UserID),
                            new XElement("username", user.Username),
                            new XElement("password", user.Password),
                            new XElement("firstname", user.Firstname),
                            new XElement("lastname", user.Lastname),
                            new XElement("licenseplate", user.Licenseplate),
                            new XElement("balance", user.Balance)
            ))));

            xDocUsers.Save(filePath);
            Console.WriteLine("Successfully saved Users");
        }
        public void SavePeriodsXML(string filePath)
        {
            xDocPeriods = new XDocument(
                new XElement("Periods",
                    Periods.Select(period =>
                        new XElement("period",
                            new XElement("periodid", period.PeriodId),
                            new XElement("userid", period.UserID),
                            new XElement("starttime", period.StartTime),
                            new XElement("endtime", period.EndTime),
                            new XElement("cost", period.Cost)
            ))));

            xDocPeriods.Save(filePath);
            Console.WriteLine("Successfully saved Periods");
        }

        public static int UniqueId()
        {
            newID += 1;
            return newID;
        }

        public static int UniquePeriodId()
        {
            newPeriodID += 1;
            return newPeriodID;
        }
    }
}
