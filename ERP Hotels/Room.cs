using System;
using System.Text.Json.Serialization;

namespace ERP_Hotels
{
    public class Room : IDisplayed
    {
        [JsonInclude]
        public int Id { get; }
        internal float Coast { get; }
        [JsonInclude]
        public RoomCategory Category { get; }
        internal byte SleepingPlaces { get; }

        public Room(int id, RoomCategory category)
        {
            Id = id;
            Category = category;
            switch (Category)
            {
                case RoomCategory.Economy:
                    Coast = 50;
                    SleepingPlaces = 4;
                    break;
                case RoomCategory.Basic:
                    Coast = 100;
                    SleepingPlaces = 3;
                    break;
                case RoomCategory.Suite:
                    Coast = 200;
                    SleepingPlaces = 2;
                    break;
                case RoomCategory.PresidentSuite:
                    Coast = 300;
                    SleepingPlaces = 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void IDisplayed.WriteYourSelf()
        {
            Console.WriteLine($" {Id,4} {Category, 15} {SleepingPlaces, 15} {Coast, 10}");
        }
    }

    public enum RoomCategory
    {
        Economy,
        Basic,
        Suite,
        PresidentSuite
    }
}
