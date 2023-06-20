using Newtonsoft.Json;
using System.Globalization;

namespace hackaton.Models.Converters
{
    public class CustomDateTimeConverter : Newtonsoft.Json.JsonConverter
    {
            private readonly string _format;

            public CustomDateTimeConverter(string format)
            {
                _format = format;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(DateTime);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.Value == null)
                    return null;

                var dateString = reader.Value.ToString();

                // Modificação para tratar o formato "dd/MM/yyyy"
                if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    return date.Date;

                // Modificação para tratar o formato "dd/MM/yyyy HH:mm:ss"
                if (DateTime.TryParseExact(dateString, _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                    return dateTime;

                return null;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value is DateTime date)
                    writer.WriteValue(date.ToString(_format));
            }
        }

 }

