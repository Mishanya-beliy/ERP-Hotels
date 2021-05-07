using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP_Hotels
{
    internal class Hotel
    {
        private int _roomsId = 0;
        internal Dictionary<int, Room> Rooms { get; }
        private int _guestId = 0;
        internal Dictionary<int, Guest> Guests { get; }

        internal Dictionary<DateTime, Dictionary<int, Booking>> CalendarBooking { get; }

        internal Hotel()
        {
            Guests = new();
            Rooms = new();
            CalendarBooking = new();
        }

        internal Hotel(IEnumerable<Guest> guests, IEnumerable<Room> rooms,
            Dictionary<DateTime, Dictionary<int, Booking>>  calendarBooking) : this()
        {
            if (guests is { })
                foreach (var guest in guests)
                    AddGuest(guest);

            if (rooms is { })
                foreach (var room in rooms)
                    AddRoom(room);

            if (calendarBooking is { }) CalendarBooking = calendarBooking;
        }

        internal int AddRoom(RoomCategory roomCategory)
        {
            var room = new Room(_roomsId, roomCategory);
            return AddRoom(room);
        }

        private int AddRoom(Room room)
        {
            if (Rooms.TryAdd(_roomsId, room)) return _roomsId++;
            return -1;
        }

        internal bool RemoveRoom(int id)
            => Rooms.Remove(id);

        internal int AddGuest(string name, string surname, string patronymic, DateTime dateOfBirth, string city,
            string street, int house, int apartment)
        {
            var guest = new Guest(_guestId, name, surname, patronymic, dateOfBirth, city, street, house, apartment);
            return AddGuest(guest);
        }

        private int AddGuest(Guest guest)
        {
            if (Guests.TryAdd(_guestId, guest)) return _guestId++;
            return -1;
        }

        internal bool RemoveGuest(int id)
            => Guests.Remove(id);


        internal Booking Booking(int idGuests, int idRoom, DateTime from, DateTime to)
        {
            if (!TryGetGuestAndRoom(idGuests, idRoom, out var guest, out var room)) return default;
            if (!CorrectDate(from, to)) return default;

            var booking = new Booking(guest, room, from, to);
            return AddBookingToCalendar(booking, from, to) ? booking : default;
        }

        internal bool AddBookingToCalendar(Booking booking, DateTime from, DateTime to)
        {
            if (booking == default     ||
                !CorrectDate(from, to) ||
                booking.From > from    ||
                booking.To   < to) return false;

            var bookings = SelectDate(from, to, true);

            if (bookings.Any(b => b.ContainsKey(booking.Room.Id)))
                return false;

            foreach (var b in bookings)
                b.Add(booking.Room.Id, booking);
            return true;
        }

        internal bool CancelBooking(int idGuests, int idRoom, DateTime from, DateTime to)
        {
            if (!CorrectDate(from)                                      ||
                !TryGetBooking(idGuests, idRoom, from, out var booking) ||
                booking.From != from                                    ||
                booking.To   != to) return false;

            RemoveBookingFromCalendar(booking, from, to);

            return true;
        }

        internal void RemoveBookingFromCalendar(Booking booking, DateTime from, DateTime to)
        {
            if (booking == default || !CorrectDate(from, to)) return;

            foreach (var bookings in SelectDate(from, to, false))
                bookings.Remove(booking.Room.Id);
        }


        internal Dictionary<int, Room> FreeRooms(DateTime from, DateTime to)
        {
            if (!CorrectDate(from, to)) return default;

            var rooms = new Dictionary<int, Room>(Rooms);

            foreach (var booking 
                in SelectDate(from, to, false).
                    SelectMany(bookings => bookings).GroupBy(r => r.Key))

                rooms.Remove(booking.Key);

            return rooms;
        }


        internal void CheckIn(int idGuests, int idRoom, DateTime date)
        {
            if (!TryGetBooking(idGuests, idRoom, date, out var booking))
                if (!Guests.ContainsKey(idGuests) ||
                    !Rooms.ContainsKey(idRoom) ||
                    !CorrectDate(date))
                    return;
                else
                    booking = Booking(idGuests, idRoom, date, date.AddDays(1));

            if (booking == default) return;
            booking.CheckIn = date;                                     
        }

        internal void CheckOut(int idGuests, int idRoom, DateTime date)
        {
            if (!TryGetBooking(idGuests, idRoom, date, out var booking))
                return;

            booking.CheckOut = date;
        }
        private static bool CorrectDate(DateTime from, DateTime to) =>
            CorrectDate(from) && from <= to;

        private static bool CorrectDate(DateTime date) => date >= DateTime.Now;


        private List<Dictionary<int, Booking>> SelectDate(DateTime from, DateTime to, bool willNeedAdd)
        {
            var selected = new List<Dictionary<int, Booking>>(to.Subtract(from).Days);

            for (var now = from; now <= to; now = now.AddDays(1))
            {
                if (!CalendarBooking.ContainsKey(now))
                    if (!willNeedAdd)
                        continue;
                    else
                        CalendarBooking.Add(now, new(1));

                selected.Add(CalendarBooking[now]);
            }

            return selected;
        }
        private bool TryGetBooking(int idGuests, int idRoom, DateTime date, out Booking booking)
        {
            booking = default;
            return Guests.TryGetValue(idGuests, out var guest)
                   && Rooms.TryGetValue(idRoom, out var room)
                   && CalendarBooking.TryGetValue(date, out var bookings)
                   && bookings.TryGetValue(room.Id, out booking)
                   && booking.Guest == guest;
        }

        private bool TryGetGuestAndRoom(int idGuests, int idRoom, out Guest guest, out Room room)
        {
            room = default;
            return Guests.TryGetValue(idGuests, out guest) &&
                   Rooms.TryGetValue(idRoom, out room);
        }
    }
}
