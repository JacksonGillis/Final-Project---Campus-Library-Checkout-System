using ConsoleApp5;
using System.Collections.Concurrent;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace ConsoleApp5
{
    internal class Program
    {
        static void Main(string[] args)
        {

            List<Lists> list = SetLists();                      // Creates a list of 4 Variables which are derived from a file
            CheckOutItem checkout = new CheckOutItem();         // Creates a new instance of the checkout item class and assigns it to the variable. The constructor of this class creates 2 lists.

            Welcome(list);                                      // Welcomes the User
            ChooseMenu(list, checkout);                         // Where the user can choose functions, the most import function of the code.
            
        }


        public static List<Lists> SetLists()
        {
            List<Lists> Lists = new List<Lists>();                               // Creates a list of the Lists function, which stores 4 variables.
            foreach (var line in File.ReadAllLines("LibraryItems.txt"))
            {
                string[] split = line.Split("|");                               // Creates an array of strings for each line in the file.
                int ID = Convert.ToInt32(split[0]);                              
                string Title = split[1];
                string Type = split[2];
                decimal DailyLateFee = Convert.ToDecimal(split[3]);
                Lists.Add(new Lists(ID, Title, Type, DailyLateFee));          // Adds to the Lists class after creating a new instance of the class so the variables are different.
            }
            return Lists;
        }


        public static int SetDate()
        {
            try 
            {
                Console.WriteLine("\nPlease enter what day it is (0-15)");
                int Day = Convert.ToInt32(Console.ReadLine());
                if ((Day < 0) || (Day > 15)) { throw new FormatException(); }                                    // Finds the current day so it can be used to calculate the time that passes later
                return Day;                                                              
            } 
            catch (FormatException) { Console.WriteLine("Incorrect day."); return SetDate(); }                   // Ensures that the code doesnt break if the user inputs an incorrect value.
        }


        public static void Welcome(List<Lists> list)
        {
            Console.WriteLine("Welcome to the Library! \n------------------------------------------------ \nhere are the options we have available!");
            Console.WriteLine("\nId | Title | Type | Daily Late Fee \n ");
            DisplayItem(list);                                                                                // A function to display a list of the 4 variables in the list class
        }


        public static void DisplayItem(List<Lists> list)
        {
            List<string> Available = Availability();                                                         // Determines which items are checked out and not
            int items = -1;                                               
            foreach (Lists item in list)
            {
                items++;                                                                             // Starts at zero because its -1 + 1.
                Console.WriteLine($"{item.Display()} | {Available[items]}");                           // Uses the Lists class to display the 4 items per value in the item list, then it shows if it is available or not.
            }
            Console.WriteLine("\nPress any key to continue!");
            Console.ReadKey();
        }


        public static int MainMenu()
        {
            MenuText();                                                                               // Portrays the options for the menu.
            try
            {
                Console.WriteLine("Please enter the option you want to select!");
                var Input = Convert.ToInt32(Console.ReadLine());
                if ((Input < 1) || (Input > 8)) { throw new FormatException(); }                     // This entire function is just to ensure that the user inputs a correct number for the menu, so if it is out of range it raises an error.
                return Input;
            }
            catch (FormatException) { Console.WriteLine("Incorrect Input, press any ket to continue"); Console.ReadKey(); return MainMenu(); }

        }


        public static void MenuText()                                    // Just shows all the options.
        {
            ClearScreen();
            Console.WriteLine("===============================================================");
            Console.WriteLine("\n 1. Add a library item");
            Console.WriteLine("\n 2. View available items");
            Console.WriteLine("\n 3. Check out an item");
            Console.WriteLine("\n 4. Return an item");
            Console.WriteLine("\n 5. View my checkout receipt");
            Console.WriteLine("\n 6. Save my checkout list to file");
            Console.WriteLine("\n 7. Load my previous checkout list from file");
            Console.WriteLine("\n 8. Exit");
            Console.WriteLine("===============================================================");
        }


        public static void ClearScreen()                               //Simply creates 30 new lines to clear the screen.
        {
            for (int i = 0; i < 30; i++)
            { Console.WriteLine("\n"); }
        }


        public static void ChooseMenu(List<Lists> list, CheckOutItem checkout)
        {
            int CurrentDay = SetDate();                        // Sets the current day to determine what day the user checks out items.
            int UserInput = MainMenu();
            decimal TotalPrice = 0;                            // Adds to it if they use the return function and are late.
            List<string> Available = Availability();
            while (UserInput != 8)
            {
                if (UserInput == 1)
                { AddItem(list); UserInput = MainMenu(); }                        
                if (UserInput == 2)
                { DisplayItem(list); UserInput = MainMenu(); }
                if (UserInput == 3)
                { Checkout(list, CurrentDay, checkout); UserInput = MainMenu(); }
                if (UserInput == 4)
                { TotalPrice = ReturnItem(list, CurrentDay, checkout); UserInput = MainMenu(); }          // The total price equals this function as it is later used to find the reciepts total return price.
                if (UserInput == 5)
                { Receipt(checkout, TotalPrice, CurrentDay); UserInput = MainMenu(); }
                if (UserInput == 6)
                { Save(checkout); UserInput = MainMenu(); }
                if (UserInput == 7)
                { Load(checkout); UserInput = MainMenu(); }
            }
            Console.WriteLine("Thank you for using my program!");
            File.WriteAllText("Availability.txt", "");
            foreach (var item in Available)
            {
                    File.AppendAllText("Availability.txt", "true\n");               // Sets the entire list to Available after they are done using. If they decide to load, it will set the Unavailable items unavailable again.
            }
        }


        public static List<string> Availability()
        {
            List<string> Availability = new List<string>(); 
            foreach (var line in File.ReadAllLines("Availability.txt"))                   // Creates a parallel list to list that determines if the items have been checked out or not.
            {
                if (line.Trim() == "true")
                { Availability.Add("Available"); }
                else
                { Availability.Add("Unavailable"); }
            }
            return Availability;
        }


        public static void AddItem(List<Lists> list)                       // Synonomous to if someone would donate something to a real library. 
        {
            try
            {
                bool Continue = true;
                while (Continue == true)
                {
                    int ID = list.Count() + 1;                                          // The ID system increases by 1 each time.
                    Console.WriteLine("What is the name of the title you want to add?");
                    string Title = Console.ReadLine();
                    Console.WriteLine("What type is this item? (Book, DVD, Journal, News Article)");
                    string Type = Console.ReadLine();
                    if ((Type.ToUpper() != "BOOK") && (Type.ToUpper() != "DVD") && (Type.ToUpper() != "JOURNAL") && (Type.ToUpper() != "NEWS ARTICLE"))
                    { throw new FormatException(); }                             // Ensures that the type is one of the four provided.
                    decimal DailyLateFee = Determine(Type);                       // Correlates the daily late fee with what type the object is
                    list.Add(new Lists(ID, Title, Type, DailyLateFee));          // Adds the 4 variables to the list.
                    Console.WriteLine("Do you want to keep going? (Y/N)");
                    string decide = Console.ReadLine();
                    if (decide.ToUpper() == "N")
                    {
                        Continue = false;                                     // Asks if they want to keep adding or not.
                    }
                    File.AppendAllText("LibraryItems.txt", $"\n{list.Last().Display()}");          // Adds the last instance of the list to the file and it displays it as it is in the same format as the file.

                    File.AppendAllText("Availability.txt", $"\ntrue");                            // Adds that the availability for this is true since it just got added.
                }
            }
            catch (FormatException) { Console.WriteLine("Invalid Input"); AddItem(list); }
        }


        public static decimal Determine(string Type)         // Determines the daily late fee based off the type
        {
            decimal DailyLateFee = 0;
            if (Type.ToUpper() == "BOOK")
            { DailyLateFee = 0.50m; }
            if (Type.ToUpper() == "DVD")
            { DailyLateFee = 0.75m; }
            if (Type.ToUpper() == "JOURNAL") 
            { DailyLateFee = 1.50m; }                       // All these use m so its a decimal and not a double.
            if (Type.ToUpper() == "NEWS ARTICLE")
            { DailyLateFee = 1.25m; }
            return DailyLateFee;

        }


        public static void Checkout(List<Lists> list, int CurrentDay, CheckOutItem checkout)
        {
            try
            {
                List<string> Avalaible = Availability();
                bool Continue = true;
                while (Continue == true)
                {
                    DisplayItem(list);
                    int Input = Inputs(list);                                // Ensures that the input doesnt raise an error.
                    if (Avalaible[Input - 1] == "Unavailable")               // Ensures that the item is available.
                    {
                        Console.WriteLine("That item is already checked out. Please select another item!"); break;                  // This breaks instead of doing a recurssion as doing a recurssion breaks it.
                    }

                    checkout.Items.Add(list[Input - 1]);                         // Takes the users input of the Lists class list and adds it to the checkout items list.
                    checkout.DayCheckedOut.Add(CurrentDay);                      // Takes the current day and adds it to a parallel list int he checkoutitem class to determine when the user checked out the item.
                    Avalaible[Input - 1] = "Unavailable";
                    Console.WriteLine("Do you want to checkout another item? (Y/N)");
                    string decide = Console.ReadLine();
                    if (decide.ToUpper() == "N")
                    {
                        Continue = false;                               // Determines if the user wants to keep going or not
                    }
                    checkout.DisplayItems();                            // Displays the checked out items
                    File.WriteAllText("Availability.txt", "");
                    for (int i = 0; i < Avalaible.Count(); i++)
                    {
                        if (Avalaible[i] == "Available")
                        { File.AppendAllText("Availability.txt", "true\n"); }
                        else
                        { File.AppendAllText("Availability.txt", "false\n"); }                     // Rewrited the file based off if they checked out the item or not

                    }
                    Console.WriteLine($"You have until day {CurrentDay + 5} to return this item!");              // Tells them when the duedate is

                }
                Console.WriteLine("Press any key to continue!");
                Console.ReadKey();
            }
            catch { new FormatException(); Console.WriteLine("Incorrect Input"); Checkout(list, CurrentDay, checkout); }

        }


        public static int Inputs(List<Lists> list)
        {
            Console.WriteLine($"\nPlease select an item you want to checkout! (1 - {list.Count()})");
            int Input = Convert.ToInt32(Console.ReadLine());
            if ((Input < 1) || (Input > list.Count())) { throw new FormatException(); }                    // Ensures that the input is correct.
            return Input;
        }


        public static decimal ReturnItem(List<Lists> list, int CurrentDay, CheckOutItem checkout)
        {
            try
            {
                bool Continue = true;
                int newDay = SetDate();                          // Creates a new day that is meant to repersent the future when the person is returning their item
                decimal totalPrice = 0;                          // Adds to the total price later for the reciept function.
                while (Continue == true)
                {
                    
                    if (newDay < CurrentDay) { Console.WriteLine("You can't time travel."); break; }            // Ensures that the new day isnt before the day that they checked out the item.
                    if (checkout.Items.Count() == 0)                                                            // Ensures that the user has items to return
                    {
                        Console.WriteLine("You have no items to return! Press any key to continue!");
                        Console.ReadKey();
                        break; 
                    }
                    for (int i = 0; i < checkout.Items.Count(); i++)
                    {
                        Console.WriteLine($"{i + 1} --------- {checkout.Items[i].Display()}");                  // Shows all the items that can be returned by showcasing the item list in the checkout class, I listed them 1 to n at the start to ensure that the user input would be accurate.
                    }
                    Console.WriteLine("Please select the number of the item you want to Checkout!");
                    int Input = Convert.ToInt32(Console.ReadLine());
                    if ((Input < 1) || (Input > checkout.Items.Count())) { throw new FormatException(); }
                    int checkedOutDay = checkout.DayCheckedOut[Input - 1];                                     // Finds the day that the item was checked out with the input due to them being parallel lists.
                    Console.WriteLine($"You checked out this item on day {checkedOutDay}.\n");

                    int dueDate = checkout.DueDate(Input - 1);                                                 // Finds the duedate by adding 5 to the date the items were checked out.
                    if (newDay > dueDate)                                                                       // If late... 
                    {
                        int daysLate = newDay - dueDate;                                                        // Determines how late the object is based by subtracting the new day by the due date.
                        decimal price = checkout.Price(daysLate, Input);                                        // Finds the price by calling the a function in the checkoutitem class which calls on a function in the list class which multiplies the days late by the rate per day if late.
                        totalPrice = +price;                                                                    // Finds the total price of the items you returned for the reciept function.
                    }
                    else { Console.WriteLine("Returned on time!"); }
                    checkout.Items.RemoveAt(Input - 1);                                                          // Removes the item from the checked out list
                    Console.WriteLine("Do you want to continue (Y/N)?");
                    string decision = Console.ReadLine();
                    if (decision.ToUpper() == "N")                                                               // Determines if the user wants to continue or not.
                    { Continue = false; } 
                }
                Console.WriteLine("Press any key to continue!");
                Console.ReadKey();
                return totalPrice;                                                                              // Which is later used in the reciept function
                 
            }
            catch
            {
                new FormatException(); Console.WriteLine("Incorrect Input"); return ReturnItem(list, CurrentDay, checkout);
            }
        }


        public static void Save(CheckOutItem checkout)
        {
            Console.WriteLine("Do you want to save your checkout list? (Y/N)");
            string input = Console.ReadLine();
            if (input.ToUpper() == "Y")
            {
                int i = 0;
                File.WriteAllText("CheckoutList.txt", "");                                                   // Creates a new empty file
                File.WriteAllText("DayCheckedOut.txt", "");                                                  // ^^^^^^^^^^^^^^^^^^^^^^^^
                foreach (var item in checkout.Items)                                                   // Parallel lists so they can use the same foreach loop
                {
                    File.AppendAllText("CheckoutList.txt", $"{item.Display()}\n");
                    File.AppendAllText("DayCheckedOut.txt", $"{checkout.DayCheckedOut[i]}\n");
                    i++;                                                                                       // Adds 1 each time so the daycheckedout can be added to the file as well
                }
            }
            
        }


        public static void Load(CheckOutItem checkout)
        {
            List<Lists> CheckOutList = new List<Lists>();                                       // Creates a new list of the Lists class.
            int n = 0;
            Console.WriteLine("Do you want to load your checkout list? (Y/N)");
            string input = Console.ReadLine();
            if (input.ToUpper() == "Y")
            {
                for (int i = 0; i<checkout.Items.Count; i++)
                {
                    checkout.Items.RemoveAt(n);                                                   // Removes all previous items in the checkout Items list incase they decide to load after checking out stuff.
                }
                List<string> Availablilities = Availability();                                  // Creates a new list of the Availability list
                foreach (var line in File.ReadAllLines("CheckoutList.txt"))                                
                {                  
                    string[] split = line.Split("|");
                    int ID = Convert.ToInt32(split[0]);
                    string Title = split[1];
                    string Type = split[2];
                    decimal DailyLateFee = Convert.ToDecimal(split[3]);
                    CheckOutList.Add(new Lists(ID, Title, Type, DailyLateFee));                        // Uses the same method as the SetLists function did
                    checkout.Items.Add(CheckOutList[n]);                                               // Adds that list into the Items list of the checkout items.                                                 
                    n++;
                    Availablilities[ID - 1] = "Unavailable";
                }
                File.WriteAllText("Availability.txt", "");
                foreach (var item in Availablilities)
                {
                if (item == "Available")
                    {
                        File.AppendAllText("Availability.txt", "true\n");                                // Recreates the list based off which items are checked out or not.
                    }
                    else
                    {
                        File.AppendAllText("Availability.txt", "false\n");
                    } 
                        
                }
                foreach (var line in File.ReadAllLines("DayCheckedOut.txt"))
                { checkout.DayCheckedOut.Add(Convert.ToInt32(line)); }                                 // Adds the to the list of when they checked out the item in the checkoutitem class.
            }
        }


        public static void Receipt(CheckOutItem checkout, decimal TotalPrice, int CurrentDay)
        {
            
            ClearScreen();
            int daysLate = 0;
            decimal checkoutPrice = 0;
            int newDay = SetDate();                                                              // Asks for what day they want get the reciept (Incase the user doesnt do it on the day they returned their items)
            if (newDay < CurrentDay) { Console.WriteLine("You can't time travel."); return; }    // Ensures its not less than current day.
            Console.WriteLine("=====================================================");
            Console.WriteLine($"You owe ${TotalPrice} For the items you returned late.");        // Total price is from the return function and its the total of the amount they owe from the items they returned.
            Console.WriteLine("=====================================================");
            for (int i = 0; i < checkout.Items.Count; i++)
            {
                int dueDate = checkout.DueDate(i);                                               // Finds the due date for each item in the checkout list.
                if (newDay > dueDate)
                {
                    daysLate = newDay - dueDate;                                                      // Finds how late each item is                           
                }
                checkoutPrice += checkout.Price(daysLate, i+1);                                     // Prints the item and the amount they owe for that specefic item while also adding up the total.
                
            }
            Console.WriteLine("=====================================================");
            Console.WriteLine($"Your total price is ${TotalPrice + checkoutPrice}");               // The entire price.
            Console.WriteLine("Press any key to continue!");
            Console.ReadKey();
        }
    }
}
