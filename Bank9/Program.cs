using System;
using System.Collections.Generic;
using System.Globalization;
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
        //Har kun 1 try catch. Men har meget andet fejlhåndtering
        static void Main(string[] args)
        {
            bool loop = true;
            //Opretter selve Bank opjektet
            Bank bankOpject = new Bank("Hund banken");
            bankOpject.LogHandlerEvent += Bank_LogHandlerEvent;
            Console.WriteLine("* Velkommen til " + bankOpject.bankName);
            do
            {
                int kontoNummer = 0;
                decimal sum = 0M;
                Menu();
                //Gemmer brugere indtastning i en streng og laver det til upper case som der switches på
                string menuValg = Console.ReadLine().ToUpper();

                #region Menu switch case
                switch (menuValg)
                {
                    case "M":
                        Console.Clear();
                        Menu();
                        break;

                    //Giver dig en menu for oprettelse af en account
                    case "O":
                        Console.Clear();
                        CreateAccountMenu(bankOpject);
                        break;

                    //Køre metoden som går ind og indsætter et beløb på en konto
                    case "I":
                        Console.Clear();
                        DepositToBankAccount(kontoNummer, sum, bankOpject);
                        break;

                    //Køre metoden som går ind og hæver et ønsket beløb fra en konto
                    case "H":
                        Console.Clear();
                        WithdrawFromBankAccount(kontoNummer, sum, bankOpject);
                        break;

                    //Tilskriver renter
                    case "R":
                        Console.Clear();
                        Task renter = new Task(bankOpject.ChargeInterest);
                        renter.Start();
                        renter.Wait();
                        Console.WriteLine("Renter er blevet tilskrevet");
                        Bank_LogHandlerEvent($"{DateTime.Now}. Banken har taget renter");
                        break;

                    //Udskriver en brugers saldo
                    case "S":
                        Console.Clear();
                        ShowAccountBallance(kontoNummer, bankOpject);
                        break;

                    //Udskriver en liste med accounts
                    case "A":
                        Console.Clear();
                        PrintList(bankOpject, bankOpject.GetAccountListForPosting());
                        break;

                    //Udskriver bankens beholdning
                    case "B":
                        Console.Clear();
                        Console.WriteLine($"* Velkommen til {bankOpject.bankName}\nBankens beholdning er på: {bankOpject.BankBalance.ToString("c")}");
                        break;

                    //Udskriver loggen
                    case "G":
                        Console.Clear();
                        Console.WriteLine(FileLogger.ReadFromLog());
                        break;

                    //Gemmer banken
                    case "U":
                        Console.Clear();
                        SaveBankWithTimer(bankOpject);
                        break;

                    //Afslutter lykken
                    case "X":
                        Console.Clear();
                        loop = false;
                        break;

                    default:
                        Console.Clear();
                        break;
                }

            } while (loop);
            #endregion

            #region Gemmer baken når den lukkes
            //Når banken er gemt lukkes programmet
            SaveBankWithTimer(bankOpject);
            Console.WriteLine("Tak for denne gang");

            #endregion

        }
        #region Metoder

        static void Menu()
        {
            //skriver menuen 
            Console.WriteLine("\nVælg en venligst en funktion.\n* M = Menu\n* O = Opret ny konto\n* I = Indsæt beløb\n* H = Hæv beløb\n* R = Rentetilskrivning\n* S = Vis saldo\n* A = Alle konti vises\n* B = Vis Bank\n* G = Vis log\n* U = Gem Banken\n* X = Afslut");
        }

        static void Bank_LogHandlerEvent(string message)
        {
            //skriver til loggen "Kræver en string"
            FileLogger.WriteToLog(message);
        }

        static void SaveBankWithTimer(Bank bankOpject)
        {
            //køre en Task som gemmer banken og logger det med tidspunkt
            Task saveBank = Task.Run(() => SaveBankAndLog(bankOpject));
            int savingProgress = 0;
            //køres imens banken gemmes og giver en primitiv progress "bar"
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
        }

        static void SaveBankAndLog(Bank bankOpject)
        {
            //gemmer banken og logger 
            bankOpject.SaveBank();
            Bank_LogHandlerEvent($"{DateTime.Now}. Gemmer banken");
        }

        static string CreateChosenAccount(Bank bankOpject, string navn, AccountType accountType)
        {
            //opretter en account og sender et account opject tilbage
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
            //string sendes tilbage til at udfylde loggen
            return $"{DateTime.Now}. Konto oprettet: Ejer navn: {navn}. Type konto: {type}. Konto Nr.: {nyAccount.AccountNumber}.";
        }

        static async void PrintList(Bank bank, List<AccountListItem> accList)
        {
            //afventer en Task som udskriver en liste med alle accounts
            await bank.GetAccountListString(accList);
        }

        //Metoden her er lavet for at ryde op i hovedmenuen
        static void CreateAccountMenu(Bank bankOpject)
        {
            Console.WriteLine("Du er ved at oprette en konto\nIndtast konto ejerens navn:");
            string kontoNavn = Console.ReadLine();
            Console.WriteLine("Hvilken type konto Skal det være?\n1. = Lønkonto\n2. = Opsparingskonto\n3. = Forbrugskonto\nEllers tryk en vilkårlig tast for at afbryde.");
            char kontoType = Convert.ToChar(Console.Read());
            Console.Clear();
            //Køre den task som er relavant for brugerens indtastning
            switch (kontoType)
            {
                case '1':
                    Task<string> nyLKonto = Task.Run(() => CreateChosenAccount(bankOpject, kontoNavn, AccountType.checkingAccount));
                    //Logger når der laves en ny account
                    Task continueLKonto = nyLKonto.ContinueWith(fL => Bank_LogHandlerEvent(fL.Result));
                    break;

                case '2':
                    Task<string> nyOKonto = Task.Run(() => CreateChosenAccount(bankOpject, kontoNavn, AccountType.savingsAccount));
                    //Logger når der laves en ny account
                    Task continueOKonto = nyOKonto.ContinueWith(fO => Bank_LogHandlerEvent(fO.Result));
                    break;

                case '3':
                    Task<string> nyFKonto = Task.Run(() => CreateChosenAccount(bankOpject, kontoNavn, AccountType.masterCardAccount));
                    //Logger når der laves en ny account
                    Task continueFKonto = nyFKonto.ContinueWith(fF => Bank_LogHandlerEvent(fF.Result));
                    break;

                default:
                    Console.WriteLine("Du har afbrudt oprettelse af ny konto");
                    break;
            }
        }

        static void ShowAccountBallance(int kontoNummer, Bank bankOpject)
        {
            Console.WriteLine("Skriv dit kontonummer");
            //Prøver at parse bruger indtastning ind i en int
            if (int.TryParse(Console.ReadLine(), out kontoNummer))
            {
                Console.Clear();
                //En annonym Task som køres med det samme og skriver i konsollen
                new Task(() => Console.WriteLine($"Din saldo er: {bankOpject.Balance(kontoNummer).ToString("c")}")).Start();
            }
            else
            {
                Console.WriteLine("Ugyldig indtastning");
            }
        }

        static async void DepositToBankAccount(int kontoNummer, decimal sum, Bank bankOpject)
        {
            Console.WriteLine("Skriv dit kontonummer");
            //Prøver at parse bruger indtastning ind i en int
            if (int.TryParse(Console.ReadLine(), out kontoNummer))
            {
                Console.WriteLine("Indtast beløb, der skal indsættes:");
                //prøver at parse bruger indtasning ind i en decimal
                if (decimal.TryParse(Console.ReadLine(), out sum))
                {
                    Console.Clear();
                    //Håndtere at du ikke indsætter negativt beløb ind på kontoen.
                    if (sum < 0)
                    {
                        Console.WriteLine("Du kan ikke indsætte et negativt beløb");
                    }
                    else
                    {
                        //Tasken køre async med Deposit metoden inde i Bank og retunere en task string
                        Task<string> deposit = bankOpject.Deposit(sum, kontoNummer);
                        await deposit;
                        //Logger til log filen med den string der kommer fra deposit Tasken
                        Bank_LogHandlerEvent(deposit.Result);
                    }
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

        static async void WithdrawFromBankAccount(int kontoNummer, decimal sum, Bank bankOpject)
        {
            try
            {
                Console.WriteLine("Skriv dit kontonummer");
                //Prøver at parse bruger indtastning ind i en int
                if (int.TryParse(Console.ReadLine(), out kontoNummer))
                {
                    Console.WriteLine("Indtast beløb, der skal hæves:");
                    //Prøver at parse bruger indtastning ind i en decimal
                    if (decimal.TryParse(Console.ReadLine(), out sum))
                    {
                        Console.Clear();
                        //tasken køre async med Withdraw metoden inde i Bank og retunere en Task string
                        Task<string> withdraw = bankOpject.Withdraw(sum, kontoNummer);
                        await withdraw;
                        //Logger log filen med den string den får fra withdraw Tasken
                        Bank_LogHandlerEvent(withdraw.Result);
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
            catch (OverdraftException e)
            {
                //Prøver man at hæve mere end hvad der står på kontoen udskrives denne fejl. Bliver også logget inde i Bank
                Console.WriteLine("Du kan ikke hæve det indtastede beløb. Din saldo er: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion
    }
}
