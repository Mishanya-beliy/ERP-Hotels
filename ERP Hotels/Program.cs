using System;
using System.Threading.Tasks;

namespace ERP_Hotels
{
    internal class Program
    {
        internal static Hotel Hotel;
        private static void Main()
        {
            FileEditor.Open();
            //Hotel = new Hotel();
            var idRoom = Hotel.AddRoom(RoomCategory.Economy);
            var idGuest = Hotel.AddGuest("Anton", "Sidorovich", "Andreevich", DateTime.Today, "Krivoy Rog",
                "Sobornosti", 23, 15);
               Hotel.AddGuest("Anton", "Sidorovich", "Andreevich", DateTime.Today, "Krivoy Rog",
                "Sobornosti", 23, 15);
               Hotel.AddGuest("Anton", "Sidorovich", "Andreevich", DateTime.Today, "Krivoy Rog",
                "Sobornosti", 23, 15);
               Hotel.AddGuest("Anton", "Sidorovich", "Andreevich", DateTime.Today, "Krivoy Rog",
                "Sobornosti", 23, 15);

            var booking = Hotel.Booking(idGuest, idRoom, new(2022, 4, 20), new(2022, 4, 30));

            //Hotel.CheckIn(idGuest, idRoom, new(2021, 4, 21));
            //booking.ChangeEndDate(new DateTime(2021, 5, 3));
            //Hotel.CheckOut(idGuest, idRoom, new DateTime(2021, 4, 24));

            Console.ReadKey();
            FileEditor.Close();
        }
    }
}
