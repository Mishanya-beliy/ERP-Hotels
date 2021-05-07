using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Text.Json.JsonSerializer;

namespace ERP_Hotels
{
    internal static class FileEditor
    {
        private static readonly JsonSerializerOptions Options = new() {WriteIndented = true, IncludeFields = true};

        internal static void Open()
        {
            List<Guest> guest = default;
            List<Room> room = default;
            Dictionary<DateTime, Dictionary<int, Booking>> calendar = default;

            try
            {
                guest = ReadFromFile<List<Guest>>("Guests").Result;
            }
            finally
            {
                try
                {
                    room = ReadFromFile<List<Room>>("Rooms").Result;
                }
                finally
                {
                    try
                    {
                        calendar = ReadFromFile<Dictionary<DateTime, Dictionary<int, Booking>>>("Bookings").Result;
                    }
                    finally
                    {
                        Program.Hotel = new( guest, room, calendar);
                    }
                }
            }
        }

        private static async Task<T> ReadFromFile<T>(string name)
        {
            if (!File.Exists(name + ".json")) return default;

            await using var stream = File.OpenRead(name + ".json");
            return await DeserializeAsync<T>(stream, Options);
        }
        internal static void Close()
        {
            WriteToFile("Guests", Program.Hotel.Guests);
            WriteToFile("Rooms", Program.Hotel.Rooms);
            WriteToFile("Bookings", Program.Hotel.CalendarBooking);
        }
        
        private static async void WriteToFile<T>(string name, T list)
        {
            await using var stream = File.OpenWrite(name + ".json");
            await SerializeAsync(stream, list, Options);
        }
    }
    //internal class JsonEnumerableT<T> : ICollection<T>, IEnumerable<T>, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
    //{
    //    List<T> _list;

    //    public JsonEnumerableT(IList sourceList)
    //    {
    //        // TODO: Change sourceList from IList to List<T> so we can do a direct assignment here.
    //        _list = new List<T>();

    //        foreach (object item in sourceList)
    //        {
    //            _list.Add((T)item);
    //        }
    //    }

    //    public T this[int index] { get => (T)_list[index]; set => _list[index] = value; }

    //    public int Count => _list.Count;

    //    public bool IsReadOnly => false;

    //    public void Add(T item)
    //    {
    //        _list.Add(item);
    //    }

    //    public void Clear()
    //    {
    //        _list.Clear();
    //    }

    //    public bool Contains(T item)
    //    {
    //        return _list.Contains(item);
    //    }

    //    public void CopyTo(T[] array, int arrayIndex)
    //    {
    //        _list.CopyTo(array, arrayIndex);
    //    }

    //    public IEnumerator<T> GetEnumerator()
    //    {
    //        return _list.GetEnumerator();
    //    }

    //    public int IndexOf(T item)
    //    {
    //        return _list.IndexOf(item);
    //    }

    //    public void Insert(int index, T item)
    //    {
    //        _list.Insert(index, item);
    //    }

    //    public bool Remove(T item)
    //    {
    //        return _list.Remove(item);
    //    }

