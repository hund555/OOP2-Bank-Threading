using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankClassLibrary.Models
{
    public abstract class Account
    {
        protected int accountNumber;
        protected string name;
        protected decimal balance;
        public AccountType AccountType{ get; set; }
        public Account(int kontoNummer, string name, decimal balance)
        {
            accountNumber = kontoNummer;
            this.name = name;
            this.balance = balance;
        }
        public Account()
        {

        }
        public int AccountNumber { get { return this.accountNumber; } set { accountNumber = value; } }
        public string Name { get { return this.name; } }
        public decimal Balance
        {
            get { return this.balance; }
            set
            {
                //Rammes af både Withdraw og deposit metoderne men det er kun muligt for withdraw at komme ind
                if (balance < (value * -1))
                {
                    //Denne Exeption kastes hvis en bruger prøver at hæve mere end hvad der står på kontoen
                    throw new OverdraftException(balance.ToString("c"));
                }
                else
                {
                    this.balance += value;
                }
            }
        }
        public abstract void ChargeInterest();
    }
}
