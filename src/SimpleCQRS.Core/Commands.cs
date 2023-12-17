using System;
namespace SimpleCQRS.Core
{
    public class Command : Message
    {
    }

    public class DeactivateInventoryItem : Command
    {
        public readonly Guid InventoryItemId;
        public readonly int OriginalVersion;

        public DeactivateInventoryItem(Guid inventoryItemId, int originalVersion)
        {
            InventoryItemId = inventoryItemId;
            OriginalVersion = originalVersion;
        }
        
        public override string ToString()
        {
            return $"deactivate inventory item with Id of {InventoryItemId}";
        }
    }

    public class CreateInventoryItem : Command
    {
        public readonly Guid InventoryItemId;
        public readonly string Name;

        public CreateInventoryItem(Guid inventoryItemId, string name)
        {
            InventoryItemId = inventoryItemId;
            Name = name;
        }

        public override string ToString()
        {
            return $"create inventory item with Id of {InventoryItemId} and name of {Name}";
        }
    }

    public class RenameInventoryItem : Command
    {
        public readonly Guid InventoryItemId;
        public readonly string NewName;
        public readonly int OriginalVersion;

        public RenameInventoryItem(Guid inventoryItemId, string newName, int originalVersion)
        {
            InventoryItemId = inventoryItemId;
            NewName = newName;
            OriginalVersion = originalVersion;
        }
        
        public override string ToString()
        {
            return $"rename inventory item with Id of {InventoryItemId} and original name of {OriginalVersion} to {NewName}";
        }
    }

    public class CheckInItemsToInventory : Command
    {
        public Guid InventoryItemId;
        public readonly int Count;
        public readonly int OriginalVersion;

        public CheckInItemsToInventory(Guid inventoryItemId, int count, int originalVersion)
        {
            InventoryItemId = inventoryItemId;
            Count = count;
            OriginalVersion = originalVersion;
        }
        
        public override string ToString()
        {
            return $"checking in {Count} items to inventory item with Id of {InventoryItemId}";
        }
    }

    public class RemoveItemsFromInventory : Command
    {
        public Guid InventoryItemId;
        public readonly int Count;
        public readonly int OriginalVersion;

        public RemoveItemsFromInventory(Guid inventoryItemId, int count, int originalVersion)
        {
            InventoryItemId = inventoryItemId;
            Count = count;
            OriginalVersion = originalVersion;
        }
        
        public override string ToString()
        {
            return $"removing {Count} items from inventory item with Id of {InventoryItemId}";
        }
    }
}
