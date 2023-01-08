﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void PrintList(List<Expense> expenses)
        {
            foreach (var e in expenses)
            {
                Console.WriteLine($"\tID: {e.Id}");
                Console.WriteLine($"\tCity: {e.City}");
                Console.WriteLine($"\tRegion: {e.Region}");
                Console.WriteLine($"\tYear: {e.Year}");
                Console.WriteLine($"\tExpenses per month:");
                foreach (var ex in e.ExpensesPerMonth)
                {
                    Console.WriteLine($"\t\t{ex.Key} : {ex.Value}");
                }
            }
        }
        static void Main(string[] args)
        {
            string key = "yo mama"; //OVO POPRAVITI XD

            NetTcpBinding binding = new NetTcpBinding();
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9998/WCFService"));
            WCFClient proxy = new WCFClient(binding,address);

            Console.WriteLine("WELCOME TO EXPENSE PROGRAM!\n\n");

            while (true)
            {
                Console.WriteLine("1 - Read expense data");  //Read
                Console.WriteLine("2 - Get average value for region"); //Read + Calculate
                Console.WriteLine("3 - Get average value for city");
                Console.WriteLine("4 - Update expense value for month"); //Read + Modify
                Console.WriteLine("5 - Add new expense"); //Read + Admin
                Console.WriteLine("6 - Delete existing expense");
                Console.WriteLine("7 - Quit");
                Console.WriteLine("Choose command:");                

                int commandNumber = int.Parse(Console.ReadLine());

                switch (commandNumber)
                {
                    case 1:                        
                        //NEEDS FIXING PROBABLY
                        byte[] encodedData = proxy.ReadData();
                        List<Expense> expenses = AES.Decrypt<List<Expense>>(encodedData, key);
                        PrintList(expenses);
                        break;
                    case 2:
                        Console.WriteLine("Expense region:");
                        string regionAvg = Console.ReadLine();
                        Console.WriteLine("Encrypting regionAvg for safety reasons...");
                        byte[] cryptedRegionAvg = AES.Encrypt(regionAvg, key);
                        byte[] encodedDataRegionAvg = proxy.GetAverageValueForRegion(cryptedRegionAvg);
                        Console.WriteLine("Decrypting recieved data for region average...");
                        double dataRegionAvg = AES.Decrypt<double>(encodedDataRegionAvg, key);
                        Console.WriteLine("Average expense for region: " + regionAvg + " is " + dataRegionAvg + ".");
                        break;
                    case 3:
                        Console.WriteLine("Expense city:");
                        string cityAvg = Console.ReadLine();
                        Console.WriteLine("Encrypting cityAvg for safety reasons...");
                        byte[] cryptedCityAvg = AES.Encrypt(cityAvg, key);
                        byte[] encodedDataCityAvg = proxy.GetAverageValueForCity(cryptedCityAvg);
                        Console.WriteLine("Decrypting recieved data for city average...");
                        double dataCityAvg = AES.Decrypt<double>(encodedDataCityAvg, key);
                        Console.WriteLine("Average expense for city: " + cityAvg + " is " + dataCityAvg + ".");
                        break;
                    case 4:
                        Console.WriteLine("Expense ID:");
                        string idUpdate = Console.ReadLine();
                        Console.WriteLine("New month expense usage:");
                        double usage = double.Parse(Console.ReadLine());
                        Console.WriteLine("Encrypting month expense usage for safety reasons...");
                        byte[] idUpdateEncrypted = AES.Encrypt(idUpdate, key);
                        byte[] usageEncrypted = AES.Encrypt(usage, key);
                        proxy.UpdateCurrentMonthUsage(usageEncrypted, idUpdateEncrypted);
                        break;
                    case 5:
                        Console.WriteLine("Expense ID:");
                        string id = Console.ReadLine();
                        Console.WriteLine("Expense region:");
                        string region = Console.ReadLine();
                        Console.WriteLine("Expense city:");
                        string city = Console.ReadLine();
                        Console.WriteLine("Expense year:");
                        uint year = uint.Parse(Console.ReadLine());
                        Dictionary<int, double> temp = new Dictionary<int, double>();
                        while (true)
                        {
                            Console.WriteLine("Enter 'x' to stop entering values");
                            Console.WriteLine("Enter month in number [1-12]: ");
                            string s = Console.ReadLine();
                            if (s == "x")
                                break;
                            int month = int.Parse(s);
                            if(month < 1 || month > 12)
                            {
                                Console.WriteLine("Enter valid month number");
                                continue;
                            }
                            Console.WriteLine("Enter usage value for " + month + ". month: ");
                            double usageMonth = double.Parse(Console.ReadLine());
                            if(usageMonth < 0)
                            {
                                Console.WriteLine("Enter a valid usage");
                                continue;
                            }
                            temp.Add(month, usageMonth);
                        }
                        Expense expense = new Expense(id, region, city, year, temp);
                        Console.WriteLine("Encrypting expense for safety reasons...");
                        byte[] cryptedExpense = AES.Encrypt(expense, key);
                        proxy.AddNew(cryptedExpense);
                        break;
                    case 6:
                        Console.WriteLine("Expense ID:");
                        string idDelete = Console.ReadLine();
                        Console.WriteLine("Encrypting delete ID for safety reasons...");
                        byte[] idDeleteEncrypted = AES.Encrypt(idDelete, key);
                        proxy.DeleteExpense(idDeleteEncrypted);
                        break;
                    case 7:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Please choose valid option.");
                        break;
                }
            }
        }
    }
}
