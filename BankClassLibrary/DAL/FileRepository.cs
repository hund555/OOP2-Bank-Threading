using BankClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace BankClassLibrary.DAL
{
    class FileRepository : IFileRepository
    {
        private List<Account> _accountList;
        int _accountNumberCounter;
        //Stien til data filen som indeholder Brugernes data til når banken skal loades ind igen
        private const string _fileName = @"Data\datafile.txt";

        public FileRepository()
        {
            _accountList = new List<Account>();
        }

        public int AddAccount(Account account)
        {
            //Når der oprettes en account sættes den ind i accountlisten 
            Interlocked.Increment(ref _accountNumberCounter);
            _accountList.Add(account);
            account.AccountNumber = _accountNumberCounter;
            return account.AccountNumber;
        }

        public Account GetAccount(int id)
        {
            //Retunere et account opjekt som har tilhørende kontonummer 
            return _accountList.Find(x => x.AccountNumber == id);
        }

        public List<AccountListItem> GetAccountList()
        {
            //Laver en liste af AccountListItem som sendes videre
            List<AccountListItem> accList = new List<AccountListItem>();
            foreach (Account ali in this._accountList)
            {
                AccountListItem listItem = new AccountListItem(ali);
                accList.Add(listItem);
            }
            return accList;
        }

        public List<Account> GetAllAccounts()
        {
            return _accountList;
        }

        public List<Account> LoadBank()
        {
            //Laver en ny liste med accounts og henter de Tidligere brugere som er gemt i en fil med deres data
            _accountList = new List<Account>();
            if (File.Exists(_fileName))
            {
                //Læser filen som ligger i stien som er gemt i fileName og bruger useing for at afbryde forbindelsen til filen når den er færdig
                using (StreamReader sr = new StreamReader(_fileName))
                {
                    //Læser filen
                    string afvent = sr.ReadLine();
                    //checker om der er noget i filen
                    if (afvent == null || afvent == "")
                    {

                    }
                    //Hvis filen ikke er tom læsses dataen ind
                    else
                    {
                        //Første køres Én lang string ind som splittes op med ';'
                        foreach (var item in afvent.Split(';'))
                        {
                            //Derefter Splittes Dataen for den engkelte account op med ':'
                            string[] data = item.Split(':');
                            //Til sidst Splittes deres Type Så den får den rigtige type account
                            if (data[3].Split('.')[2] == "CheckingAccount")
                            {
                                _accountList.Add(new CheckingAccount(Convert.ToInt32(data[0]), data[1], Convert.ToDecimal(data[2])));
                            }
                            else if (data[3].Split('.')[2] == "MasterCardAccount")
                            {
                                _accountList.Add(new MasterCardAccount(Convert.ToInt32(data[0]), data[1], Convert.ToDecimal(data[2])));
                            }
                            else if (data[3].Split('.')[2] == "SavingsAccount")
                            {
                                _accountList.Add(new SavingsAccount(Convert.ToInt32(data[0]), data[1], Convert.ToDecimal(data[2])));
                            }
                            _accountNumberCounter++;
                        }
                    }
                }
            }
            return _accountList;
        }

        public void SaveBank()
        {
            bool splitChar = false;
            string[] accountOpject = new string[1];
            //Hvis mappen ikke eksitere oprettes den
            if (!Directory.Exists(_fileName))
            {
                Directory.CreateDirectory("Data");
            }
            //Hvis filen ikke eksistere opresttes den
            if (!File.Exists(_fileName))
            {
                File.CreateText(_fileName);
            }
            //Køre alle accounts som ligger i accountlisten og skriver dem ind i en lang streng med tegn til at splitte dem igen når de loades ind i banken igen
            foreach (Account account in _accountList)
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
            //Skriver strengen ind i filen
            File.WriteAllLines(_fileName, accountOpject);
        }

        public void UpdateAccount(Account acc)
        {
            SaveBank();
        }
    }
}
