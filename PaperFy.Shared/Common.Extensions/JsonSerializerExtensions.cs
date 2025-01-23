using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaperFy.Shared.Common.Extensions
{

	public static class JsonSerializerExtensions
	{
		private static JsonDocumentOptions DocumentOptions;

		private static JsonSerializerOptions SerializerOptions;

		static JsonSerializerExtensions()
		{
			DocumentOptions = new JsonDocumentOptions
			{
				AllowTrailingCommas = true,
				CommentHandling = JsonCommentHandling.Skip
			};
			SerializerOptions = new JsonSerializerOptions
			{
				Converters =
			{
				(JsonConverter)new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower),
				(JsonConverter)new ObjectToInferredTypesConverter()
			},
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
				WriteIndented = true
			};
		}

		public static T Deserialize<T>(this string source)
		{
			return (T)JsonSerializer.Deserialize(source, typeof(T), SerializerOptions);
		}

		public static object Deserialize(this string source, Type type)
		{
			return JsonSerializer.Deserialize(source, type, SerializerOptions);
		}

		public static T Duplicate<T>(this object source)
		{
			return source.Serialize().Deserialize<T>();
		}

		public static void Populate(this object source, string json)
		{
			PopulateObject(source, json);
		}

		public static string Serialize(this object source)
		{
			return JsonSerializer.Serialize(source, SerializerOptions);
		}

		private static void PopulateObject<T>(T target, string json) where T : class
		{
			PopulateObject(target, json, target.GetType());
		}

		private static object PopulateObject(object target, string json, Type type)
		{
			if (string.IsNullOrEmpty(json) || json == "null")
			{
				return null;
			}
			JsonElement rootElement = JsonDocument.Parse(json, DocumentOptions).RootElement;
			if (rootElement.ValueKind == JsonValueKind.Array)
			{
				return PopulateArrayProperty(json, type);
			}
			foreach (JsonProperty item in rootElement.EnumerateObject())
			{
				PopulateObjectProperty(target, item, type);
			}
			return target;
		}

		private static void PopulateObjectProperty(object target, JsonProperty property, Type type)
		{
			PropertyInfo propertyInfo = type.GetProperty(property.Name.Replace("\"", string.Empty));
			if (propertyInfo == null)
			{
				PropertyInfo[] properties = type.GetProperties();
				foreach (PropertyInfo propertyInfo2 in properties)
				{
					object[] customAttributes = propertyInfo2.GetCustomAttributes(inherit: true);
					for (int j = 0; j < customAttributes.Length; j++)
					{
						if (customAttributes[j] is JsonPropertyNameAttribute jsonPropertyNameAttribute && jsonPropertyNameAttribute.Name == property.Name)
						{
							propertyInfo = propertyInfo2;
							break;
						}
					}
					if (propertyInfo != null)
					{
						break;
					}
				}
			}
			if (!(propertyInfo == null))
			{
				object obj = null;
				if (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(string))
				{
					obj = JsonSerializer.Deserialize(property.Value.GetRawText(), propertyInfo.PropertyType, SerializerOptions);
				}
				else if (propertyInfo.PropertyType.IsArray)
				{
					obj = propertyInfo.GetValue(target);
					obj = PopulateArrayProperty(property.Value.GetRawText(), propertyInfo.PropertyType);
				}
				else
				{
					obj = propertyInfo.GetValue(target);
					PopulateObject(obj, property.Value.GetRawText(), propertyInfo.PropertyType);
				}
				propertyInfo.SetValue(target, obj);
			}
		}

		private static IEnumerable PopulateArrayProperty(string json, Type type)
		{
			if (string.IsNullOrEmpty(json) || json == "null")
			{
				return null;
			}
			JsonElement rootElement = JsonDocument.Parse(json, DocumentOptions).RootElement;
			if (rootElement.ValueKind == JsonValueKind.Array && rootElement.GetArrayLength() > 0)
			{
				Type type2 = type.GetInterfaces().FirstOrDefault((Type i) => i.GenericTypeArguments.Any() && i.FullName.Contains("IEnumerable"));
				if (type2 == null)
				{
					return null;
				}
				Type type3 = type2.GenericTypeArguments[0];
				Array array = Array.CreateInstance(type3, rootElement.GetArrayLength());
				for (int j = 0; j < rootElement.GetArrayLength(); j++)
				{
					array.SetValue(rootElement[j].GetRawText().Deserialize(type3), j);
				}
				return array;
			}
			return null;
		}
	}
}
