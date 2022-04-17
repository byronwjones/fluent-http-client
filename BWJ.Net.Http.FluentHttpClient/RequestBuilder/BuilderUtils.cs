using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace BWJ.Net.Http.RequestBuilder
{
    internal static class BuilderUtils
    {
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairCollection<T>(T obj)
            where T : class
        {
            if(obj is IEnumerable<KeyValuePair<string, string>>)
            {
                return obj as IEnumerable<KeyValuePair<string, string>>;
            }

            NotNullOrEnumerable(obj, nameof(obj));

            var collection = new List<KeyValuePair<string, string>>();
            var formType = obj.GetType();
            var formProps = formType.GetProperties();
            if(formProps.Length == 0)
            {
                throw new ArgumentException("Object must have public properties", nameof(obj));
            }

            foreach (var prop in formProps)
            {
                if(prop.GetCustomAttribute<JsonIgnoreAttribute>() is null)
                {
                    var name = GetFormName(prop);
                    object propValue = prop.GetValue(obj);
                    if(propValue is IDictionary)
                    {
                        var dict = prop.GetValue(obj) as IDictionary;
                        foreach (var key in dict.Keys)
                        {
                            collection.Add(new KeyValuePair<string, string>($"{name}[{key}]", dict[key]?.ToString() ?? string.Empty));
                        }
                    }
                    else if(propValue is not string && propValue is IEnumerable)
                    {
                        var arr = prop.GetValue(obj) as IEnumerable;
                        foreach(var item in arr)
                        {
                            collection.Add(new KeyValuePair<string, string>($"{name}[]", item?.ToString() ?? string.Empty));
                        }
                    }
                    else
                    {
                        collection.Add(new KeyValuePair<string, string>(name, prop.GetValue(obj)?.ToString() ?? string.Empty));
                    }
                }
            }

            return collection;
        }

        public static string GetFormName(PropertyInfo property)
        {
            var nameAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();
            return nameAttribute != null ? nameAttribute.PropertyName : property.Name;
        }

        public static void NotNullOrEnumerable(object parameter, string parameterName)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(parameterName);
            }
            if (parameter is IEnumerable)
            {
                throw new ArgumentException("Enumerable objects are not valid", parameterName);
            }
        }
    }
}
