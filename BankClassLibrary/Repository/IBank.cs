using BankClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankClassLibrary.Repository
{
    interface IBank
    {
        List<AccountListItem> GetAccountListForPosting();
        public decimal BankBalance { get; }
        public Account CreateAccount(string name, AccountType type);
        public Task<string> Deposit(decimal amount, int accountNumber);
        public Task<string> Withdraw(decimal amount, int accountNumber);
        public decimal Balance(int accountNumber);
        //private Account GetAccountOpject(int accountNumber); giver fejl af en eller anden grund
        public void ChargeInterest();
    }
}
