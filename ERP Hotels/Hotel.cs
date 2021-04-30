using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP_Hotels
{
    internal class Hotel
    {
        private int _roomsId = 0;
        private readonly Dictionary<int, Room> _rooms;

        private int _guestsId = 0;
        private readonly Dictionary<int, Guest> _guests;

        private readonly Dictionary<DateTime, List<Booking>> _calendarBooking;
        private readonly Dictionary<Booking, List<string>> _journal; 

        internal Hotel()
        {
            _rooms = new ();
            _guests = new ();
            _journal = new();
            _calendarBooking = new ();
        }

        internal int AddRoom(Category category)
        {
            if (_rooms.TryAdd(_roomsId, new Room(category)))
                return _roomsId++;

            return -1;
        }

        internal bool RemoveRoom(int idRoom)
            => _rooms.Remove(idRoom);

        internal int AddGuest(string name, string surname, string patronymic, DateTime dateOfBirth, string city,
            string street, int house, int apartment)
        {
            if (_guests.TryAdd(_guestsId, new Guest(name, surname, patronymic, dateOfBirth, city, street, house,
                apartment)))
                return _guestsId++;

            return -1;
        }

        internal bool RemoveGuest(int idGuest)
            => _guests.Remove(idGuest);

        internal delegate void DoBooking(List<Booking> list, Booking booking);
        internal Booking Booking(int idGuests, int idRoom, DateTime from, DateTime to)
        {
            if (!_rooms.TryGetValue(idRoom, out var room)) return null;
            if (!_guests.TryGetValue(idGuests, out var guest)) return null;
            var booking = new Booking(guest, room, from, to);


            for (var now = booking.From; now < booking.To; now += TimeSpan.FromDays(1))
                if (_calendarBooking.TryGetValue(now, out var bookings))
                    if (bookings.Any(b => b.Room == booking.Room))
                        return null;

            BookingDo(booking, booking.From, booking.To, AddBooking);
            return booking;
        }
        internal void AddBooking(List<Booking> list, Booking booking)
        {
            list.Add(booking);
        }
        internal bool CancelBooking(int idGuests, int idRoom, DateTime from, DateTime to)
        {
            if(!_rooms.TryGetValue(idRoom, out var room)) return false;
            if(!_guests.TryGetValue(idGuests, out var guest)) return false;
            if(!_calendarBooking.TryGetValue(from, out var bookings)) return false;

            var booking = bookings.FirstOrDefault(b => b.Guest == guest && b.Room == room && b.From <= from && b.To >= to);
            if (booking == null)
                return false;

            BookingDo(booking, from, to, RemoveBooking);
            return true;
        }
        internal void RemoveBooking(List<Booking> list, Booking booking)
        {
            list.Remove(booking);
        }

        internal bool BookingDo(Booking booking, DateTime from, DateTime to, DoBooking addOrRemove)
        {
            for (var now = from; now <= to; now += TimeSpan.FromDays(1))
            {
                if (!_calendarBooking.ContainsKey(now))
                    _calendarBooking.Add(now, new List<Booking>(1));

                addOrRemove(_calendarBooking[now], booking);
            }

            return true;
        }

        internal int CountFreeRoom(DateTime from, DateTime to)
        {
            var count = _rooms.Count;

            while (from <= to)
            {
                if (_calendarBooking.ContainsKey(from))
                    if (_calendarBooking[from].Count < count)
                        count = _calendarBooking[from].Count;

                from += TimeSpan.FromDays(1);
            }

            return count;
        }

        internal void CheckIn(int idGuest, int idRoom, DateTime from)
        {
            Booking nowBooking = null; 
            if (_calendarBooking.ContainsKey(from))
                nowBooking = _calendarBooking[@from]
                    .FirstOrDefault(b => b.Guest == _guests[idGuest] && b.Room == _rooms[idRoom]);


            nowBooking ??= new Booking(_guests[idGuest], _rooms[idRoom], @from, @from += TimeSpan.FromDays(1));

            if (!_journal.ContainsKey(nowBooking))
                _journal.Add(nowBooking, new List<string>());

            if (_journal[nowBooking].Contains("CheckIn"))
                return;

            if (nowBooking.From != from)
                nowBooking.ChangeStartDate(from);

            _journal[nowBooking].Add("CheckIn");
        }

        internal void CheckOut(int idGuest, int idRoom, DateTime date)
        {
            Booking nowBooking = null;
            if (_calendarBooking.ContainsKey(date))
                nowBooking = _calendarBooking[date]
                    .FirstOrDefault(b => b.Guest == _guests[idGuest] && b.Room == _rooms[idRoom]);

            if (nowBooking == null)
                return;

            if (!_journal.ContainsKey(nowBooking))
                return;

            if (_journal[nowBooking].Contains("CheckOut"))
                return;

            if (nowBooking.To != date)
                nowBooking.ChangeEndDate(date);

            _journal[nowBooking].Add("CheckOut");
        }
    }

    internal class Booking
    {
        internal DateTime From { get; private set; }
        internal DateTime To { get; private set; }
        internal Room Room { get; }
        internal Guest Guest { get; }

        internal Booking(Guest guest, Room room, DateTime from, DateTime to)
        {
            Guest = guest;
            Room = room;
            From = from;
            To = to;
        }

        internal void ChangeStartDate(DateTime newDate)
        {
            if (From > newDate)
                Program.Hotel.BookingDo(this, newDate, From.AddDays(-1), Program.Hotel.AddBooking);
            else
                Program.Hotel.BookingDo(this, From, newDate.AddDays(-1), Program.Hotel.RemoveBooking);
            From = newDate;

        }
        internal void ChangeEndDate(DateTime newDate)
        {
            if (To < newDate)
                Program.Hotel.BookingDo(this, To.AddDays(1), newDate, Program.Hotel.AddBooking);
            else
                Program.Hotel.BookingDo(this, newDate.AddDays(1), To, Program.Hotel.RemoveBooking);
            To = newDate;
        }
    }
}
