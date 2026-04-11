using UnityEngine;

namespace DangExtension
{

    public static class ClassUtils
    {
        /// <summary> Perform a deep Copy of the object. Binary Serialization is used to perform the copy </summary>
        /// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
        /// <typeparam name="T">The type of object being copied </typeparam>
        /// <param name="source">The object instance to copy </param>
        /// <returns>The copied object </returns>
        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new System.ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (UnityEngine.Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.Stream stream = new System.IO.MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
        /// <summary>
        /// Clone data using Unity Json (safe for [Serializable] classes)
        /// </summary>
        public static T CloneData<T>(this T source)
        {
            if (source == null) return default;

            return JsonUtility.FromJson<T>(JsonUtility.ToJson(source));
        }
        /// <summary>
        /// Clone Unity Object (ScriptableObject, GameObject, etc.)
        /// </summary>
        public static T CloneUnityObject<T>(this T source) where T : Object
        {
            if (source == null) return null;

            return Object.Instantiate(source);
        }
    }

}