using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP_Hotels
{
    internal class Hotel
    {
        private int _roomsId = 0;
        private Dictionary<int, Room> _rooms;

        private int _guestsId = 0;
        private Dictionary<int, Guest> _guests;

        private Dictionary<DateTime, List<Booking>> _calendarBooking;//Dictionary<Room, List<Guest>>> _calendarBooking;

        internal Hotel()
        {
            _rooms = new ();
            _guests = new ();

            _calendarBooking = new();
        }

        internal int AddRoom(Category category)
        {
            if (_rooms.TryAdd(_roomsId, new Room(category)))
                return _roomsId++;

            return -1;
        }

        internal bool RemoveRoom(int idRoom)
            => _rooms.Remove(idRoom);

        internal int AddGuest(string name)
        {
            if (_guests.TryAdd(_guestsId, new Guest(name)))
                return _guestsId++;

            return -1;
        }

        internal bool RemoveGuest(int idGuest)
            => _guests.Remove(idGuest);

        internal bool Booking(int idGuests, int idRoom, DateTime from, DateTime to)
        {
            /*(from b in _calendarBooking
                where b.Key >= @from && b.Key <= to
                select b.Value)
            */ //Do check for duplicate booking

            var room = _rooms[idRoom];
            var guest = _guests[idGuests];
            var newBooking = new Booking(guest, room, from, to);

            while (from <= to)
            {
                if (!_calendarBooking.ContainsKey(from))
                    _calendarBooking.Add(from, new List<Booking>(1));

                _calendarBooking[from].Add(newBooking);

                from += TimeSpan.FromDays(1);
            }

            return true;
        }
        private bool Booking(Booking booking) // Do noraml booking
        {
            for (var now = booking._from; now < booking._to; now += TimeSpan.FromDays(1))
            {
                if (!_calendarBooking.ContainsKey(booking._from))
                    _calendarBooking.Add(booking._from, new List<Booking>(1));

                _calendarBooking[booking._from].Add(booking);
            }

            return true;
        }
        /*
        internal bool Booking(List<int> idGuests, int idRoom, DateTime @from, DateTime to)
        {
            var room = _rooms[idRoom];

            if (idGuests.Count > room._sleepingPlaces)
                return false;

            var  guest = new List<Guest>();
            guest.AddRange(idGuests.Select(idGuest => _guests[idGuest]));


            while (@from <= to)
            {
                if(!_calendarBooking.ContainsKey(@from))
                    _calendarBooking.Add(@from, new Dictionary<Room, List<Guest>>());


                if (!_calendarBooking[@from].ContainsKey(room))
                    _calendarBooking[@from].Add(room, new List<Guest>());
                else
                if(_calendarBooking[@from][room].Count + guest.Count > room._sleepingPlaces)
                    return false;

                _calendarBooking[@from][room].AddRange(guest);

                @from += TimeSpan.FromDays(1);
            }

            return true;
        }
        */ //Booking
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

        internal void CheckIn(int idGuest, int idRoom, DateTime from, DateTime to)
        {
            var nowBooking = new Booking(_guests[idGuest], _rooms[idRoom], @from, to);
            var k =
                from booking in _calendarBooking[@from]
                where booking.Equals(nowBooking)
                select booking;

            if (k.Any())
                nowBooking = k.First();
            else
                Booking(nowBooking);

            nowBooking.CheckIn();
        }
    }

    internal class Booking
    {
        internal DateTime _from { get; }
        internal DateTime _to { get; }
    internal Room _room { get; }
        internal Guest _guest { get; }
        private bool guestIsCheckIn;

        internal Booking(Guest guest, Room room, DateTime from, DateTime to)
        {
            _guest = guest;
            _room = room;
            _from = from;
            _to = to;
        }

        internal void CheckIn()
        {
            if (!guestIsCheckIn) guestIsCheckIn = true;
        }
    }
}
