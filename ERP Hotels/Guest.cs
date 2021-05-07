using System;
using System.Text.Json.Serialization;

namespace ERP_Hotels
{
    internal class Guest
    {
        [JsonInclude]
        public int Id { get; }
        [JsonInclude]
        public FullName FullName { get; }
        [JsonInclude]
        public DateTime DateOfBirth { get; }
        [JsonInclude]
        public Address Address { get; }

        [JsonConstructor]
        public Guest(int id, FullName fullName, DateTime dateOfBirth, Address address)
        {
            Id = id;
            FullName = fullName;
            DateOfBirth = dateOfBirth;
            Address = address;
        }

        internal Guest(int id, string name, string surname, string patronymic, DateTime dateOfBirth, string city, string street, int house,
            int apartment) : this(id, new(name, surname, patronymic), dateOfBirth, new(city, street, house, apartment))
        {
        }
    }

    internal class FullName
    {
        public string Name { get; }
        public string Surname { get; }
        public string Patronymic { get; }

        public FullName(string name, string surname, string patronymic)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
        }
    }

    internal class Address
    {
        public string City { get; }
        public string Street { get; }
        public int House { get; }
        public int Apartment { get; }

        public Address(string city, string street, int house, int apartment)
        {
            City = city;
            Street = street;
            House = house;
            Apartment = apartment;
        }
    }
}
