using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ERP_Hotels
{
    internal class Program
    {
        private static readonly CancellationTokenSource Source = new();
        private static readonly CancellationToken Token = Source.Token;

        private static readonly string Rooms = $" Rooms\n {"Id",4} {"Category",15} {"Sleeping place",15} {"Coast",10}";
        private static readonly string Guest = $" Guests\n {"Id",4} {"Name",10} {"Surname",13} {"Patronymic",15} " +
                                                     $"{"Date birth",12} "                                      +
                                                     $"{"City",10} {"Street",10} {"House",5} {"Apartment",5}";

        internal static Hotel Hotel;

        private static void Main()
        {
            FileEditor.Open();

            string response = default;
            while (true)
            {
                ColoredWrite(_yorId == -1
                    ? " You unauthorized"
                    : $" Your id: {_yorId}", ConsoleColor.DarkYellow);
                DisplayOf(Guest, Hotel.Guests.Values);
                DisplayOf(Rooms, Hotel.Rooms.Values);

                ColoredWrite($" {response}", ConsoleColor.DarkRed);

                try
                {
                    response = SelectAction();
                    FileEditor.Write(Token);
                    Console.Clear();
                }
                catch
                {
                    Source.Dispose();
                    break;
                }
            }
        }

        private static void ColoredWrite(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static void DisplayOf<T>(string ho, Dictionary<int, T>.ValueCollection hotelItems) where T: IDisplayed
        {
            Console.WriteLine(ho);
            foreach (var hotelItem in hotelItems)
                hotelItem.WriteYourSelf();
            Console.WriteLine($" Count: {hotelItems.Count}\n");
        }

        private static string SelectAction()
        {
            switch (GetResponse(1,3, $" Choose action\n 1:Edit guest 2:Edit room 3:Edit booking"))
            {
                case 1:
                    return EditGuest();
                case 2:
                    return EditRoom();
                case 3:
                    return EditBooking();
            }

            return default;
        }

        private static int _yorId = -1;

        private static string EditGuest()
        {
            switch (GetResponse(1, 3, $" 1:Registration guest 2:Remove guest 3:Authorization(select)"))
            {
                case 1:
                    string name = GetResponse(" Name: ");
                    string surname = GetResponse(" Surname:");
                    string patronymic = GetResponse(" Patronymic:");

                    var birth = GetDate(" Date of birth");

                    string city = GetResponse(" City:");
                    string street = GetResponse(" Street:");
                    int house = GetResponse(0, 3000, " House:");
                    int apartment = GetResponse(0, 3000, " Apartment:");

                    _yorId = Hotel.AddGuest(name, surname, patronymic, birth, city, street, house, apartment);
                    return $" Now your id is: {_yorId}";
                case 2:
                    int id = GetResponse(0, Int32.MaxValue, "Select id guest:");
                    return $" Status operation is: {Hotel.RemoveGuest(id)}";
                case 3:
                    return Authorization()
                        ? $" Now your id is: {_yorId}"
                        : " This guest not exist";
            }

            return default;
        }

        private static  bool Authorization()
        {
            int id = GetResponse(0, Hotel.Guests.Keys.Max(), " Select id guest for authorization:");
            if (Hotel.Guests.ContainsKey(id))
            {
                _yorId = id;
                Console.WriteLine($" Now your id is: {_yorId}");
                return true;
            }
            else
                return false;
        }
        private static string EditRoom()
        {
            switch (GetResponse(1, 2, $" 1:Add room 2:Remove room"))
            {
                case 1:
                    int categ = GetResponse(1, 4, " 1: Economy 2: Basic 3: Suite 4: PresidentSuite") - 1;
                    return $"Room id is: {Hotel.AddRoom((RoomCategory)categ)}";
                case 2:
                    int id = GetResponse(0, Int32.MaxValue, "Select id room:");
                    return  $"Status operation is: {Hotel.RemoveRoom(id)}";
            }

            return default;
        }

        private static string EditBooking()
        {
            string response = default;
            switch (GetResponse(1, 6,
                $" 1:Check free room 2:Booking 3: Remove 4: Extend / shorten 5: Check in 6: Check out"))
            {
                case 1:
                    Console.WriteLine(" Select range");
                    foreach (var freeRoom in Hotel.FreeRooms(GetDate(" From:"), GetDate(" To:")).Keys)
                        response += " " + freeRoom;
                    return "Free rooms id:" + response;
                case 2:
                    CheckAuth();
                    return Hotel.Booking(_yorId, GetResponse(0, int.MaxValue, " Select room by id:"),
                               GetDate(" From:"),
                               GetDate(" To:")) ==
                           default
                        ? " Something went wrong"
                        : " Congratulations, your room is booked for you";
                case 3:
                    CheckAuth();
                    return Hotel.CancelBooking(_yorId, GetResponse(0, int.MaxValue, " Select room by id:"),
                               GetDate(" From:"),
                               GetDate(" To:"))
                        ? " Successfully canceled reservation"
                        : " Something went wrong";
                case 4:
                    CheckAuth();
                    Hotel.TryGetBooking(_yorId, GetResponse(0, int.MaxValue, " Select room by id:"),
                        GetDate(" Write any date old booking:"), out var booking);
                    if (booking != default)
                    {
                        Console.WriteLine(" Successfully found");
                        booking.From = GetDate(" Write new date from:");
                        booking.To = GetDate(" new date to:");
                        return " Successfully change range";
                    }
                    else return " Booking not found";
                case 5:
                    CheckAuth();
                    return Hotel.CheckIn(_yorId,
                        GetResponse(0, int.MaxValue, " Select room by id:"), DateTime.Now)
                        ? " Successfully check in"
                        : " Something went wrong";
                case 6:
                    CheckAuth();
                    return Hotel.CheckOut(_yorId, 
                        GetResponse(0, int.MaxValue, " Select room by id:"), DateTime.Now)
                        ? " Successfully check in"
                        : " Something went wrong";
            }
            return default;
        }

        private static void CheckAuth()
        {
            while (_yorId == -1)
            {
                Console.WriteLine(" Before this action need authorization");
                Console.WriteLine(Authorization() 
                    ? $" Now your id is: {_yorId}"
                    : " This guest not exist");
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

        private const string Stop = "Stop";

        private static  int GetResponse(int min, int max, string request)
        {
            int result;
            string s;

            Console.WriteLine(request + " for exit write 'Stop'");
            while (!int.TryParse(s = Console.ReadLine(), out result) || result < min || result > max)
                if (s == Stop)
                {
                    Source.Cancel();
                    throw new NotImplementedException();
                }
                else
                    Console.WriteLine($"Write correct number {min} - {max}!");
            return result;
        }
    }

    internal interface IDisplayed
    {
        internal void WriteYourSelf();
    }
}
