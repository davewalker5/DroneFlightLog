using DroneFlightLog.Data.Entities;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DroneFlightLog.Data.Binders
{
    public class MaintenanceRecordTypeJsonConverter : JsonConverter<MaintenanceRecordType>
    {
        public override MaintenanceRecordType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();

            if (Enum.TryParse(typeToConvert, value, true, out object result))
            {
                return (MaintenanceRecordType)result;
            }

            throw new JsonException($"Unable to convert \"{value}\" to enum {typeToConvert}.");
        }

        public override void Write(Utf8JsonWriter writer, MaintenanceRecordType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
