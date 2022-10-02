using System;
namespace SimpleCQRS.Core
{
    public class Event : Message
    {
        public Guid Id;
        public int Version;
    }

    public class InventoryItemDeactivated : Event
    {
        public InventoryItemDeactivated(Guid id)
        {
            Id = id;
        }
        
        public override string ToString()
        {
            return $"Inventory item with Id of {Id} is deactivated";
        }
    }

    public class InventoryItemCreated : Event
    {
        public readonly string Name;
        public InventoryItemCreated(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return $"An inventory item with Id of {Id} and name of {Name} is created";
        }
    }

    public class InventoryItemRenamed : Event
    {
        public readonly string NewName;

        public InventoryItemRenamed(Guid id, string newName)
        {
            Id = id;
            NewName = newName;
        }
    }

    public class ItemsCheckedInToInventory : Event
    {
        public readonly int Count;

        public ItemsCheckedInToInventory(Guid id, int count)
        {
            Id = id;
            Count = count;
        }
    }

    public class ItemsRemovedFromInventory : Event
    {
        public readonly int Count;

        public ItemsRemovedFromInventory(Guid id, int count)
        {
            Id = id;
            Count = count;
        }
    }
}