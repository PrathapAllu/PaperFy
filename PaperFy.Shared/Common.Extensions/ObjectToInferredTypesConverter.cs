using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaperFy.Shared.Common.Extensions
{

	internal class ObjectToInferredTypesConverter : JsonConverter<object>
	{
		public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.True:
					return true;
				case JsonTokenType.False:
					return false;
				case JsonTokenType.Number:
					{
						if (reader.TryGetInt64(out var value2))
						{
							return value2;
						}
						return reader.GetDouble();
					}
				case JsonTokenType.String:
					{
						if (reader.TryGetDateTime(out var value))
						{
							return value;
						}
						return reader.GetString();
					}
				default:
					return JsonDocument.ParseValue(ref reader).RootElement.Clone();
			}
		}

		public override void Write(Utf8JsonWriter writer, object objectToWrite, JsonSerializerOptions options)
		{
			JsonSerializer.Serialize(writer, objectToWrite, objectToWrite.GetType(), options);
		}
	}
}
