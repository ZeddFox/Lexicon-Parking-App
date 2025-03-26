using System;
using System.Linq;
using System.Security.Principal;
using System.Text.Json;

namespace Lexicon_Parking_App
{
    public class Backend
    {
        public string programPath = "C:\\Users\\zeddf\\source\\repos\\Lexicon-Parking-App\\Lexicon-Parking-App\\";
        public string usersFilename = "Users.json";
        public string periodsFilename = "Periods.json";

        public string userPath;
        public string periodsPath;

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

            Users = ReadUsersFromFile();
            Periods = ReadPeriodsFromFile();
        }

        public Period? StartPeriod(int userID)
        {
            startPeriodMessage = "";

            // Find user with provided userID
            User? currentUser = Users.Find(user => user.UserID == userID);
            if (currentUser != null)
            {
                // Find active period connected to userID, If period already exists, return null
                Period foundPeriod = Periods.Find(period => period.UserId == userID && period.EndTime == null);

                if (foundPeriod != null)
                {
                    startPeriodMessage = "Active session already exists";
                    Console.WriteLine(startPeriodMessage);
                    return null;
                }
                // If period does not exist, start new period then return the new period
                else
                {
                    Period newPeriod = new Period(userID, DateTime.Now);

                    Periods.Add(newPeriod);
                    WritePeriodsToFile(Periods);

                    startPeriodMessage = "Session started successfully";
                    Console.WriteLine(startPeriodMessage);
                    return newPeriod;
                }
            }
            // If user does not exist, return null
            else
            {
                startPeriodMessage = "User does not exist.";
                Console.WriteLine(startPeriodMessage);
                return null;
            }
        }
        public Period? EndPeriod(int userID)
        {
            endPeriodMessage = "";

            // Find user with provided userID
            User? currentUser = Users.Find(user => user.UserID == userID);
            if (currentUser != null)
            {
                // Find active period connected to userID, If period does not exist, return null
                Period currentPeriod = Periods.Find(period => period.UserId == userID && period.EndTime == null);
                if (currentPeriod == null)
                {
                    endPeriodMessage = "No active session found";
                    Console.WriteLine(endPeriodMessage);
                    return null;
                }
                // If period does not exist, return the new period
                else
                {
                    currentPeriod.Stop();

                    currentPeriod.PeriodCost = CalculateCost(currentPeriod);
                    currentUser.Balance += currentPeriod.PeriodCost;

                    WriteUsersToFile(Users);
                    WritePeriodsToFile(Periods);

                    endPeriodMessage = "Session ended successfully";
                    Console.WriteLine(endPeriodMessage);
                    return currentPeriod;
                }
            }
            // If user does not exist, return null
            else
            {
                endPeriodMessage = "User does not exist.";
                Console.WriteLine(endPeriodMessage);
                return null;
            }
        }
        public Period? GetSession(int userID)
        {
            currentPeriodMessage = "";

            // Find user. If user exists, find period.
            User? tempUser = Users.Find(item => item.UserID == userID);
            if (tempUser != null)
            {
                // Find period. If period does not exist, return null
                Period? currentPeriod = Periods.Find(period => period.UserId == userID && period.EndTime == null);
                if (currentPeriod == null)
                {
                    currentPeriodMessage = $"No period found for {tempUser.Firstname + " " + tempUser.Lastname}";
                    Console.WriteLine(currentPeriodMessage);
                    return null;
                }
                // If period exists, return period
                else
                {
                    currentPeriodMessage = "Period obtained successfully";
                    Console.WriteLine(currentPeriodMessage);

                    currentPeriod.PeriodCost = CalculateCost(currentPeriod);

                    return currentPeriod;
                }

            }
            // If user doesn't exist, return null
            else
            {
                currentPeriodMessage = $"No user with id: {userID} was found.";
                Console.WriteLine(currentPeriodMessage);
                return null;
            }
        }
        public List<Period>? GetPreviousSessions(int userID)
        {
            previousPeriodsMessage = "";

            User? currentUser = null;

            // Find user with provided userID
            try
            {
                currentUser = Users.Find(user => user.UserID == userID);

                // Find active periods connected to userID
                try
                {
                    List<Period>? previousPeriods = Periods.Where(period => period.UserId == userID && period.EndTime != null).ToList();

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

            // Find user by username
            User? tempUser = Users.Find(i => i.Username == username);
            if (tempUser == null)
            {
                loginMessage = "User not found";
                Console.WriteLine(loginMessage);
                return null;
            }
            // If user is found, compare their password with input password
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

            // Check for incorrect or missng data
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Licenseplate))
            {
                registerMessage = "Invalid data";
                Console.WriteLine(registerMessage);
                return false;
            }
            // If data is correctly input, continue
            else
            {
                User? newUser = null;

                // Check if user with same username exists
                newUser = Users.Find(i => i.Username == user.Username);

                if (newUser != null)
                {
                    registerMessage = "User with that username already exists";
                    Console.WriteLine(registerMessage);
                    return false;
                }
                // If user does not already exist, create new
                else
                {
                    newUser = new User();

                    newUser.Username = user.Username;
                    newUser.Password = user.Password;
                    newUser.Firstname = user.Firstname;
                    newUser.Lastname = user.Lastname;
                    newUser.Licenseplate = user.Licenseplate;

                    Users.Add(newUser);
                    WriteUsersToFile(Users);

                    registerMessage = "User registered successfully";
                    Console.WriteLine(registerMessage);

                    return true;
                }
            }
        }
        public decimal? UserBalance(int userID)
        {
            userBalanceMessage = "";

            // Find user by their userID
            User? tempUser = Users.Find(i => i.UserID == userID);

            if (tempUser != null)
            {
                // If user exists return balance
                userBalanceMessage = "Balance retreived successfully.";
                Console.WriteLine(userBalanceMessage);
                return tempUser.Balance;
            }
            else
            {
                // If user does not exist return null
                userBalanceMessage = "User does not exist.";
                Console.WriteLine(userBalanceMessage);
                return null;
            }
        }
        public User? UserDetails(int userID)
        {
            userDetailsMessage = "";

            // Find user by their userID
            User? tempUser = Users.Find(i => i.UserID == userID);

            if (tempUser != null)
            {
                // If user exists return user
                userBalanceMessage = "Balance retreived successfully.";
                Console.WriteLine(userBalanceMessage);
                return tempUser;
            }
            else
            {
                // If user does not exist return null
                userBalanceMessage = "User does not exist.";
                Console.WriteLine(userBalanceMessage);
                return null;
            }
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

        #region JSON Functions
        public void WriteUsersToFile(List<User> toWrite)
        {
            try
            {
                // Convert list to JSON then write to file
                var json = JsonSerializer.Serialize(toWrite);
                File.WriteAllText(userPath, json);
                Console.WriteLine("Successfully wrote users to file");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing users to file.");
            }
        }
        public void WritePeriodsToFile(List<Period> toWrite)
        {
            try
            {
                // Convert list to JSON then write to file
                var json = JsonSerializer.Serialize(toWrite);
                File.WriteAllText(periodsPath, json);
                Console.WriteLine("Successfully wrote periods to file");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing periods to file.");
            }
        }
        public List<User>? ReadUsersFromFile()
        {
            try
            {
                // Read all text from file
                var json = File.ReadAllText(userPath);
                List<User> readList = new List<User>();

                // Convert read text to JSON then make list containing data
                readList = JsonSerializer.Deserialize<List<User>>(json);
                Console.WriteLine("Successfully read users from file");
                return readList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to file.");
                Console.WriteLine(ex);
                return new List<User>();
            }
        }
        public List<Period>? ReadPeriodsFromFile()
        {
            try
            {
                // Read all text from file
                var json = File.ReadAllText(periodsPath);
                List<Period> readList = new List<Period>();

                // Convert read text to JSON then make list containing data
                readList = JsonSerializer.Deserialize<List<Period>>(json);
                Console.WriteLine("Successfully read periods from file");
                return readList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to file.");
                Console.WriteLine(ex);
                return new List<Period>();
            }
        }
        #endregion

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
