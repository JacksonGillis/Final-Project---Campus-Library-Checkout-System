using ConsoleApp5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    internal class CheckOutItem
    {

        public List<Lists> Items { get; set; } 
        public List<int> DayCheckedOut { get; set; }                                 // gets and sets both of these Lists.

        public CheckOutItem()
        {
            Items = new List<Lists>();                              // Makes the checkout items a list of the Lists class
            DayCheckedOut = new List<int>();                        // Makes the dayCheckedOut list. (a parallel list to the Items list.)
        }

        public void DisplayItems()
        {
            foreach (var item in Items)
            {
                Console.WriteLine(item.Display());              // Displays the entire Items list.
            }
        }

        public int DueDate(int input)
        { return DayCheckedOut[input] + 5; }                       // Determines the due date by adding 5 to the days checked out (Does it for a specefic item on the list)

        public decimal Price(int daysLate, int input)
        {
            decimal price = Items[input - 1].DailyLate(daysLate);               // Determines the price for a single item by calling a function in the Lists class that does the daysLate multiplied by the dailylatefee.
            Console.WriteLine($"{Items[input - 1].Display()}        You owe ${price}");              //Displays the item and the price they owe.
            return price;                                                                    // Returns the price for the item so it can be added up.
        }
       
    }
}
