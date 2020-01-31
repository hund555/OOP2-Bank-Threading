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
            bankOpject.LogHandlerEvent += Bank_LogHandlerEvent;
            Console.WriteLine("* Velkommen til " + bankOpject.bankName);
            //Console.WriteLine("Vælg en venligst en funktion.\n* M = Menu\n* L = Opret ny lønkonto\n* O = Opret ny opsparingskonto\n* F = Opret ny forbrugskonto\n* I = Indsæt beløb\n* H = Hæv beløb\n* R = Rentetilskrivning\n* S = Vis saldo\n* B = Vis Bank\n* X = Afslut");
            do
            {
                int kontoNummer;
                decimal sum;
                loop = true;
                menu();
                string menuValg = Console.ReadLine().ToUpper();
                switch (menuValg)
                {
                    case "M":
                        Console.Clear();
                        menu();
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
                                Task nyLKonto = new Task(() => CreateAccount(bankOpject, kontoNavn, AccountType.checkingAccount));
                                nyLKonto.Start();
                                break;

                            case '2':
                                Task nyOKonto = new Task(() => CreateAccount(bankOpject, kontoNavn, AccountType.savingsAccount));
                                nyOKonto.Start();
                                break;

                            case '3':
                                Task nyFKonto = new Task(() => CreateAccount(bankOpject, kontoNavn, AccountType.masterCardAccount));
                                nyFKonto.Start();
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
                                Task<decimal> deposit = Task.Run(() => bankOpject.Deposit(sum, kontoNummer));
                                Task continueDeposit = deposit.ContinueWith(fd =>
                                {
                                    Console.WriteLine("Din saldo er: {0}", fd.Result.ToString("c"));
                                    Bank_LogHandlerEvent($"{DateTime.Now.ToString()}. {sum.ToString("c")} kr. lagt ind på konto nr. {kontoNummer}. nye beløb: {bankOpject.Balance(kontoNummer).ToString("c")}");
                                }, TaskScheduler.Current);
                                
                                
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
                                    Task<decimal> withdraw = Task.Run(() => bankOpject.Withdraw(sum, kontoNummer));
                                    Task continueWithdraw = withdraw.ContinueWith(fw =>
                                    {
                                        Console.WriteLine("Din saldo er: {0}", fw.Result.ToString("c"));
                                        Bank_LogHandlerEvent($"{DateTime.Now.ToString()}. Beløb {sum.ToString("c")} er blevet hævet fra konto {kontoNummer}. Den nye saldo er: {bankOpject.Balance(kontoNummer).ToString("c")}");
                                    }, TaskScheduler.Current);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Ugyldig indtastning");
                            }
                        }
                        catch (OverdraftException e)
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
                        foreach (AccountListItem item in bankOpject.GetAccountList())
                        {
                            Console.WriteLine($"Konto ejer: {item.Name}\nKonto type: {item.AccountType}\nKonto saldo: {item.Balance.ToString("c")}\nKonto nummer: {item.AccountNumber}\n");
                        }
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
        static void menu()
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

        static void CreateAccount(Bank bankOpject, string navn, AccountType accountType)
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
            bankOpject.SaveBank();
            Bank_LogHandlerEvent($"{DateTime.Now}. Konto oprettet: Ejer navn: {navn}. Type konto: {type}. Konto Nr.: {nyAccount.AccountNumber}.");
            Console.WriteLine($"Ny {type} er blevet oprettet til {navn}. Dit konto nummer er: {nyAccount.AccountNumber}. Din saldo er: {nyAccount.Balance.ToString("c")}");
        }
    }
}
