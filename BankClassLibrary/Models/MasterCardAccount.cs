using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankClassLibrary.Models
{
    class MasterCardAccount : Account
    {
        public MasterCardAccount(string name)
        {
            this.AccountType = AccountType.masterCardAccount;
            this.name = name;
        }
        public MasterCardAccount(int kontoNummer, string name, decimal balance) : base(kontoNummer, name, balance)
        {

        }

        public override void ChargeInterest()
        {
            if (this.balance >= 0)
            {
                this.balance *= 1.01M;
            }
            else
            {
                this.balance *= 1.2M;
            }
        }
    }
}
