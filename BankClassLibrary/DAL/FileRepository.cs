using BankClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BankClassLibrary.DAL
{
    class FileRepository : IFileRepository
    {
        List<Account> accountList;
        int accountNumberCounter;
        const string fileName = @"Data\datafile.txt";
        public FileRepository()
        {
            accountList = new List<Account>();
        }
        public int AddAccount(Account account)
        {
            accountNumberCounter++;
            accountList.Add(account);
            account.AccountNumber = accountNumberCounter;
            return account.AccountNumber;
        }

        public Account GetAccount(int id)
        {
            return accountList.Find(x => x.AccountNumber == id);
        }

        public List<AccountListItem> GetAccountList()
        {
            List<AccountListItem> accountList = new List<AccountListItem>();
            foreach (Account ali in this.accountList)
            {
                AccountListItem listItem = new AccountListItem(ali);
                accountList.Add(listItem);
            }
            return accountList;
        }

        public List<Account> GetAllAccounts()
        {
            return accountList;
        }

        public List<Account> LoadBank()
        {
            accountList = new List<Account>();
            if (File.Exists(fileName))
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string afvent = sr.ReadLine();
                    if (afvent == null || afvent == "")
                    {

                    }
                    else
                    {
                        foreach (var item in afvent.Split(';'))
                        {
                            string[] data = item.Split(':');
                            if (data[3].Split('.')[2] == "CheckingAccount")
                            {
                                accountList.Add(new CheckingAccount(Convert.ToInt32(data[0]), data[1], Convert.ToDecimal(data[2])));
                            }
                            else if (data[3].Split('.')[2] == "MasterCardAccount")
                            {
                                accountList.Add(new MasterCardAccount(Convert.ToInt32(data[0]), data[1], Convert.ToDecimal(data[2])));
                            }
                            else if (data[3].Split('.')[2] == "SavingsAccount")
                            {
                                accountList.Add(new SavingsAccount(Convert.ToInt32(data[0]), data[1], Convert.ToDecimal(data[2])));
                            }
                            accountNumberCounter++;
                        }
                    }
                }
            }
            return accountList;
        }

        public void SaveBank()
        {
            bool splitChar = false;
            string[] accountOpject = new string[1];
            if (!Directory.Exists(fileName))
            {
                Directory.CreateDirectory("Data");
            }
            if (!File.Exists(fileName))
            {
                File.CreateText(fileName);
            }
            foreach (Account account in accountList)
            {
                if (splitChar == false)
                {

                    accountOpject[0] += $"{account.AccountNumber}:{account.Name}:{account.Balance}:{account.GetType()}";
                }
                else
                {

                    accountOpject[0] += $";{account.AccountNumber}:{account.Name}:{account.Balance}:{account.GetType()}";
                }
                splitChar = true;
            }
            File.WriteAllLines(fileName, accountOpject);
        }

        public void UpdateAccount(Account acc)
        {
            SaveBank();
        }
    }
}
