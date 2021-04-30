using System;

namespace ERP_Hotels
{
    internal class Guest
    {
        internal FullName FullName { get; }
        internal DateTime DateOfBirth { get; }
        internal Address Address { get; }

        internal Guest(string name, string surname, string patronymic, DateTime dateOfBirth, string city, string street, int house,
            int apartment)
        {
            FullName = new FullName(name, surname, patronymic);
            DateOfBirth = dateOfBirth;
            Address = new Address(city, street, house, apartment);
        }
    }

    internal class FullName
    {
        internal string Name { get; }
        internal string Surname { get; }
        internal string Patronymic { get; }

        internal FullName(string name, string surname, string patronymic)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
        }
    }

    internal class Address
    {
        internal string City { get; }
        internal string Street { get; }
        internal int House { get; }
        internal int Apartment { get; }

        internal Address(string city, string street, int house, int apartment)
        {
            this.City = city;
            this.Street = street;
            this.House = house;
            this.Apartment = apartment;
        }
    }
}
