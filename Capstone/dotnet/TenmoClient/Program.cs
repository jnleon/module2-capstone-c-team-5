using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TenmoClient.Data;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly AccountServices accountServices = new AccountServices();
        private static readonly ServicesThings servicesThings = new ServicesThings();
        private static readonly TransferServices transferServices = new TransferServices();

        static void Main(string[] args)
        {
            Run();
        }
        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance"); //#3 use case - view current balance
                Console.WriteLine("2: View your past transfers"); //#5 use case - log to see past transfers sent/received
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks"); //#4 use case - show list of users, enter userID to transfer to, call MakeTransfer method
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)//view account balance 
                {
                    Account account = accountServices.GetAccount();
                    Console.WriteLine("Your account balance is: " + account.Balance);
                }
                else if (menuSelection == 2)//get list of past transfers pertaining to my id
                {
                    Console.WriteLine("\n-------------------------------------------\nTransfers");
                    Console.WriteLine(String.Format("{0, 0}\t|  {1,-18}  {2,2} ", "ID", "FROM/TO", "|  Amount"));
                    Console.WriteLine("-------------------------------------------");
                    List<Transfer> t = transferServices.GetTransfers();
                    foreach (Transfer transfer in t)
                    {
                        if (transfer.UserFromId == UserService.GetUserId())
                        {
                            Console.WriteLine(String.Format("{0, 0}\t|  {1,-18}  {2,2:C} ", transfer.TransferId, "To: " + transfer.UserNameTo, "|  " + transfer.Amount.ToString("C2")));
                        }
                        else if (transfer.UserToId == UserService.GetUserId())
                        {
                            Console.WriteLine(String.Format("{0, 0}\t|  {1,-18}  {2,2:C} ", transfer.TransferId, "From: " + transfer.UserNameFrom, "|  " + transfer.Amount.ToString("C2")));
                        }
                    }
                    Console.WriteLine("-------------------------------------------\nPlease enter transfer ID to view details (0 to cancel):");
                    var inputID = int.Parse(Console.ReadLine());

                    foreach (Transfer transfer in t)
                    {
                        if (inputID == transfer.TransferId)
                        {
                            Console.WriteLine("--------------------------------------------\nTransfer Details\n--------------------------------------------");
                            Console.WriteLine("Id : " + inputID);
                            Console.WriteLine("From: " + transfer.UserNameFrom);
                            Console.WriteLine("To: " + transfer.UserNameTo);
                            //CAST ENUM< LOOK AT ENUMS MR RIGGS + TO STRING STUFF
                            if (transfer.TransferTypeId == 1)
                            {
                                Console.WriteLine("Type: Request");
                            }
                            else if (transfer.TransferTypeId == 2)
                            {
                                Console.WriteLine("Type: Send");
                            }
                            if (transfer.TransferStatusId == 1)
                            {
                                Console.WriteLine("Status: Pending");
                            }
                            else if (transfer.TransferStatusId == 2)
                            {
                                Console.WriteLine("Status: Approved");
                            }
                            else if (transfer.TransferStatusId == 3)
                            {
                                Console.WriteLine("Status: Rejected");
                            }
                            Console.WriteLine("Amount: $" + transfer.Amount.ToString("C2"));
                            Console.WriteLine("--------------------------------------------");
                        }
                    }
                }

                else if (menuSelection == 3)
                {

                }
                else if (menuSelection == 4)//Transfer funds
                {
                    List<User> users = servicesThings.GetUsers();
                    Console.WriteLine("-------------------------------------------\nUsers");
                    Console.WriteLine(String.Format("{0, 0}\t|  {1,-18} ", "ID", "NAME"));
                    Console.WriteLine("-------------------------------------------");

                    //prints the list of each user and their ID number
                    foreach (User user in users)
                    {
                        Console.WriteLine(String.Format("{0, 0}\t|  {1,-18} ", user.UserId, user.Username));
                    }
                    Console.WriteLine("-------------------------------------------\n");

                   
                    int recipientID =0 ;
                    decimal inputAmount = 0;
                   
                    bool isSuccessful = false;
                    while (!isSuccessful)
                    {
                        try
                        {
                            Console.WriteLine("Enter ID of user you are sending to(0 to cancel):");
                            recipientID = int.Parse(Console.ReadLine());
                       
                            Console.WriteLine("Enter amount:");
                            inputAmount = decimal.Parse(Console.ReadLine());
                            transferServices.TransferMoney(recipientID, inputAmount);
                            isSuccessful = true;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("\n******INVALID INPUT******\n");
                        }
                    }
                }
                
                else if (menuSelection == 5)
                {

                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Run(); //return to entry point
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
