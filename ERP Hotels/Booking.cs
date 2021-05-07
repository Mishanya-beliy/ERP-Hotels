using System;
using System.Text.Json.Serialization;

namespace ERP_Hotels
{
    internal class Booking
    {
        [JsonInclude]
        public Room Room { get; }
        [JsonInclude]
        public Guest Guest { get; }

        private DateTime _from;
        private DateTime _to;
        private DateTime _checkIn;
        private DateTime _checkOut;


        [JsonInclude]
        public DateTime From
        {
            get => _from;
            set
            {
                if (CheckIn != DateTime.MinValue) return;

                if (Program.Hotel != default)
                    if (From != value)
                        if (From > value)
                            Program.Hotel.AddBookingToCalendar(this, value, From.AddDays(-1));
                        else
                            Program.Hotel.RemoveBookingFromCalendar(this, From, value.AddDays(-1));

                _from = value;
            }
        }

        [JsonInclude]
        public DateTime To
        {
            get => _to;
            set
            {
                if (CheckOut != DateTime.MinValue) return;

                if (Program.Hotel != default)
                    if (To != value)
                        if (To < value)
                            Program.Hotel.AddBookingToCalendar(this, To.AddDays(1), value);
                        else
                            Program.Hotel.RemoveBookingFromCalendar(this, value.AddDays(1), To);
                _to = value;
            }
        }

        [JsonInclude]
        public DateTime CheckIn
        {
            get => _checkIn;
            set
            {
                From = value;
                if (From == value)
                    _checkIn = value;
            }
        }

        [JsonInclude]
        public DateTime CheckOut
        {
            get => _checkOut;
            set
            {
                To = value;
                if (To == value)
                    _checkOut = value;
            }
        }

        internal Booking(Guest guest, Room room, DateTime from, DateTime to)
        {
            Guest = guest;
            Room = room;
            _from = from;
            _to = to;
        }

        [JsonConstructor]
        public Booking(Guest guest, Room room, DateTime from, DateTime to, DateTime checkIn, DateTime checkOut) :this(guest, room, from, to)
        {
            _checkIn = checkIn;
            _checkOut = checkOut;
        }
    }
}
