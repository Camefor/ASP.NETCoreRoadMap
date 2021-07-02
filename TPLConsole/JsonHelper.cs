using Newtonsoft.Json;

namespace TPLConsole
{
    public static class JsonHelper
    {
        public static string ToJson(this object value)
        {
            try
            {
                return JsonConvert.SerializeObject(value, Formatting.Indented);
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// JSON对象序列化为JSON字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TrySerializeObject(object value)
        {
            try
            {
                return JsonConvert.SerializeObject(value, Formatting.Indented);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// JSON字符串反序列化为JSON对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T TryDeserializeObject<T>(string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch
            {
                return default(T);
            }
        }
    }
}
