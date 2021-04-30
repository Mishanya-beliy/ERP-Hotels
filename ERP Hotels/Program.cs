using System;
using System.Collections.Generic;

namespace ERP_Hotels
{
    internal class Program
    {
        private static void Main()
        {
            Hotel  hotel = new();

            var idRoom = hotel.AddRoom(Category.Standart);
            var idGuest = hotel.AddGuest("Anton");

            hotel.Booking(new List<int>{ idGuest }, idRoom, new DateTime(2021, 4, 28), new DateTime(2021, 4, 30));

            Console.WriteLine(new DateTime(2021, 4, 30) - new DateTime(2021, 4, 28));
        }
    }
}
