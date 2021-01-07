using System;
using System.Reflection;

namespace AutoS3
{
    /// <summary>
    /// S3 Client Name attribute
    /// </summary>
    public class S3ClientNameAttribute : Attribute
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name"></param>
        public S3ClientNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Get name by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual string GetName(Type type)
        {
            return Name;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetClientName<T>()
        {
            return GetClientName(typeof(T));
        }
        
        /// <summary>
        /// Get client name by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetClientName(Type type)
        {
            var nameAttribute = type.GetCustomAttribute<S3ClientNameAttribute>();

            if (nameAttribute == null)
            {
                return type.FullName;
            }

            return nameAttribute.GetName(type);
        }
    }
}
