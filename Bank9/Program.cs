using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankClassLibrary.Models;
using BankClassLibrary.Repository;
using BankClassLibrary.Utilities;

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
                Account denneAccount;
                string menuValg = Console.ReadLine().ToUpper();
                switch (menuValg)
                {
                    case "M":
                        Console.Clear();
                        menu();
                        break;
                    case "L":
                        Console.Clear();
                        Console.WriteLine("Du er ved at lave en Lønkonto\nIndtast konto ejerens navn:");
                        string lNavn = Console.ReadLine();
                        denneAccount = bankOpject.CreateAccount(lNavn, AccountType.checkingAccount);
                        Console.Clear();
                        Console.WriteLine($"Ny lønkonto er blevet oprettet til {lNavn}. Dit konto nummer er: {denneAccount.AccountNumber}. Din saldo er kr. {denneAccount.Balance}");
                        Bank_LogHandlerEvent($"{DateTime.Now.ToString()}. Lønkonto lavet til: {lNavn}. Kontonummer: {denneAccount.AccountNumber}");
                        break;
                    case "O":
                        Console.Clear();
                        Console.WriteLine("Du er ved at lave en Opsparingskonto\nIndtast konto ejerens navn:");
                        string oNavn = Console.ReadLine();
                        denneAccount = bankOpject.CreateAccount(oNavn, AccountType.savingsAccount);
                        Console.Clear();
                        Console.WriteLine($"Ny opsparingskonto er blevet oprettet til {oNavn}. Dit konto nummer er: {denneAccount.AccountNumber}. Din saldo er kr. {denneAccount.Balance}");
                        Bank_LogHandlerEvent($"{DateTime.Now.ToString()}. Opsparingskonto lavet til: {oNavn}. Kontonummer: {denneAccount.AccountNumber}");
                        break;
                    case "F":
                        Console.Clear();
                        Console.WriteLine("Du er ved at lave en forbrugskonto\nIndtast konto ejerens navn:");
                        string fNavn = Console.ReadLine();
                        denneAccount = bankOpject.CreateAccount(fNavn, AccountType.masterCardAccount);
                        Console.Clear();
                        Console.WriteLine($"Ny forbrugskonto er blevet oprettet til {fNavn}. Dit konto nummer er: {denneAccount.AccountNumber}. Din saldo er kr. {denneAccount.Balance}");
                        Bank_LogHandlerEvent($"{DateTime.Now.ToString()}. Forbrugskonto lavet til: {fNavn}. Kontonummer: {denneAccount.AccountNumber}");
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
                                Console.WriteLine($"Din saldo er kr. {Math.Round(bankOpject.Deposit(sum, kontoNummer), 2)}");
                                Bank_LogHandlerEvent($"{DateTime.Now.ToString()}. {sum} kr. lagt ind på konto nr. {kontoNummer}. nye beløb: {bankOpject.Balance(kontoNummer)}");
                            }
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
                                    Console.WriteLine($"Din saldo er kr. {Math.Round(bankOpject.Withdraw(0 - sum, kontoNummer), 2)}");
                                    Bank_LogHandlerEvent($"{DateTime.Now.ToString()}. Beløb {sum} kr. er blevet hævet fra konto {kontoNummer}. Den nye saldo er: {bankOpject.Balance(kontoNummer)} kr");
                                }
                            }
                        }
                        catch (OverdraftException e)
                        {
                            Console.WriteLine("Du kan ikke hæve det indtastede beløb. Din saldo er kr. " + e.Message);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        break;
                    case "R":
                        Console.Clear();
                        bankOpject.ChargeInterest();
                        Console.WriteLine("Renter er blevet tilskrevet");
                        Bank_LogHandlerEvent("Banken har taget renter");
                        break;
                    case "S":
                        Console.Clear();
                        Console.WriteLine("Skriv dit kontonummer");
                        if (int.TryParse(Console.ReadLine(), out kontoNummer))
                        {
                            Console.Clear();
                            Console.WriteLine($"Din saldo er kr. {Math.Round(bankOpject.Balance(kontoNummer), 2)}");
                        }
                        break;
                    case "A":
                        Console.Clear();
                        foreach (AccountListItem item in bankOpject.GetAccountList())
                        {
                            Console.WriteLine($"Konto ejer: {item.Name}\nKonto type: {item.AccountType}\nKonto saldo: {item.Balance}\nKonto nummer: {item.AccountNumber}\n");
                        }
                        break;
                    case "B":
                        Console.Clear();
                        Console.WriteLine($"* Velkommen til {bankOpject.bankName}\nBankens beholdning er på kr. {bankOpject.BankBalance}");
                        break;
                    case "G":
                        Console.Clear();
                        Console.WriteLine(FileLogger.ReadFromLog());
                        break;
                    case "U":
                        Console.Clear();
                        Console.WriteLine("Indtast kontonummer");
                        if (int.TryParse(Console.ReadLine(), out kontoNummer))
                        {
                            denneAccount = bankOpject.GetAccountOpject(kontoNummer);
                            bankOpject.UpdateAccount(denneAccount);
                        }
                        break;
                    case "X":
                        Console.Clear();
                        loop = false;
                        Console.WriteLine("Tak for denne gang");
                        break;
                    default:
                        Console.WriteLine("Indast venligst en af de nævnte tegn, eller tryk (M) for at få listen igen");
                        Console.Clear();
                        break;
                }
            } while (loop);
            bankOpject.SaveBank();

        }
        static void menu()
        {
            Console.WriteLine("\nVælg en venligst en funktion.\n* M = Menu\n* L = Opret ny lønkonto\n* O = Opret ny opsparingskonto\n* F = Opret ny forbrugskonto\n* I = Indsæt beløb\n* H = Hæv beløb\n* R = Rentetilskrivning\n* S = Vis saldo\n* A = Alle konti vises\n* B = Vis Bank\n* G = Vis log\n* X = Afslut");
        }
        static void Bank_LogHandlerEvent(string message)
        {
            FileLogger.WriteToLog(message);
        }
    }
}
