using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankClassLibrary.Models
{
    public class AccountListItem
    {
        public AccountListItem(Account account)
        {
            AccountNumber = account.AccountNumber;
            Name = account.Name;
            Balance = account.Balance;
            AccountType = account.AccountType;
        }
        public int AccountNumber { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public AccountType AccountType{ get; set; }
    }
}
