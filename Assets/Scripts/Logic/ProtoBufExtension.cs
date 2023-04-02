using System;
using System.IO;

namespace Logic
{
    /// <summary>
    /// ProtoBuf Extensions.
    /// <para>copied from internet, copyright reserved by the owner</para>
    /// </summary>
    public static class ProtoBufExtension
    {
        /// <summary>
        /// serialize object to base64 encoded string
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="obj">object instance</param>
        /// <returns>base64 encoded string</returns>
        public static string SerializeToString_PB<T>(this T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, obj);
                return Convert.ToBase64String(ms.GetBuffer(), 0, (int) ms.Length);
            }
        }

        /// <summary>
        /// deserialize object from base64 encoded string
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="txt">base64 encoded string</param>
        /// <returns>object instance</returns>
        public static T DeserializeFromString_PB<T>(this string txt)
        {
            byte[] arr = Convert.FromBase64String(txt);
            using (MemoryStream ms = new MemoryStream(arr))
                return ProtoBuf.Serializer.Deserialize<T>(ms);
        }

        /// <summary>
        /// serialize object to byte array
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="obj">object instance</param>
        /// <returns>byte array</returns>
        public static byte[] SerializeToByteAry_PB<T>(this T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// deserialize object from byte array
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="arr">byte array</param>
        /// <returns>object instance</returns>
        public static T DeserializeFromByteAry_PB<T>(this byte[] arr)
        {
            using (MemoryStream ms = new MemoryStream(arr))
                return ProtoBuf.Serializer.Deserialize<T>(ms);
        }

        /// <summary>
        /// serialize object to file
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="obj">object instance</param>
        /// <param name="path">target file path</param>
        public static void SerializeToFile_PB<T>(this T obj, string path)
        {
            using (var file = File.Create(path))
            {
                ProtoBuf.Serializer.Serialize(file, obj);
            }
        }

        /// <summary>
        /// deserialize object from file
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="path">target file path</param>
        /// <returns>object instance</returns>
        public static T DeserializeFromFile_PB<T>(this string path)
        {
            using (var file = File.OpenRead(path))
            {
                return ProtoBuf.Serializer.Deserialize<T>(file);
            }
        }
    }
}
