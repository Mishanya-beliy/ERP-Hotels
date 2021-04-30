using System;
using System.Collections.Generic;

namespace ERP_Hotels
{
    internal class Guest
    {
        internal string _name { get; }
        private string surname;
        private string patronymic;

        private readonly DateTime dateOfBirth;

        private List<DateTime> checkIn, checkOut;

        internal Guest(string name)
        {
            _name = name;
        }

    }
}
