using BankClassLibrary.Models;
using BankClassLibrary.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using BankClassLibrary.Utilities;
using System.Threading;

namespace BankClassLibrary.Repository
{
    public delegate void LogHandlerDelegate(string logMessage);
    public class Bank : IBank
    {

        private Object _LockAccount = new Object();
        FileRepository fileRepository;
        private decimal _bankBalance;
        private Account _accountOpject;
        public string bankName;
        public decimal BankBalance
        {
            get
            {
                _bankBalance = 0;
                foreach (Account account in fileRepository.GetAllAccounts())
                {
                    _bankBalance += account.Balance;
                }
                return _bankBalance;
            }
        }
        public Bank(string bankName)
        {
            fileRepository = new FileRepository();
            fileRepository.LoadBank();
            this.bankName = bankName;
        }

        #region CreateAccount
        public Account CreateAccount(string name, AccountType type)
        {
            //Laver en account efter brugerens tidligere indtasninger og ligger en lock på så Tasks ikke påvirker hinanden
            lock (_LockAccount)
            {
                if (type == AccountType.checkingAccount)
                {
                    CheckingAccount checkingAccount = new CheckingAccount(name);
                    fileRepository.AddAccount(checkingAccount);
                    _accountOpject = checkingAccount;
                }
                else if (type == AccountType.savingsAccount)
                {
                    SavingsAccount savings = new SavingsAccount(name);
                    fileRepository.AddAccount(savings);
                    _accountOpject = savings;
                }
                else if (type == AccountType.masterCardAccount)
                {
                    MasterCardAccount master = new MasterCardAccount(name);
                    fileRepository.AddAccount(master);
                    _accountOpject = master;
                }
                return _accountOpject;
            }
        }
        #endregion

        public async Task<string> Deposit(decimal amount, int accountNumber)
        {
            //Indsætter et ønsket beløb ind på en ønsket konto og retunere en Task string som logges
            return await Task.Run(() =>
            {
                lock (_LockAccount)
                {
                    _accountOpject = fileRepository.GetAccount(accountNumber);
                    _accountOpject.Balance = amount;
                    Console.WriteLine("Din saldo er: {0}", _accountOpject.Balance.ToString("c"));
                    //strengen sendes tilbage og logges
                    return $"{DateTime.Now.ToString()}. {amount.ToString("c")} kr. lagt ind på konto nr. {accountNumber}. nye beløb: {_accountOpject.Balance.ToString("c")}";
                }
            });


        }

        public async Task<string> Withdraw(decimal amount, int accountNumber)
        {
            try
            {
                //Hæver et ønsket beløb fra en ønsket konto og retunere en Task string som logges
                return await Task.Run(() =>
                {
                    lock (_LockAccount)
                    {
                        _accountOpject = fileRepository.GetAccount(accountNumber);
                        //Denne if else sørger for at man ikke kan manipulere indtastning til man får penge når man hæver
                        if (amount < 0)
                        {
                            _accountOpject.Balance = amount;
                        }
                        else
                        {
                            _accountOpject.Balance = (amount * -1);
                        }
                        Console.WriteLine("Din saldo er: {0}", _accountOpject.Balance.ToString("c"));
                        //strengen som sendes tilbage og bliver logget
                        return $"{DateTime.Now.ToString()}. Beløb {amount.ToString("c")} er blevet hævet fra konto {accountNumber}. Den nye saldo er: {_accountOpject.Balance.ToString("c")}";
                    }
                });
            }
            catch (OverdraftException e)
            {
                //Hvis brugen prøver at hæve mere end hvad der er på kontoen logges det her og sender det videre til programmet
                LogHandlerEvent($"{DateTime.Now}. Der er prøvet at hæve: {amount.ToString("c")} fra konto: {_accountOpject.AccountNumber} men fejlede");
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public decimal Balance(int accountNumber)
        {
            //retunere den enkelte accounts saldo
            return GetAccountOpject(accountNumber).Balance;
        }

        public Account GetAccountOpject(int accountNumber)
        {
            return _accountOpject = fileRepository.GetAccount(accountNumber);
        }

        public void ChargeInterest()
        {
            //tilskriver renter for alle accounts der er i banken
            lock (_LockAccount)
            {
                foreach (Account item in fileRepository.GetAllAccounts())
                {
                    item.ChargeInterest();
                }
            }
        }

        public List<AccountListItem> GetAccountListForPosting()
        {
            //Får en liste af AccountListItem som sendes videre
            return fileRepository.GetAccountList();
        }

        public async Task GetAccountListString(List<AccountListItem> accList)
        {
            //Laver en Liste af Task string som indeholder de enkelte ting som der er valgt fra Listen accountListItem
            List<Task<string>> printAccList = new List<Task<string>>();
            foreach (AccountListItem item in accList)
            {
                printAccList.Add(Task<string>.Run(() => $"Konto ejer: {item.Name}\nKonto saldo: {item.Balance.ToString("c")}\n"));
            }

            Task printMe = Task.WhenAll(printAccList);
            //Venter på at alle er kørt igennem før de bliver udskrivet
            await printMe;
            printAccList.ForEach(p => Console.WriteLine(p.Result));
        }

        public LogHandlerDelegate LogHandlerEvent;

        public void SaveBank()
        {
            //Køre en anden metode som gemmer banken
            fileRepository.SaveBank();
        }

        public void UpdateAccount(Account acc)
        {
            //Bliver ikke brugt da man ikke kan gemme en enkelt del af en textfil
            fileRepository.UpdateAccount(acc);
        }
    }
}
