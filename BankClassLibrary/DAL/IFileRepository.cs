using BankClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankClassLibrary.DAL
{
    interface IFileRepository
    {
        int AddAccount(Account account);
        Account GetAccount(int id);
        List<AccountListItem> GetAccountList();
        List<Account> GetAllAccounts();
        List<Account> LoadBank();
        void SaveBank();
        void UpdateAccount(Account acc);
    }
}
