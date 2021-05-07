using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP_Hotels
{
    internal class Program
    {
        internal static Hotel Hotel;
        private static void Main()
        {
            FileEditor.Open();

            Console.WriteLine($" Count rooms: {Hotel.Rooms.Count}");
            Console.WriteLine($" Count guests: {Hotel.Guests.Count}");

            while(true)
            {
                Console.WriteLine($" {"Id",4} {"Name",10} {"Surname",13} {"Patronymic",15} " +
                                  $"{"Date birth",12} "                                      +
                                  $"{"City",10} {"Street",10} {"House",5} {"Apartment",5}");
                foreach (var hotelGuest in Hotel.Guests.Values)
                {
                    hotelGuest.WriteYourSelf();
                }

                Console.WriteLine($"\n {"Id",4} {"Category",10} {"Sleeping place",15} {"Coast",10}");
                SelectAction();
                FileEditor.Close();
                Console.Clear();
            }
        }

        private static void SelectAction()
        {
            switch (GetResponse(1,3, $" Choose action\n 1:Edit guest 2:Edit room 3:Edit booking"))
            {
                case 1:
                    EditGuest();
                    break;
                case 2:
                    EditRoom();
                    break;
                case 3:
                    EditBooking();
                    break;
            }
        }

        private static int _yorId = -1;
        private static void EditGuest()
        {
            switch (GetResponse(1,3, $" 1:Registration guest 2:Remove guest 3:Authorization(select)"))
            {
                case 1:
                    string name = GetResponse(" Name: ");
                    string surname = GetResponse(" Surname:");
                    string patronymic = GetResponse(" Patronymic:");
                    
                    var birth = GetDate(" Date of birth");

                    string city  = GetResponse(" City:");
                    string street = GetResponse(" Street:");
                    int house = GetResponse(0, 3000, " House:");
                    int apartment = GetResponse(0, 3000, " Apartment:");

                    _yorId = Hotel.AddGuest(name, surname, patronymic, birth, city, street, house, apartment);
                    Console.WriteLine($" Now your id is: {_yorId}");
                    
                    break;
                case 2:
                    int id = GetResponse(0, Int32.MaxValue, "Select id guest:");
                    Console.WriteLine($" Status operation is: {Hotel.RemoveGuest(id)}");
                    break;
                case 3:
                    break;
            }
        }

        private static  void Authorization()
        {
            int id = GetResponse(0, Int32.MaxValue, " Select id guest for authorization:");
            if (Hotel.Guests.ContainsKey(id))
            {
                _yorId = id;
                Console.WriteLine($" Now your id is: {_yorId}");
            }
            else
                Console.WriteLine(" This guest not exist");
        }
        private static void EditRoom()
        {
            switch (GetResponse(1, 2, $" 1:Add room 2:Remove room"))
            {
                case 1:
                    int categ = GetResponse(1, 4, " 1: Economy 2: Basic 3: Suite 4: PresidentSuite") - 1;
                    Console.WriteLine($"Room id is: {Hotel.AddRoom((RoomCategory)categ)}");
                    break;
                case 2:
                    int id = GetResponse(0, Int32.MaxValue, "Select id room:");
                    Console.WriteLine($"Status operation is: {Hotel.RemoveRoom(id)}");
                    break;
            }
        }

        private static void EditBooking()
        {
            switch (GetResponse(1, 6,
                $" 1:Check free room 2:Booking 3: Remove 4: Extend / shorten 5: Check in 6: Check out"))
            {
                case 1:
                    Console.WriteLine(" Select range");
                    foreach (var freeRoom in Hotel.FreeRooms(GetDate(" From:"), GetDate(" To:")).Keys)
                        Console.Write(" " + freeRoom);
                    Console.WriteLine(": id free rooms");
                    break;
                case 2:
                    CheckAuth();
                    Console.WriteLine(Hotel.Booking(_yorId, GetResponse(0, int.MaxValue, " Select room by id:"),
                                          GetDate(" From:"),
                                          GetDate(" To:")) ==
                                      default
                        ? " Something went wrong"
                        : " Congratulations, your room is booked for you");
                    break;
                case 3:
                    CheckAuth();
                    Console.WriteLine(Hotel.CancelBooking(_yorId, GetResponse(0, int.MaxValue, " Select room by id:"), GetDate(" From:"),
                        GetDate(" To:")) == false ? " Something went wrong" : " Successfully canceled reservation");
                    break;
                case 4:
                    CheckAuth();
                    Hotel.TryGetBooking(_yorId, GetResponse(0, int.MaxValue, " Select room by id:"),
                        GetDate(" Write any date old booking:"), out var booking);
                    if (booking != default)
                    {
                        Console.WriteLine(" Successfully found");
                        booking.From = GetDate(" Write new date from:");
                        booking.To = GetDate(" new date to:");
                    }
                    else Console.WriteLine(  " Booking not found");
                    break;
                case 5:
                    CheckAuth();
                    Hotel.CheckIn(_yorId, GetResponse(0, int.MaxValue, " Select room by id:"), DateTime.Now);
                    break;
                case 6:
                    CheckAuth();
                    Hotel.CheckOut(_yorId, GetResponse(0, int.MaxValue, " Select room by id:"), DateTime.Now);
                    break;
            }
        }

        private static void CheckAuth()
        {
            while (_yorId == -1)
            {
                Console.WriteLine(" Before this action need authorization");
                Authorization();
            }
        }
        private static DateTime GetDate(string request)
        {
            Console.WriteLine(request);
            var year = GetResponse(0, 3000, " Year:");
            var month = GetResponse(1, 12, " Month:");
            var day = GetResponse(1, 31, " Day:");
            return  new (year, month, day);

        }
        private static string GetResponse(string request)
        {
            Console.WriteLine(request);
            return Console.ReadLine();
        }
        static List<int> GetParametrs(string[] requests)
        {
            List<int> response = new(requests.Length);
            response.AddRange(requests.Select(request => GetResponse(int.MinValue, int.MaxValue, request)));
            return response;
        }

        private const string Stop = "Stop";
        static  int GetResponse(int min, int max, string request)
        {
            int result;
            string s;

            Console.WriteLine(request);
            while (!int.TryParse(s = Console.ReadLine(), out result) || result < min || result > max)
                if (s == Stop)
                    throw new NotImplementedException();
                else
                    Console.WriteLine($"Write correct number {min} - {max}!");
            return result;
        }
    }
}
