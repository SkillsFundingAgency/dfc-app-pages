using Newtonsoft.Json;

namespace DFC.App.Pages.Cms.Extension
{
    public static class SessionExtensions
    {
        /// <summary>
        /// Sets the object as json.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// Gets the object from json.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="key">The key.</param>
        /// <returns>The result type.</returns>
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
