using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Helpers
{
    public static class ReflectionHelper
    {
        public static object GetPropertyValue(object obj, string propertyName)
        {
            foreach (string part in propertyName.Split('.'))
            {
                if (obj == null) return null;

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);

                if (info == null) return null;

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public static void SetPropertyValue(object obj, string propertyName, object value)
        {
            Type type = obj.GetType();
            PropertyInfo property = type.GetProperty(propertyName);

            if (property == null) return;

            property.SetValue(obj, value, null);
        }

        public static List<Property> GetProperties(Type type, bool isNested = false)
        {
            var propertyList = new List<Property>();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;

                if (propertyType.IsClass && propertyType != typeof(string))
                {
                    var nestedPropertyNames = GetProperties(propertyType, true).Select(nestedProperty =>
                    {
                        var fullName = isNested ? $"{type.Name}.{nestedProperty}"
                                                : $"{nestedProperty}";

                        return new Property { FullName = fullName, Info = nestedProperty.Info };
                    });
                    propertyList.AddRange(nestedPropertyNames);
                }
                else
                {
                    var item = new Property { Info = property };

                    if (isNested && type.IsClass && type != typeof(string))
                        item.FullName = $"{type.Name}.{property.Name}";
                    else
                        item.FullName = property.Name;

                    propertyList.Add(item);
                }

            }

            return propertyList;
        }
        public static Property GetProperty(Type type, string propertyPath)
        {
            string[] properties = propertyPath.Split('.');
            Property result = new Property
            {
                FullName = propertyPath
            };

            foreach (var property in properties)
            {
                result.Info = type.GetProperty(property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (result.Info == null)
                    throw new ArgumentException($"'{property}' is not a property of '{type.Name}'");

                type = result.Info.PropertyType;
            }

            return result;
        }
    }

    public class Property
    {
        public string FullName { get; set; }
        public PropertyInfo Info { get; set; }
    }
}
