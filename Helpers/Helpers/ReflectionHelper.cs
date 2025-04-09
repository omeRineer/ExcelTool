using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Helpers
{
    public static class ReflectionHelper
    {
        public static object? GetPropertyValue(object obj, string propertyName)
        {
            object? value = obj;

            foreach (string part in propertyName.Split('.'))
            {
                Type? type = value?.GetType();

                if (type == null)
                    return null;

                PropertyInfo? info = type.GetProperty(part);

                if (info == null)
                    return null;

                value = info.GetValue(value, null);
            }
            return value;
        }
        public static void SetPropertyValue(object obj, string propertyName, object value)
        {
            Type type = obj.GetType();
            PropertyInfo? property = type.GetProperty(propertyName);

            if (property == null)
                throw new Exception($"{propertyName} does not exist!");

            property.SetValue(obj, value, null);
        }
        public static List<Property> GetProperties(Type type, bool isNested = false, Type[]? attributeTypes = null)
        {
            var propertyList = new List<Property>();
            var properties = type.GetProperties();

            if (attributeTypes != null)
                properties = properties.Where(f => attributeTypes.Any(attrType => f.GetCustomAttribute(attrType) != null))
                                       .ToArray();

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;

                if (propertyType.IsClass && propertyType != typeof(string))
                {
                    var nestedPropertyNames = GetProperties(propertyType, true, attributeTypes != null ? attributeTypes : null)
                                                  .Select(nestedProperty => new Property { FullName = nestedProperty.FullName, Info = nestedProperty.Info });
                    
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
