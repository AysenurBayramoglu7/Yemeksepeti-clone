using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace YemekSepeti.WebUI.Models
{
    //alışveriş sepeti gibi geçici verileri hafızada tutmak için kullanılır.
    // Session (Oturum) işlemlerini kolaylaştıran yardımcı sınıf.
    // Nesneleri JSON formatına çevirip kaydeder ve okur.
    public static class SessionExtensions
    {
        // Bir nesneyi JSON string'e çevirip Session'a kaydeder.
        public static void SetJson<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Session'daki JSON string verisini okuyip tekrar nesneye çevirir.
        public static T? GetJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
