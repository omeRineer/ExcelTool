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

        public static List<string> GetPropertiesName(Type type, bool isNested = false)
        {
            var propertyNames = new List<string>();

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;

                if (propertyType.IsClass && propertyType != typeof(string))
                {
                    var nestedPropertyNames = GetPropertiesName(propertyType, true).Select(nestedPropertyName =>
                    {
                        return isNested ? $"{type.Name}.{nestedPropertyName}" : $"{nestedPropertyName}";
                    });
                    propertyNames.AddRange(nestedPropertyNames);
                }
                else
                {
                    if (isNested && type.IsClass && type != typeof(string))
                        propertyNames.Add($"{type.Name}.{property.Name}");
                    else
                        propertyNames.Add(property.Name);
                }

            }

            return propertyNames;
        }
    }
}
