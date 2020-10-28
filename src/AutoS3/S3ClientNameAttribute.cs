﻿using System;
using System.Reflection;

namespace AutoS3
{
    public class S3ClientNameAttribute : Attribute
    {
        public string Name { get; }

        public S3ClientNameAttribute(string name)
        {
            Name = name;
        }

        public virtual string GetName(Type type)
        {
            return Name;
        }

        public static string GetClientName<T>()
        {
            return GetClientName(typeof(T));
        }

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
