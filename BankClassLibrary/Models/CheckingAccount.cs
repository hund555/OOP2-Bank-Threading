using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankClassLibrary.Models
{
    class CheckingAccount : Account
    {
        public CheckingAccount(string name)
        {
            this.AccountType = AccountType.checkingAccount;
            this.name = name;
        }
        public CheckingAccount(int kontoNummer, string name, decimal balance) : base(kontoNummer, name, balance)
        {

        }

        public override void ChargeInterest()
        {
            balance *= 1.005M;
        }
    }
}
