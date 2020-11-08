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
        private static readonly RequestServices requestServices = new RequestServices();

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
                Console.WriteLine("2: View your past transfers"); //#5 & #6 use case - log to see past transfers sent/received
                Console.WriteLine("3: View your pending requests"); //#8 & #9 view pending requests and approve/reject
                Console.WriteLine("4: Send TE bucks"); //#4 use case - show list of users, enter userID to transfer to, call MakeTransfer method
                Console.WriteLine("5: Request TE bucks"); //#7 request a transfer
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
                    PrintCurrentBalance();
                }
                else if (menuSelection == 2)//get list of past transfers and then transfer details based on the ID the user selects
                {
                    List<Transfer> t = PrintTransferList();
                    PrintDetailsForTransferId(t);
                }

                else if (menuSelection == 3)//view pending requests and approve/review
                {
                    List<Transfer> t = PrintListOfRequests();
                    int transferId = SelectRequestBasedOnTransferID(t);

                    if(transferId != 0)
                    {
                        int userSelection;
                        bool isSuccessful = false;
                        while (!isSuccessful)
                        {
                            try
                            {
                                Console.WriteLine("1: Approve\n2: Reject\n0: Don't approve or reject\n---------\nPlease choose an option:");
                                userSelection = int.Parse(Console.ReadLine());
                                switch (userSelection)
                                {
                                    case 0:
                                        isSuccessful = true;
                                        break;
                                    case 1:
                                        requestServices.AcceptTransfer(transferId);
                                        isSuccessful = true;
                                        break;
                                    case 2:
                                        requestServices.RejectTransfer(transferId);
                                        isSuccessful = true;
                                        break;
                                    default:
                                        Console.WriteLine("\n******INVALID INPUT******\n");
                                        break;
                                }
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("\n******INVALID TRANSFER ID******\n");
                            }
                        }
                    }       
                }
                else if (menuSelection == 4)//Transfer funds
                {
                    PrintListOfUsers();
                    TransferFunds();              
                }

                else if (menuSelection == 5)//log to see past transfers sent and received
                {
                    PrintListOfUsers();
                    RequestATransfer();
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

        //--------------------------------------------------------------------
        //private static methods: for better organization in the program class
        //--------------------------------------------------------------------

        private static int SelectRequestBasedOnTransferID(List<Transfer> t)
        {
            int inputID = -1;
            bool inputIdInList = false; //determining if inputId is in the Transfer List "t"
            while (inputID != 0)
            {
                try
                {
                    Console.WriteLine("-------------------------------------------\nPlease enter transfer ID to approve / reject(0 to cancel): ");
                    inputID = int.Parse(Console.ReadLine());
                    if (inputID == 0)
                    {
                        continue;
                    }

                    foreach (Transfer transfer in t)//determining if inputId is in the Transfer List "t"
                    {
                        if (inputID == transfer.TransferId)
                        {
                            inputIdInList = true;
                        }
                    }

                    if (!inputIdInList)//if inputID is not in list.... inform user
                    {
                        Console.WriteLine("\n******INVALID TRANSFER ID******\n");
                        continue;
                    }
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("\n******INVALID TRANSFER ID******\n");
                }
            }
            return inputID;
        }

        private static List<Transfer> PrintListOfRequests()
        {
            Console.WriteLine("\n-------------------------------------------\nPending Transfers");
            Console.WriteLine(String.Format("{0, 0}\t|  {1,-18}  {2,2} ", "ID", "TO", "|  Amount"));
            Console.WriteLine("-------------------------------------------");

            List<Transfer> t = requestServices.GetTransfers();
            foreach (Transfer transfer in t)
            {
                if (transfer.UserFromId == UserService.GetUserId())
                {
                    Console.WriteLine(String.Format("{0, 0}\t|  {1,-18}  {2,2:C} ", transfer.TransferId, "To: " + transfer.UserNameTo, "|  " + transfer.Amount.ToString("C2")));
                }
            }

            return t;
        }

        private static void RequestATransfer()
        {
            int userId;
            decimal inputAmount;
            while (true)
            {
                try
                {
                    Console.WriteLine("Enter ID of user you are requesting from(0 to cancel):");
                    userId = int.Parse(Console.ReadLine());

                    Console.WriteLine("Enter amount:");
                    inputAmount = decimal.Parse(Console.ReadLine());
                    requestServices.RequestTransfer(userId, inputAmount);
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("\n******INVALID INPUT******\n");
                }
            }
        }

        private static void PrintCurrentBalance()
        {
            Account account = accountServices.GetAccount();
            Console.WriteLine("Your account balance is: " + account.Balance);
        }

        private static void TransferFunds()
        {
            int recipientID;
            decimal inputAmount;
            int userSelection = -1;
            while (userSelection != 0)
            {
                try
                {
                    Console.WriteLine("Enter ID of user you are sending to(0 to cancel):");
                    recipientID = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter amount:");
                    inputAmount = decimal.Parse(Console.ReadLine());
                    transferServices.TransferMoney(recipientID, inputAmount);
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("\n******INVALID INPUT******\n");
                }
            }
        }

        private static void PrintListOfUsers()
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
        }

        private static void PrintDetailsForTransferId(List<Transfer> t)
        {
            int inputID = -1;
            bool inputIdInList = false; //determining if inputId is in the Transfer List "t"
            while (inputID != 0)
            {
                try
                {
                    Console.WriteLine("-------------------------------------------\nPlease enter transfer ID to view details (0 to cancel):");
                    inputID = int.Parse(Console.ReadLine());
                    foreach(Transfer transfer in t)//determining if inputId is in the Transfer List "t"
                    {
                        if(inputID == transfer.TransferId)
                        {
                            inputIdInList = true;
                        }
                    }
                    if (!inputIdInList)//if inputID is not in list.... inform user
                    {
                        Console.WriteLine("\n******INVALID TRANSFER ID******\n");
                        continue;
                    }
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("\n******INVALID TRANSFER ID******\n");
                }
            }
           
            foreach (Transfer transfer in t)
            {
                if (inputID == transfer.TransferId)
                {
                    inputIdInList = true;
                    Console.WriteLine("--------------------------------------------\nTransfer Details\n--------------------------------------------");
                    Console.WriteLine("Id : " + inputID);
                    Console.WriteLine("From: " + transfer.UserNameFrom);
                    Console.WriteLine("To: " + transfer.UserNameTo);

                    switch (transfer.TransferId)
                    {
                        case 1:
                            Console.WriteLine("Type: Request");
                            break;
                        case 2:
                            Console.WriteLine("Type: Send");
                            break;
                    }
                    switch (transfer.TransferStatusId)
                    {
                        case 1:
                            Console.WriteLine("Status: Pending");
                            break;
                        case 2:
                            Console.WriteLine("Status: Approved");
                            break;
                        case 3:
                            Console.WriteLine("Status: Rejected");
                            break;
                    }
                    Console.WriteLine("Amount: " + transfer.Amount.ToString("C2"));
                    Console.WriteLine("--------------------------------------------");
                }
            }
        }

        private static List<Transfer> PrintTransferList()
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
            return t;
        }
    }
}
