using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace BWJ.Net.Http.RequestBuilder
{
    internal static class BuilderUtils
    {
        public static Dictionary<string, string> ToDictionary<T>(T obj)
            where T : class
        {
            if(obj is Dictionary<string, string>)
            {
                return obj as Dictionary<string, string>;
            }

            NotNullOrEnumerable(obj, nameof(obj));

            var dict = new Dictionary<string, string>();
            var formType = obj.GetType();
            var formProps = formType.GetProperties();
            foreach (var prop in formProps)
            {
                var name = GetFormName(prop);
                dict.Add(name, prop.GetValue(obj)?.ToString());
            }

            return dict;
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
