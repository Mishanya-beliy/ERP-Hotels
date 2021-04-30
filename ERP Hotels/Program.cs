using System;

namespace ERP_Hotels
{
    internal class Program
    {
        internal static Hotel Hotel;
        private static void Main()
        {
            Hotel = new();

            var idRoom = Hotel.AddRoom(Category.Standart);
            var idGuest = Hotel.AddGuest("Anton", "Sidorovich", "Andreevich", DateTime.Today, "Krivoy Rog",
                "Sobornosti", 23, 15);

            Booking booking = Hotel.Booking(idGuest, idRoom, new DateTime(2021, 4, 20), new DateTime(2021, 4, 30));

            Hotel.CheckIn(idGuest, idRoom, new DateTime(2021, 4, 21));
            booking.ChangeEndDate(new DateTime(2021, 5, 3));
            Hotel.CheckOut(idGuest, idRoom, new DateTime(2021, 4, 24));
        }
    }
}
