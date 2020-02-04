using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankClassLibrary.Models;
using BankClassLibrary.Repository;
using BankClassLibrary.Utilities;
using System.Threading;

namespace Bank9
{
    class Program
    {
        static void Main(string[] args)
        {
            bool loop;
            Bank bankOpject = new Bank("Hund banken");
            List<AccountListItem> accList = bankOpject.GetAccountListForPosting();
            bankOpject.LogHandlerEvent += Bank_LogHandlerEvent;
            Console.WriteLine("* Velkommen til " + bankOpject.bankName);
            //Console.WriteLine("Vælg en venligst en funktion.\n* M = Menu\n* L = Opret ny lønkonto\n* O = Opret ny opsparingskonto\n* F = Opret ny forbrugskonto\n* I = Indsæt beløb\n* H = Hæv beløb\n* R = Rentetilskrivning\n* S = Vis saldo\n* B = Vis Bank\n* X = Afslut");
            do
            {
                int kontoNummer;
                decimal sum;
                loop = true;
                Menu();
                string menuValg = Console.ReadLine().ToUpper();
                switch (menuValg)
                {
                    case "M":
                        Console.Clear();
                        Menu();
                        break;

                    case "O":
                        Console.Clear();
                        Console.WriteLine("Du er ved at oprette en konto\nIndtast konto ejerens navn:");
                        string kontoNavn = Console.ReadLine();
                        Console.WriteLine("Hvilken type konto Skal det være?\n1. = Lønkonto\n2. = Opsparingskonto\n3. = Forbrugskonto\nEllers tryk en vilkårlig tast for at afbryde.");
                        char kontoType = Convert.ToChar(Console.Read());
                        Console.Clear();
                        switch (kontoType)
                        {
                            case '1':
                                Task<string> nyLKonto = Task.Run(() => CreateAccount(bankOpject, kontoNavn, AccountType.checkingAccount));

                                Task continueLKonto = nyLKonto.ContinueWith(fL => Bank_LogHandlerEvent(fL.Result));
                                break;

                            case '2':
                                Task<string> nyOKonto = Task.Run(() => CreateAccount(bankOpject, kontoNavn, AccountType.savingsAccount));

                                Task continueOKonto = nyOKonto.ContinueWith(fO => Bank_LogHandlerEvent(fO.Result));
                                break;

                            case '3':
                                Task<string> nyFKonto = Task.Run(() => CreateAccount(bankOpject, kontoNavn, AccountType.masterCardAccount));

                                Task continueFKonto = nyFKonto.ContinueWith(fF => Bank_LogHandlerEvent(fF.Result));
                                break;

                            default:
                                Console.WriteLine("Du har nu afbrudt oprettelse af ny konto");
                                break;
                        }
                        
                        break;

                    case "I":
                        Console.Clear();
                        Console.WriteLine("Skriv dit kontonummer");
                        if (int.TryParse(Console.ReadLine(), out kontoNummer))
                        {
                            Console.WriteLine("Indtast beløb, der skal indsættes:");
                            if (decimal.TryParse(Console.ReadLine(), out sum))
                            {
                                Console.Clear();
                                //Thread deposit = new Thread(bankOpject.Deposit(sum, kontoNummer));
                                Task<string> deposit = Task.Run(() => bankOpject.Deposit(sum, kontoNummer));
                                Task continueDeposit = deposit.ContinueWith(fd =>
                                {
                                    Bank_LogHandlerEvent(fd.Result);
                                }, TaskScheduler.Current);
                                Task faltedDeposit = deposit.ContinueWith(falted => Console.WriteLine("Noget gik galt"), TaskContinuationOptions.OnlyOnFaulted);
                                
                            }
                            else
                            {
                                Console.WriteLine("Ugyldig indtastning");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Ugyldig indtastning");
                        }
                        break;

                    case "H":
                        try
                        {
                            Console.Clear();
                            Console.WriteLine("Skriv dit kontonummer");
                            if (int.TryParse(Console.ReadLine(), out kontoNummer))
                            {
                                Console.WriteLine("Indtast beløb, der skal hæves:");
                                if (decimal.TryParse(Console.ReadLine(), out sum))
                                {
                                    Console.Clear();
                                    Task<string> withdraw = Task.Run(() => bankOpject.Withdraw(sum, kontoNummer));
                                    Task continueWithdraw = withdraw.ContinueWith(fw =>
                                    {
                                        Bank_LogHandlerEvent(fw.Result);
                                    }, TaskContinuationOptions.OnlyOnRanToCompletion);

                                    Task faltedWithdraw = withdraw.ContinueWith(falted => Console.WriteLine("Noget gik galt. Check om din konto indeholder det ønskede beløb"), TaskContinuationOptions.OnlyOnFaulted);
                                }
                                else
                                {
                                    Console.WriteLine("Ugyldig indtastning");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Ugyldig indtastning");
                            }
                        }
                        catch (AggregateException e)
                        {
                            Console.WriteLine("Du kan ikke hæve det indtastede beløb. Din saldo er: " + e.Message);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        break;

                    case "R":
                        Console.Clear();
                        Task renter = new Task(bankOpject.ChargeInterest);
                        renter.Start();
                        renter.Wait();
                        Console.WriteLine("Renter er blevet tilskrevet");
                        Bank_LogHandlerEvent($"{DateTime.Now}. Banken har taget renter");
                        break;

                    case "S":
                        Console.Clear();
                        Console.WriteLine("Skriv dit kontonummer");
                        if (int.TryParse(Console.ReadLine(), out kontoNummer))
                        {
                            Console.Clear();
                            new Task(() => Console.WriteLine($"Din saldo er: {bankOpject.Balance(kontoNummer).ToString("c")}")).Start();
                        }
                        break;

                    case "A":
                        Console.Clear();
                        PrintList(bankOpject, accList);
                        break;

                    case "B":
                        Console.Clear();
                        Console.WriteLine($"* Velkommen til {bankOpject.bankName}\nBankens beholdning er på: {bankOpject.BankBalance.ToString("c")}");
                        break;

                    case "G":
                        Console.Clear();
                        Console.WriteLine(FileLogger.ReadFromLog());
                        break;

                    case "U":
                        Console.Clear();
                        Task saveBank = Task.Run(() => SaveBankAndLog(bankOpject));
                        int savingProgress = 0;
                        if (!saveBank.IsCompleted && savingProgress <= 100)
                        {
                            for (int i = 0; i <= 10; i++)
                            {
                                Console.Clear();
                                Console.WriteLine("Gemmer banken. Vent venligst");
                                Console.WriteLine(i * 10 + "%");
                                savingProgress += 10;
                                Thread.Sleep(500);
                            }
                        }
                        Console.WriteLine("Banken er gemt");

                        //Console.WriteLine("Indtast kontonummer");
                        //if (int.TryParse(Console.ReadLine(), out kontoNummer))
                        //{
                        //    denneAccount = bankOpject.GetAccountOpject(kontoNummer);
                        //    bankOpject.UpdateAccount(denneAccount);
                        //}
                        //else
                        //{
                        //    Console.WriteLine("Ugyldig indtastning");
                        //}
                        break;

                    case "X":
                        Console.Clear();
                        loop = false;
                        break;

                    default:
                        Console.WriteLine("Indast venligst en af de nævnte tegn, eller tryk (M) for at få listen igen");
                        Console.Clear();
                        break;
                }

            } while (loop);
            Task saveAndShutdown = Task.Run(() => SaveBankAndLog(bankOpject));
            int Progress = 0;
            if (!saveAndShutdown.IsCompleted && Progress <= 100)
            {
                for (int i = 0; i <= 10; i++)
                {
                    Console.Clear();
                    Console.WriteLine("Gemmer banken. Vent venligst");
                    Console.WriteLine(i * 10 + "%");
                    Progress += 10;
                    Thread.Sleep(500);
                }
            }
            Console.WriteLine("Banken er gemt.\nTak for denne gang");
        }
        static void Menu()
        {
            Console.WriteLine("\nVælg en venligst en funktion.\n* M = Menu\n* O = Opret ny konto\n* I = Indsæt beløb\n* H = Hæv beløb\n* R = Rentetilskrivning\n* S = Vis saldo\n* A = Alle konti vises\n* B = Vis Bank\n* G = Vis log\n* U = Gem Banken\n* X = Afslut");
        }
        static void Bank_LogHandlerEvent(string message)
        {
            FileLogger.WriteToLog(message);
        }

        static void SaveBankAndLog(Bank bankOpject)
        {
            bankOpject.SaveBank();
            Bank_LogHandlerEvent($"{DateTime.Now}. Gemmer banken");
        }

        static string CreateAccount(Bank bankOpject, string navn, AccountType accountType)
        {
            Account nyAccount = bankOpject.CreateAccount(navn, accountType);
            string type = "";
            if (accountType == AccountType.checkingAccount)
            {
                type = "lønkonto";
            }
            else if (accountType == AccountType.masterCardAccount)
            {
                type = "forbrugskonto";
            }
            else if (accountType == AccountType.savingsAccount)
            {
                type = "opsparingskonto";
            }
            Console.WriteLine($"Ny {type} er blevet oprettet til {navn}. Dit konto nummer er: {nyAccount.AccountNumber}. Din saldo er: {nyAccount.Balance.ToString("c")}");
            return $"{DateTime.Now}. Konto oprettet: Ejer navn: {navn}. Type konto: {type}. Konto Nr.: {nyAccount.AccountNumber}.";
        }
        static async void PrintList(Bank bank, List<AccountListItem> accList)
        {

            await bank.GetAccountListString(accList);
        }
    }
}
