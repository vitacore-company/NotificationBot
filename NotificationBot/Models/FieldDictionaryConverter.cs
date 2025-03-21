using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#nullable disable
namespace NotificationsBot.Models
{
    internal class FieldDictionaryConverter : VssSecureJsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IDictionary<string, object>);
        }

        public override object ReadJson(
          JsonReader reader,
          Type objectType,
          object existingValue,
          JsonSerializer serializer)
        {
            JObject jobject = JObject.Load(reader);
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (KeyValuePair<string, JToken> keyValuePair in jobject)
            {
                object obj = keyValuePair.Value.Type != JTokenType.Object ? ((JValue)keyValuePair.Value).Value : keyValuePair.Value.ToObject<IdentityRef>(serializer);
                dictionary.Add(keyValuePair.Key, obj);
            }
            return dictionary;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}