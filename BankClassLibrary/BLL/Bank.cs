using BankClassLibrary.Models;
using BankClassLibrary.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
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
        decimal bankBalance;
        Account accountOpject;
        public string bankName;
        public decimal BankBalance
        {
            get
            {
                bankBalance = 0;
                foreach (Account account in fileRepository.GetAllAccounts())
                {
                    bankBalance += account.Balance;
                }
                return bankBalance;
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
            lock (_LockAccount)
            {
                if (type == AccountType.checkingAccount)
                {
                    CheckingAccount checkingAccount = new CheckingAccount(name);
                    fileRepository.AddAccount(checkingAccount);
                    accountOpject = checkingAccount;
                }
                else if (type == AccountType.savingsAccount)
                {
                    SavingsAccount savings = new SavingsAccount(name);
                    fileRepository.AddAccount(savings);
                    accountOpject = savings;
                }
                else if (type == AccountType.masterCardAccount)
                {
                    MasterCardAccount master = new MasterCardAccount(name);
                    fileRepository.AddAccount(master);
                    accountOpject = master;
                }
                return accountOpject;
            }
        }
        #endregion
        public decimal Deposit(decimal amount, int accountNumber)
        {
            //accountOpject = GetAccountOpject(accountNumber);
            lock (_LockAccount)
            {
                accountOpject = fileRepository.GetAccount(accountNumber);
                accountOpject.Balance = amount;
                return accountOpject.Balance;
            }
            
        }
        public decimal Withdraw(decimal amount, int accountNumber)
        {
            //accountOpject = GetAccountOpject(accountNumber);
            lock (_LockAccount)
            {
                accountOpject = fileRepository.GetAccount(accountNumber);
                accountOpject.Balance = (amount * -1);
                return accountOpject.Balance;
            }
        }
        public decimal Balance(int accountNumber)
        {
            //accountOpject = fileRepository.GetAccount(accountNumber);
            //accountOpject = GetAccountOpject(accountNumber);
            return fileRepository.GetAccount(accountNumber).Balance;
        }

        public Account GetAccountOpject(int accountNumber)
        {
            accountOpject = fileRepository.GetAccount(accountNumber);
            return accountOpject;
        }
        public void ChargeInterest()
        {
            lock (_LockAccount)
            {
                foreach (Account item in fileRepository.GetAllAccounts())
                {
                    item.ChargeInterest();
                }
            }
        }
        public List<AccountListItem> GetAccountList()
        {
            return fileRepository.GetAccountList();
        }
        public LogHandlerDelegate LogHandlerEvent;
        public void SaveBank()
        {
            fileRepository.SaveBank();
        }
        public void UpdateAccount(Account acc)
        {
            fileRepository.UpdateAccount(acc);
        }
    }
}
