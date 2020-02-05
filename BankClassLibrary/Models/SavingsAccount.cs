using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankClassLibrary.Models
{
    class SavingsAccount : Account
    {
        public SavingsAccount(string name)
        {
            this.AccountType = AccountType.savingsAccount;
            this.name = name;
        }
        public SavingsAccount(int kontoNummer, string name, decimal balance) : base(kontoNummer, name, balance)
        {

        }

        public override void ChargeInterest()
        {
            //Når renter tilskrives får alle accounts af denne type nedenstående renter
            if (balance < 50000)
            {
                balance *= 1.01M;
            }
            else if (balance < 100000)
            {
                balance *= 1.02M;
            }
            else
            {
                balance *= 1.03M;
            }
        }
    }
}
