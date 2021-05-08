using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Text.Json.JsonSerializer;

namespace ERP_Hotels
{
    internal static class FileEditor
    {
        private static readonly JsonSerializerOptions Options = new() {WriteIndented = true, IncludeFields = true};

        internal static void Open()
        {
            Program.Hotel = new(ReadFromFile<Dictionary <int, Guest>>("Guests").Result,
                ReadFromFile<Dictionary<int, Room>>("Rooms").Result,
                ReadFromFile<Dictionary<DateTime, Dictionary<int, Booking>>>("Bookings").Result);
        }

        private static async Task<T> ReadFromFile<T>(string name)
        {
            if (!File.Exists(name + ".json")) return default;

            await using var stream = File.OpenRead(name + ".json");
            T b = default;
            try
            {
                b = await DeserializeAsync<T>(stream, Options);
            }
            catch (Exception)
            {
                Console.WriteLine("Exception in " + name);
            }
            return b;
        }
        internal static void Close()
        {
            WriteToFile("Guests", Program.Hotel.Guests);
            WriteToFile("Rooms", Program.Hotel.Rooms);
            WriteToFile("Bookings", Program.Hotel.CalendarBooking);
        }
        
        private static async void WriteToFile<T>(string name, T list)
        {
            await using var stream = File.OpenWrite(name + ".json");
            await SerializeAsync(stream, list, Options);
        }
    }

}
