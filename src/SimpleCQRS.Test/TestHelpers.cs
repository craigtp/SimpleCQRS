using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleCQRS.Core;

namespace SimpleCQRS.Test;

public static class TestHelpers
{
    public static bool DeepEquals(object? obj1, object? obj2, List<string>? elementsToIgnore = null)
    {
        elementsToIgnore ??= new List<string>();
        
        if (ReferenceEquals(obj1, obj2))
            return true;

        if (obj1 == null || obj2 == null)
            return false;

        var type1 = obj1.GetType();
        var type2 = obj2.GetType();

        if (type1 != type2)
            return false;

        if (IsSimpleType(type1))
        {
            if (!obj1.Equals(obj2))
            {
                return false;
            }
        }

        if (obj1 is IEnumerable enum1 && obj2 is IEnumerable enum2)
            return DeepSequenceEquals(enum1.Cast<object>(), enum2.Cast<object>(), elementsToIgnore);

        var fields = type1.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
        var properties = type1.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

        fields.RemoveAll(f => elementsToIgnore.Contains(f.Name));
        properties.RemoveAll(p => elementsToIgnore.Contains(p.Name));

        foreach (var field in fields)
        {
            var value1 = field.GetValue(obj1);
            var value2 = field.GetValue(obj2);

            if (!DeepEquals(value1, value2, elementsToIgnore))
                return false;
        }

        foreach (var prop in properties)
        {
            var value1 = prop.GetValue(obj1);
            var value2 = prop.GetValue(obj2);

            if (!DeepEquals(value1, value2, elementsToIgnore))
                return false;
        }

        return true;
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive || type.IsValueType || type == typeof(string);
    }

    private static bool DeepSequenceEquals(IEnumerable<object> seq1, IEnumerable<object> seq2, List<string>? elementsToIgnore)
    {
        using (var enumerator1 = seq1.GetEnumerator())
        using (var enumerator2 = seq2.GetEnumerator())
        {
            while (enumerator1.MoveNext() && enumerator2.MoveNext())
            {
                if (!DeepEquals(enumerator1.Current, enumerator2.Current, elementsToIgnore))
                    return false;
            }

            return !enumerator1.MoveNext() && !enumerator2.MoveNext();
        }
    }

    public static void PrintTest<TCommand>(EventSpecification<TCommand> specification) where TCommand : Command
    {
        Console.WriteLine("Specification: " + specification.GetType().Name.Replace("_", " "));
        Console.WriteLine();
        Console.WriteLine("Given:");
        var existingEvents = specification.Given().ToList();
        if (!existingEvents.Any())
        {
            Console.WriteLine("\t" + "[No existing events]");
        }
        else
        {
            var firstEvent = true;
            foreach (var @event in existingEvents)
            {
                Console.WriteLine("\t" + (firstEvent ? string.Empty : "and ") + @event);
                firstEvent = false;
            }
        }

        Console.WriteLine();
        Console.WriteLine("When:");
        Console.WriteLine("\t" + specification.When());
        Console.WriteLine();
        Console.WriteLine("Expect:");
        try
        {
            var expected = specification.Then();
            var firstEvent = true;
            foreach (var @event in expected)
            {
                Console.WriteLine("\t" + (firstEvent ? string.Empty : "and ") + @event);
                firstEvent = false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("\t" + $"An exception of type {ex.GetType()} is thrown.");
        }

        Console.WriteLine();
    }
}