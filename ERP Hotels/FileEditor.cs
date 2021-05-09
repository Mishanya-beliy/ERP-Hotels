using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
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
        internal static void Write(CancellationToken token)
        {
            WriteToFile("Guests", Program.Hotel.Guests, token);
            WriteToFile("Rooms", Program.Hotel.Rooms, token);
            WriteToFile("Bookings", Program.Hotel.CalendarBooking, token);
        }
        
        private static async void WriteToFile<T>(string name, T list, CancellationToken token)
        {
            await using var stream = File.OpenWrite(name + ".json");
            await SerializeAsync(stream, list, Options, token);
        }
    }

}
