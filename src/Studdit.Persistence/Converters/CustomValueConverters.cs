using System.Text.Json;

namespace Studdit.Persistence.Converters
{
    /// <summary>
    /// Custom value converters for complex types
    /// </summary>
    public static class CustomValueConverters
    {
        /// <summary>
        /// Converter for storing lists as JSON
        /// </summary>
        public static class ListToJsonConverter<T>
        {
            public static readonly Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<List<T>, string> Instance
                = new(
                    list => JsonSerializer.Serialize(list, (JsonSerializerOptions?)null),
                    json => JsonSerializer.Deserialize<List<T>>(json, (JsonSerializerOptions?)null) ?? new List<T>()
                );
        }

        /// <summary>
        /// Converter for storing dictionaries as JSON
        /// </summary>
        public static class DictionaryToJsonConverter<TKey, TValue> where TKey : notnull
        {
            public static readonly Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<Dictionary<TKey, TValue>, string> Instance
                = new(
                    dict => JsonSerializer.Serialize(dict, (JsonSerializerOptions?)null),
                    json => JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(json, (JsonSerializerOptions?)null) ?? new Dictionary<TKey, TValue>()
                );
        }
    }
}
