using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SimpleCQRS.Test
{
    public class ObjectComparer
    {
        private readonly StringBuilder _differences = new();
        
        public ComparisonResult Compare(object obj1, object obj2, List<string>? elementsToIgnore = null)
        {
            elementsToIgnore ??= new List<string>();
            
            Compare(obj1, obj2, _differences, string.Empty, elementsToIgnore);

            return new ComparisonResult(_differences.Length == 0, _differences.ToString());
        }
        
        private void Compare(object? obj1, object? obj2, StringBuilder differences, string path, List<string> elementsToIgnore)
        {
            if (ReferenceEquals(obj1, obj2)) return;
            if (obj1 == null || obj2 == null)
            {
                differences.AppendLine($"{PathOutput(path)}{ObjectString(obj1)} != {ObjectString(obj2)}");
                return;
            }

            var type1 = obj1.GetType();
            var type2 = obj2.GetType();

            if (type1 != type2)
            {
                differences.AppendLine($"{PathOutput(path)}Types differ ({type1} != {type2})");
                return;
            }

            if (IsSimpleType(type1))
            {
                if (!obj1.Equals(obj2))
                {
                    var separator = path == string.Empty ? "" : ": ";
                    differences.AppendLine($"{path}{separator}{ObjectString(obj1)} != {ObjectString(obj2)}");
                }
                return;
            }

            if (typeof(IEnumerable).IsAssignableFrom(type1))
            {
                CompareEnumerables((IEnumerable)obj1, (IEnumerable)obj2, differences, path, elementsToIgnore);
                return;
            }
            
            foreach (var property in type1.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!property.CanRead || elementsToIgnore.Contains(property.Name)) continue;
                
                var value1 = property.GetValue(obj1);
                var value2 = property.GetValue(obj2);

                var sanitizedPath = string.IsNullOrEmpty(path) ? $"{{{type1}}}" : path;
                Compare(value1, value2, differences, $"{sanitizedPath}.{property.Name}", elementsToIgnore);
            }

            foreach (var field in type1.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (elementsToIgnore.Contains(field.Name)) continue;
                
                var value1 = field.GetValue(obj1);
                var value2 = field.GetValue(obj2);
                
                var sanitizedPath = string.IsNullOrEmpty(path) ? $"{{{type1}}}" : path;
                Compare(value1, value2, differences, $"{sanitizedPath}.{field.Name}", elementsToIgnore);
            }
        }

        private void CompareEnumerables(IEnumerable enum1, IEnumerable enum2, StringBuilder differences, string path, List<string> elementsToIgnore)
        {
            var list1 = enum1.Cast<object>().ToList();
            var list2 = enum2.Cast<object>().ToList();

            if (list1.Count != list2.Count)
            {
                differences.AppendLine($"{PathOutput(path)}Collection sizes differ ({list1.Count} != {list2.Count})");
                return;
            }

            for (var i = 0; i < list1.Count; i++)
            {
                Compare(list1[i], list2[i], differences, $"{path}[{i}]", elementsToIgnore);
            }
        }

        private bool IsSimpleType(Type type)
        {
            return type.IsPrimitive ||
                   type.IsEnum ||
                   type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid);
        }
        
        private string ObjectString(object? obj)
        {
            try
            {
                return obj switch
                {
                    null => "(null)",
                    DateTime dateObj => dateObj.ToString("o", CultureInfo.InvariantCulture),
                    DateTimeOffset dateObj => dateObj.ToString("o", CultureInfo.InvariantCulture),
                    _ => obj.ToString()!
                };
            }
            catch
            {
                return string.Empty;
            }
        }

        private string PathOutput(string path)
        {
            return string.IsNullOrWhiteSpace(path) ? "" : $"{path}: ";
        }
    }

    public class ComparisonResult
    {
        public bool Success { get; }
        public string Differences { get; }

        public ComparisonResult(bool success, string differences)
        {
            Success = success;
            Differences = differences;
        }
    }
}