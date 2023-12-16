using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleCQRS.Core;

namespace SimpleCQRS.Test;

[TestFixture]
public class AssertPropertiesAreEqualTests
{
    [Test]
    public void TwoDifferentObjectsAreNotEqual1()
    {
        var gooId = Guid.NewGuid();
        var obj1 = new InventoryItemCreated(gooId, "Object1");
        var obj2 = new InventoryItemCreated(gooId, "Object2");

        Assert.False(TestHelpers.DeepEquals(obj1,obj2));
    }
    
    [Test]
    public void TwoObjectsAreEqual1()
    {
        var gooId = Guid.NewGuid();
        var obj1 = new InventoryItemCreated(gooId, "Object1");
        var obj2 = new InventoryItemCreated(gooId, "Object1");

        Assert.True(TestHelpers.DeepEquals(obj1,obj2));
    }

    [Test]
    public void TwoPrimitivesAreEqual1()
    {
        int int1 = 50;
        int int2 = 50;
        Assert.True(TestHelpers.DeepEquals(int1,int2));
    }
    
    [Test]
    public void TwoPrimitivesAreEqual2()
    {
        Guid gooId1 = Guid.NewGuid();
        Guid gooId2 = Guid.Parse(gooId1.ToString());
        Assert.True(TestHelpers.DeepEquals(gooId1,gooId2));
    }
    
    [Test]
    public void TwoDifferentPrimitivesAreNotEqual1()
    {
        Guid gooId1 = Guid.NewGuid();
        Guid gooId2 = Guid.NewGuid();
        Assert.False(TestHelpers.DeepEquals(gooId1,gooId2));
    }

    [Test]
    public void TwoListsAreEqual1()
    {
        var list1 = new List<string>() { "item1", "item2" };
        var list2 = new List<string>() { "item1", "item2" };
        Assert.True(TestHelpers.DeepEquals(list1,list2));
        
    }
    
    [Test]
    public void TwoDifferentListsAreNotEqual1()
    {
        var list1 = new List<string>() { "item1", "item2" };
        var list2 = new List<string>() { "item1", "item3" };
        Assert.False(TestHelpers.DeepEquals(list1,list2));
        
    }

    [Test]
    public void TwoDictionariesAreEqual1()
    {
        var dict1 = new Dictionary<int, string>() { { 1, "item1" }, {2, "item2"} };
        var dict2 = new Dictionary<int, string>() { { 1, "item1" }, {2, "item2"} };
        Assert.True(TestHelpers.DeepEquals(dict1,dict2));
    }
    
    [Test]
    public void TwoDifferentDictionariesAreNotEqual1()
    {
        var dict1 = new Dictionary<int, string>() { { 1, "item1" }, {2, "item2"} };
        var dict2 = new Dictionary<int, string>() { { 3, "item1" }, {2, "item2"} };
        Assert.False(TestHelpers.DeepEquals(dict1,dict2));
    }
}