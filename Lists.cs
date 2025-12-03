using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    internal class Lists
    {
        private int ID;
        private string Title;
        private String Type;
        private Decimal DailyLateFee;


        public Lists(int ID, string Title, String Type, Decimal DailyLateFee)
        {
            this.ID = ID;                     // this constructor gets and sets all the variables.
            this.Title = Title;
            this.Type = Type;
            this.DailyLateFee = DailyLateFee;
        }

        public string Display()
        {
            return $"{ID} | {Title} | {Type} | {DailyLateFee}";                  // Displays the variables in a certain part of the list.
        }

        public decimal DailyLate(int daysLate)
        { return DailyLateFee * daysLate; }                         // Multiplies the daily late fee by the days late for a select item in the list.

        public int ItemID()
        {
            return ID;                                         // To find where the placement on the Items list from the CheckOutItem class is.
        }
    }
}
