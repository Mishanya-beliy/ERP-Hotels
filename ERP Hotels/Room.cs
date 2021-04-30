﻿using System;

namespace ERP_Hotels
{
    internal class Room
    {
        private double _coast;
        internal byte _sleepingPlaces { get; }
        private Category _category;

        //private List<DateTime> startBooking, endBooking;

        public Room(Category category)
        {
            _category = category;
            switch (_category)
            {
                case Category.Economy:
                    _coast = 50;
                    _sleepingPlaces = 4;
                    break;
                case Category.Standart:
                    _coast = 100;
                    _sleepingPlaces = 3;
                    break;
                case Category.Suite:
                    _coast = 200;
                    _sleepingPlaces = 2;
                    break;
                case Category.PresidentSuite:
                    _coast = 300;
                    _sleepingPlaces = 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal enum Category
    {
        Economy,
        Standart,
        Suite,
        PresidentSuite
    }
}
