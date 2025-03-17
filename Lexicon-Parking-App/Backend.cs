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
        public string accountsFilename = "Accounts.xml";
        public string periodsFilename = "Periods.xml";

        XDocument xDocAccounts;
        XDocument xDocPeriods;

        List<User> Accounts { get; set; }
        List<Period> Periods { get; set; }

        public decimal daytimeCost = 14;
        public decimal nighttimeCost = 8;

        static int newID = 0;

        public Backend()
        {
            xDocAccounts = new XDocument();
            xDocPeriods = new XDocument();

            Accounts = new List<User>();
            Periods = new List<Period>();

            LoadAccountsXML(programPath + accountsFilename);
            LoadPeriodsXML(programPath + periodsFilename);
        }

        public string StartPeriod(int accountID, string licenseplate)
        {
            // Check if a period is already active.
            // Start new period
            foreach (User account in Accounts)
            {
                if (account.ID == accountID && account.Licenseplate == licenseplate)
                {
                    if (!account.ActivePeriod)
                    {
                        Period newPeriod = new Period(accountID, licenseplate, DateTime.Now);

                        Periods.Add(newPeriod);

                        account.ActivePeriod = true;
                        return "Period Started successfully.";
                    }
                }
            }
            return "Error while starting period.";
        }

        public string EndPeriod(int accountID, string licenseplate)
        {
            // Check if a period is not active.
            // End period
            foreach (User account in Accounts)
            {
                if (account.ID == accountID && account.Licenseplate == licenseplate)
                {
                    if (account.ActivePeriod)
                    {
                        //End period
                        account.Balance += CalculateCost(accountID);

                        Periods.RemoveAll(i => i.AccountID == accountID && i.Licenseplate == licenseplate);

                        account.ActivePeriod = false;
                        return "Period Ended successfully.";
                    }
                }
            }
            return "Error while ending period.";
        }

        public string GetSession(int accountID, string licenseplate)
        {
            // Check if period is active. If it is, return date started and total cost of period
            User account = Accounts.Find(i => i.ID == accountID);
            if (account != null)
            {
                if (account.ActivePeriod)
                {
                    //Calculate cost of session
                    decimal cost = CalculateCost(accountID);

                    Period period = Periods.Find(i => i.AccountID == accountID && i.Licenseplate == licenseplate);

                    return $"Period started {period.StartDate} and will cost {cost}kr so far.";
                }
                else
                {
                    return "No Active Session";
                }
            }
            else
            {
                return "Either account or licenseplate was not found.";
            }
        }

        public string Login(string username, string password)
        {
            User tempAccount;

            try
            {
                tempAccount = Accounts.Find(i => i.Username == username);
            }
            catch
            {
                return "No account with that username exists.";
            }

            if (tempAccount.Username == username && tempAccount.Password == password)
            {
                return "Login successful.";
            }
            else
            {
                return "Password entered was incorrect";
            }
        }

        public string RegisterNewUser(string username, string password, string firstname, string lastname, string licenseplate)
        {
            User newAccount = new User(username, password, firstname, lastname, licenseplate);

            Accounts.Add(newAccount);
            return "Account added successfully";
        }

        public decimal AccountBalance(int accountID)
        {
            // Get balance from account by using uniqueID
            return Accounts.Find(i => i.ID == accountID).Balance;
        }

        public User AccountDetails(int accountID)
        {
            // Get details from account by using uniqueID
            User account = Accounts.Find(i => i.ID == accountID);

            return account;
        }

        public decimal CalculateCost(int accountID)
        {
            decimal cost = 0;

            Period period = Periods.Find(i => i.AccountID == accountID);
            DateTime stopDate = DateTime.Now;

            DateTime currentTime = period.StartDate;

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

        void LoadAccountsXML(string filePath)
        {
            var accounts = xDocAccounts.Descendants("account")

                .Select(accountElement => new User(

                    int.Parse(accountElement.Element("id").Value),

                    accountElement.Element("username").Value,
                    accountElement.Element("password").Value,
                    accountElement.Element("firstname").Value,
                    accountElement.Element("lastname").Value,
                    accountElement.Element("licenseplate").Value,

                    decimal.Parse(accountElement.Element("balance").Value),
                    bool.Parse(accountElement.Element("activesession").Value)

                    )).ToList();

            Accounts.AddRange(accounts);

            foreach (var account in Accounts)
            {
                if (account.ID > newID)
                {
                    newID = account.ID;
                }
            }
        }

        void LoadPeriodsXML(string filePath)
        {
            var periods = xDocAccounts.Descendants("period")

            .Select(periodElement => new Period(

                int.Parse(periodElement.Element("accountid").Value),

                (periodElement.Element("licenseplate").Value),

                DateTime.Parse(periodElement.Element("username").Value)

                )).ToList();

            Periods.AddRange(periods);
        }

        public void SaveAccountXML(string filePath)
        {
            xDocAccounts = new XDocument(
                new XElement("Accounts",
                    Accounts.Select(account =>
                        new XElement("account",
                            new XElement("id", account.ID),
                            new XElement("username", account.Username),
                            new XElement("password", account.Password),
                            new XElement("firstname", account.Firstname),
                            new XElement("lastname", account.Lastname),
                            new XElement("licenseplate", account.Licenseplate),
                            new XElement("balance", account.Balance),
                            new XElement("activeperiod", account.ActivePeriod)
            ))));

            xDocAccounts.Save(filePath);
        }

        public void SavePeriodsXML(string filePath)
        {
            xDocPeriods = new XDocument(
                new XElement("Periods",
                    Periods.Select(period =>
                        new XElement("period",
                            new XElement("accountid", period.AccountID),
                            new XElement("startdate", period.StartDate)
            ))));

            xDocPeriods.Save(filePath);
        }

        public static int UniqueId()
        {
            newID += 1;
            return newID;
        }
    }
}
