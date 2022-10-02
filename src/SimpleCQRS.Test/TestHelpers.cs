using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SimpleCQRS.Core;

namespace SimpleCQRS.Test;

public static class TestHelpers
{
    /// <summary>
    /// Asserts all properties of the provided objects are equal.
    /// </summary>
    /// <param name="expected">The expected object.</param>
    /// <param name="actual">The actual object.</param>
    public static void AssertAllPropertiesAreEqual(object expected, object actual)
    {
        if (IsSimpleType(actual))
        {
            if (!Equals(expected, actual))
            {
                Assert.Fail("Objects do not match. Expected: {0} but was: {1}", expected, actual);
            }

            return;
        }
        else if (actual is IEnumerable)
        {
            AssertEnumerablesAreEqual(null, (IEnumerable)expected, (IEnumerable)actual);
            return;
        }

        PropertyInfo[] properties = expected.GetType().GetProperties();
        foreach (PropertyInfo property in properties)
        {
            object expectedValue = property.GetValue(expected, null);
            object actualValue = property.GetValue(actual, null);

            if (expectedValue == null && actualValue != null)
            {
                Assert.Fail("Property {0}.{1} does not match. Expected: null but was: {2}", property.DeclaringType.Name,
                    property.Name, actualValue);
            }
            else if (expectedValue != null && actualValue == null)
            {
                Assert.Fail("Property {0}.{1} does not match. Expected: {2} but was: null", property.DeclaringType.Name,
                    property.Name, expectedValue);
            }
            else if (expectedValue == null && actualValue == null)
            {
                continue;
            }

            if (expectedValue.GetType() != actualValue.GetType())
            {
                Assert.Fail("Property {0}.{1} does not match. Expected: {2} but was: {3}", property.DeclaringType.Name,
                    property.Name, expectedValue, actualValue);
            }

            if (IsSimpleType(actualValue))
            {
                if (!Equals(expectedValue, actualValue))
                {
                    Assert.Fail("Property {0}.{1} does not match. Expected: {2} but was: {3}",
                        property.DeclaringType.Name, property.Name, expectedValue, actualValue);
                }
            }
            else if (actualValue is IEnumerable)
            {
                AssertEnumerablesAreEqual(property, (IEnumerable)expectedValue, (IEnumerable)actualValue);
            }
            else
            {
                AssertAllPropertiesAreEqual(expectedValue, actualValue);
            }
        }
    }

    // from https://stackoverflow.com/a/32337906/2785615
    private static bool IsSimpleType(object obj)
    {
        if (obj == null)
        {
            return true;
        }

        Type type = obj.GetType();
        return
            type.IsPrimitive ||
            type == typeof(string) ||
            type == typeof(decimal) ||
            type == typeof(DateTime) ||
            type == typeof(DateTimeOffset) ||
            type == typeof(TimeSpan) ||
            type == typeof(Guid) ||
            type.IsEnum ||
            Convert.GetTypeCode(type) != TypeCode.Object ||
            (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
             IsSimpleType(type.GetGenericArguments()[0]));
    }

    /// <summary>
    /// Asserts 2 enumerables are equal.
    /// </summary>
    /// <param name="expected">The expected enumerable.</param>
    /// <param name="actual">The actual enumerable.</param>
    public static void AssertEnumerablesAreEqual(IEnumerable expected, IEnumerable actual)
    {
        AssertEnumerablesAreEqual(null, expected, actual);
    }

    private static void AssertEnumerablesAreEqual(PropertyInfo property, IEnumerable expected, IEnumerable actual)
    {
        if (actual is IList)
        {
            AssertListsAreEqual(property, (IList)expected, (IList)actual);
            return;
        }

        if (actual is IDictionary)
        {
            AssertDictionariesAreEqual(property, (IDictionary)expected, (IDictionary)actual);
        }

        // loop through expected and make sure all its elements are in actual
        foreach (object expectedElement in expected)
        {
            bool foundMatch = false;
            foreach (object actualElement in actual)
            {
                if (CheckAllPropertiesAreEqual(expectedElement, actualElement))
                {
                    foundMatch = true;
                    break;
                }
            }

            if (!foundMatch)
            {
                if (property == null)
                {
                    Assert.Fail("Expected IEnumerable containing {0} but it was not found.", expectedElement);
                }
                else
                {
                    Assert.Fail(
                        "Property {0}.{1} does not match. Expected IEnumerable containing {2} but it was not found.",
                        property.PropertyType.Name, property.Name, expectedElement);
                }
            }
        }

        // loop through actual and make sure all its elements are in expected
        foreach (object actualElement in actual)
        {
            bool foundMatch = false;
            foreach (object expectedElement in expected)
            {
                if (CheckAllPropertiesAreEqual(expectedElement, actualElement))
                {
                    foundMatch = true;
                    break;
                }
            }

            if (!foundMatch)
            {
                if (property == null)
                {
                    Assert.Fail("Expected IEnumerable not containing {0}, but it was found.", actualElement);
                }
                else
                {
                    Assert.Fail(
                        "Property {0}.{1} does not match. Expected IEnumerable not containing {2}, but it was found.",
                        property.PropertyType.Name, property.Name, actualElement);
                }
            }
        }
    }

    private static void AssertListsAreEqual(PropertyInfo property, IList expected, IList actual)
    {
        if (actual.Count != expected.Count)
        {
            if (property == null)
            {
                Assert.Fail("Expected IList containing {0} elements but was IList containing {1} elements",
                    expected.Count, actual.Count);
            }
            else
            {
                Assert.Fail(
                    "Property {0}.{1} does not match. Expected IList containing {2} elements but was IList containing {3} elements",
                    property.PropertyType.Name, property.Name, expected.Count, actual.Count);
            }
        }

        for (int i = 0; i < actual.Count; i++)
        {
            AssertAllPropertiesAreEqual(expected[i], actual[i]);
        }
    }

    private static void AssertDictionariesAreEqual(PropertyInfo property, IDictionary expected, IDictionary actual)
    {
        if (actual.Count != expected.Count)
        {
            if (property == null)
            {
                Assert.Fail("Expected IDictionary containing {0} elements but was IDictionary containing {1} elements",
                    expected.Count, actual.Count);
            }
            else
            {
                Assert.Fail(
                    "Property {0}.{1} does not match. Expected IDictionary containing {2} elements but was IDictionary containing {3} elements",
                    property.PropertyType.Name, property.Name, expected.Count, actual.Count);
            }
        }

        foreach (object key in expected.Keys)
        {
            if (!actual.Contains(key))
            {
                if (property == null)
                {
                    Assert.Fail("Expected IDictionary containing key {0} but it was not found.", key);
                }
                else
                {
                    Assert.Fail(
                        "Property {0}.{1} does not match. Expected IDictionary containing key {2} but it was not found.",
                        property.PropertyType.Name, property.Name, key);
                }
            }

            AssertAllPropertiesAreEqual(expected[key], actual[key]);
        }
    }

    private static bool CheckAllPropertiesAreEqual(object expected, object actual)
    {
        try
        {
            AssertAllPropertiesAreEqual(expected, actual);
            return true;
        }
        catch (AssertionException)
        {
            return false;
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
            var expected = specification.Expect();
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