    //    public void RemoveAt(int index)
    //    {
    //        _list.RemoveAt(index);
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }
    //}

    //internal sealed class DefaultEnumerableConverter : JsonEnumerableConverter
    //{
    //    public override IEnumerable CreateFromList(ref ReadStack state, IList sourceList, JsonSerializerOptions options)
    //    {
    //        Type elementType = state.Current.GetElementType();

    //        Type t = typeof(JsonEnumerableT<>).MakeGenericType(elementType);
    //        return (IEnumerable)Activator.CreateInstance(t, sourceList);
    //    }
    //}
    //internal class Converter : JsonConverter<IEnumerable>
    //{
    //    public override void Write(Utf8JsonWriter writer, IEnumerable value, JsonSerializerOptions options)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override IEnumerable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    //    {
    //        var list = new List<Type>();
    //        list.Add(reader.get);

    //        throw new NotImplementedException();
    //    }
    //}
    //public class IEnumerableConverter : JsonConverterFactory
    //{
    //    public override bool CanConvert(Type typeToConvert)
    //    {
    //        if (!typeToConvert.IsGenericType)
    //        {
    //            return false;
    //        }

    //        if (typeToConvert.GetGenericTypeDefinition() != typeof(Dictionary<,>))
    //        {
    //            return false;
    //        }

    //        return typeToConvert.GetGenericArguments()[0].IsEnum;
    //    }

    //    public override JsonConverter CreateConverter(
    //        Type type,
    //        JsonSerializerOptions options)
    //    {
    //        Type ienumerableType = type.GetGenericArguments()[0];
    //        Type keyType = type.GetGenericArguments()[0];
    //        Type valueType = type.GetGenericArguments()[1];

    //        JsonConverter converter = (JsonConverter)Activator.CreateInstance(
    //            typeof(IEnumerableConverter<>).MakeGenericType(
    //                new Type[] { ienumerableType }),
    //            BindingFlags.Instance | BindingFlags.Public,
    //            binder: null,
    //            args: new object[] { options },
    //            culture: null);

    //        return converter;
    //    }

    //    private class IEnumerableConverter<T> :
    //        JsonConverter<IEnumerable<T>> where T : Guest, Room
    //    {
    //        private readonly JsonConverter<TValue> _valueConverter;
    //        private readonly Type _keyType;
    //        private readonly Type _valueType;

    //        public DictionaryEnumConverterInner(JsonSerializerOptions options)
    //        {
    //            // For performance, use the existing converter if available.
    //            _valueConverter = (JsonConverter<TValue>)options
    //                .GetConverter(typeof(TValue));

    //            // Cache the key and value types.
    //            _keyType = typeof(TKey);
    //            _valueType = typeof(TValue);
    //        }

    //        public override Dictionary<TKey, TValue> Read(
    //            ref Utf8JsonReader reader,
    //            Type typeToConvert,
    //            JsonSerializerOptions options)
    //        {
    //            if (reader.TokenType != JsonTokenType.StartObject)
    //            {
    //                throw new JsonException();
    //            }

    //            var dictionary = new Dictionary<TKey, TValue>();

    //            while (reader.Read())
    //            {
    //                if (reader.TokenType == JsonTokenType.EndObject)
    //                {
    //                    return dictionary;
    //                }

    //                // Get the key.
    //                if (reader.TokenType != JsonTokenType.PropertyName)
    //                {
    //                    throw new JsonException();
    //                }

    //                string propertyName = reader.GetString();

    //                // For performance, parse with ignoreCase:false first.
    //                if (!Enum.TryParse(propertyName, ignoreCase: false, out TKey key) &&
    //                    !Enum.TryParse(propertyName, ignoreCase: true, out key))
    //                {
    //                    throw new JsonException(
    //                        $"Unable to convert \"{propertyName}\" to Enum \"{_keyType}\".");
    //                }

    //                // Get the value.
    //                TValue value;
    //                if (_valueConverter != null)
    //                {
    //                    reader.Read();
    //                    value = _valueConverter.Read(ref reader, _valueType, options);
    //                }
    //                else
    //                {
    //                    value = JsonSerializer.Deserialize<TValue>(ref reader, options);
    //                }

    //                // Add to dictionary.
    //                dictionary.Add(key, value);
    //            }

    //            throw new JsonException();
    //        }

    //        public override void Write(
    //            Utf8JsonWriter writer,
    //            Dictionary<TKey, TValue> dictionary,
    //            JsonSerializerOptions options)
    //        {
    //            writer.WriteStartObject();

    //            foreach ((TKey key, TValue value) in dictionary)
    //            {
    //                var propertyName = key.ToString();
    //                writer.WritePropertyName
    //                    (options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName);

    //                if (_valueConverter != null)
    //                {
    //                    _valueConverter.Write(writer, value, options);
    //                }
    //                else
    //                {
    //                    JsonSerializer.Serialize(writer, value, options);
    //                }
    //            }

    //            writer.WriteEndObject();
    //        }
    //    }
    //}
}
