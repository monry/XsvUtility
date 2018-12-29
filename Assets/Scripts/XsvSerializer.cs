using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Monry.XsvUtility
{
    public static class TsvSerializer
    {
        public static T Deserialize<T>(string text)
        {
            return InternalSerializer.Deserialize<T>(XsvParser.Delimiter.Tab, text);
        }

        public static T DeserializeWithHeader<T>(string text)
        {
            return InternalSerializer.DeserializeWithHeader<T>(XsvParser.Delimiter.Tab, text);
        }

        public static string Serialize<T>(T instance)
        {
            return InternalSerializer.Serialize(XsvParser.Delimiter.Tab, instance);
        }

        public static string Serialize<T>(T instance, IEnumerable<string> header)
        {
            return InternalSerializer.Serialize(XsvParser.Delimiter.Tab, instance, header);
        }
    }

    public static class CsvSerializer
    {
        public static T Deserialize<T>(string text)
        {
            return InternalSerializer.Deserialize<T>(XsvParser.Delimiter.Comma, text);
        }

        public static T DeserializeWithHeader<T>(string text)
        {
            return InternalSerializer.DeserializeWithHeader<T>(XsvParser.Delimiter.Comma, text);
        }

        public static string Serialize<T>(T instance)
        {
            return InternalSerializer.Serialize(XsvParser.Delimiter.Comma, instance);
        }

        public static string Serialize<T>(T instance, IEnumerable<string> header)
        {
            return InternalSerializer.Serialize(XsvParser.Delimiter.Comma, instance, header);
        }
    }

    internal static class InternalSerializer
    {
        private static IDictionary<Type, Func<string, object>> ValueParseDelegateMap { get; } = new Dictionary<Type, Func<string, object>>
        {
            {typeof(bool), value => bool.TryParse(value, out var result) && result},
            {typeof(int), value => int.TryParse(value, out var result) ? result : default},
            {typeof(float), value => float.TryParse(value, out var result) ? result : default},
            {typeof(string), value => value},
        };

        internal static T Deserialize<T>(XsvParser.Delimiter delimiter, string text)
        {
            var rows = XsvParser.Parse(delimiter, text);

            // Boxing to SetValue for struct
            var instance = (object) Activator.CreateInstance<T>();
            var (fields, properties) = CollectMembersForList(typeof(T));
            foreach (var field in fields)
            {
                field.SetValue(instance, GenerateListInstance(field.FieldType.GenericTypeArguments[0], rows));
            }

            foreach (var property in properties)
            {
                property.SetValue(instance, GenerateListInstance(property.PropertyType.GenericTypeArguments[0], rows));
            }

            // Unboxing
            return (T) instance;
        }

        internal static T DeserializeWithHeader<T>(XsvParser.Delimiter delimiter, string text)
        {
            var rows = XsvParser.ParseWithHeader(delimiter, text);

            // Boxing to SetValue for struct
            var instance = (object) Activator.CreateInstance<T>();
            var (fields, properties) = CollectMembersForList(typeof(T));
            foreach (var field in fields)
            {
                field.SetValue(instance, GenerateListInstance(field.FieldType.GenericTypeArguments[0], rows));
            }

            foreach (var property in properties)
            {
                property.SetValue(instance, GenerateListInstance(property.PropertyType.GenericTypeArguments[0], rows));
            }

            // Unboxing
            return (T) instance;
        }

        internal static string Serialize<T>(XsvParser.Delimiter delimiter, T instance)
        {
            var (fields, properties) = CollectMembersForList(typeof(T));
            var result = new List<IEnumerable<string>>();
            foreach (var field in fields)
            {
                var list = (IEnumerable) field.GetValue(instance);
                result.AddRange(list.Cast<object>().Select(item => GenerateRow(field.FieldType.GenericTypeArguments[0], item)));
            }
            foreach (var property in properties)
            {
                var list = (IEnumerable) property.GetValue(instance);
                result.AddRange(list.Cast<object>().Select(item => GenerateRow(property.PropertyType.GenericTypeArguments[0], item)));
            }

            return XsvParser.Compose(delimiter, result);
        }

        internal static string Serialize<T>(XsvParser.Delimiter delimiter, T instance, IEnumerable<string> header)
        {
            header = header.ToList();
            var (fields, properties) = CollectMembersForList(typeof(T));
            var result = new List<IEnumerable<string>>();
            foreach (var field in fields)
            {
                var list = (IEnumerable) field.GetValue(instance);
                result.AddRange(list.Cast<object>().Select(item => GenerateRow(field.FieldType.GenericTypeArguments[0], item, header)));
            }
            foreach (var property in properties)
            {
                var list = (IEnumerable) property.GetValue(instance);
                result.AddRange(list.Cast<object>().Select(item => GenerateRow(property.PropertyType.GenericTypeArguments[0], item, header)));
            }

            return XsvParser.Compose(delimiter, result, header);
        }

        private static IEnumerable<string> GenerateRow(Type type, object instance)
        {
            var (fields, properties) = CollectMembersForItem(type);
            var result = new Dictionary<int, string>();
            foreach (var field in fields)
            {
                result[field.GetCustomAttribute<XsvColumnAttribute>().Index] = field.GetValue(instance).ToString();
            }
            foreach (var property in properties)
            {
                result[property.GetCustomAttribute<XsvColumnAttribute>().Index] = property.GetValue(instance).ToString();
            }

            return result.OrderBy(x => x.Key).Select(x => x.Value);
        }

        private static IEnumerable<string> GenerateRow(Type type, object instance, IEnumerable<string> header)
        {
            var (fields, properties) = CollectMembersForItem(type);
            var result = new Dictionary<string, string>();
            foreach (var field in fields)
            {
                result[field.GetCustomAttribute<XsvColumnAttribute>().Name] = field.GetValue(instance).ToString();
            }
            foreach (var property in properties)
            {
                result[property.GetCustomAttribute<XsvColumnAttribute>().Name] = property.GetValue(instance).ToString();
            }

            return result.Where(x => header.Contains(x.Key)).OrderBy(x => header.ToList().IndexOf(x.Key)).Select(x => x.Value);
        }

        private static object GenerateListInstance(Type type, IEnumerable<IList<string>> rows)
        {
            var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
            var addMethod = list.GetType().GetMethod("Add");

            foreach (var row in rows)
            {
                // ReSharper disable once PossibleNullReferenceException
                addMethod.Invoke(list, new[] {GenerateItemInstance(type, row)});
            }

            return list;
        }

        private static object GenerateListInstance(Type type, IEnumerable<IDictionary<string, string>> rows)
        {
            var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
            var addMethod = list.GetType().GetMethod("Add");

            foreach (var row in rows)
            {
                // ReSharper disable once PossibleNullReferenceException
                addMethod.Invoke(list, new[] {GenerateItemInstance(type, row)});
            }

            return list;
        }

        private static object GenerateItemInstance(Type type, IList<string> row)
        {
            var instance = Activator.CreateInstance(type);
            var (fields, properties) = CollectMembersForItem(type);
            foreach (var field in fields)
            {
                var columnAttribute = field.GetCustomAttribute<XsvColumnAttribute>();
                if (row.Count > columnAttribute.Index)
                {
                    field.SetValue(instance, ParseValue(field.FieldType, row[columnAttribute.Index]));
                }
            }

            foreach (var property in properties)
            {
                var columnAttribute = property.GetCustomAttribute<XsvColumnAttribute>();
                if (row.Count > columnAttribute.Index)
                {
                    property.SetValue(instance, ParseValue(property.PropertyType, row[columnAttribute.Index]));
                }
            }

            return instance;
        }

        private static object GenerateItemInstance(Type type, IDictionary<string, string> row)
        {
            var instance = Activator.CreateInstance(type);
            var (fields, properties) = CollectMembersForItem(type);
            foreach (var field in fields)
            {
                var columnAttribute = field.GetCustomAttribute<XsvColumnAttribute>();
                if (row.ContainsKey(columnAttribute.Name))
                {
                    field.SetValue(instance, ParseValue(field.FieldType, row[columnAttribute.Name]));
                }
            }

            foreach (var property in properties)
            {
                var columnAttribute = property.GetCustomAttribute<XsvColumnAttribute>();
                if (row.ContainsKey(columnAttribute.Name))
                {
                    property.SetValue(instance, ParseValue(property.PropertyType, row[columnAttribute.Name]));
                }
            }

            return instance;
        }

        private static (IEnumerable<FieldInfo> fields, IEnumerable<PropertyInfo> properties) CollectMembersForList(Type type)
        {
            return (
                type
                    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(
                        x =>
                            x.GetCustomAttribute<XsvRowAttribute>() != null &&
                            x.FieldType.IsGenericType &&
                            x.FieldType.GenericTypeArguments.Length > 0 &&
                            typeof(IEnumerable).IsAssignableFrom(x.FieldType)
                    ),
                type
                    .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(
                        x =>
                            x.GetCustomAttribute<XsvRowAttribute>() != null &&
                            x.PropertyType.IsGenericType &&
                            x.PropertyType.GenericTypeArguments.Length > 0 &&
                            typeof(IEnumerable).IsAssignableFrom(x.PropertyType)
                    )
            );
        }

        private static (IEnumerable<FieldInfo> fields, IEnumerable<PropertyInfo> properties) CollectMembersForItem(Type type)
        {
            return (
                type
                    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(
                        x =>
                            x.GetCustomAttribute<XsvColumnAttribute>() != null
                    ),
                type
                    .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(
                        x =>
                            x.GetCustomAttribute<XsvColumnAttribute>() != null
                    )
            );
        }

        private static object ParseValue(Type type, string value)
        {
            return ValueParseDelegateMap.ContainsKey(type) ? ValueParseDelegateMap[type](value) : default;
        }
    }
}