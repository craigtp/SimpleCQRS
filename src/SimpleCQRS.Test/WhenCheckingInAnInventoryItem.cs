using System;
using System.Collections.Generic;
using SimpleCQRS.Core;

namespace SimpleCQRS.Test;

public class When_Checking_In_An_Inventory_Item : EventSpecification<CheckInItemsToInventory>
{
    private readonly Guid _inventoryItemId = Guid.NewGuid();
    private readonly int _amountToCheckIn = 10;

    public override IEnumerable<Event> Given()
    {
        yield return new InventoryItemCreated(_inventoryItemId, _inventoryItemId.ToString());
    }

    public override CheckInItemsToInventory When()
    {
        return new CheckInItemsToInventory(_inventoryItemId, _amountToCheckIn, 1);
    }

    protected override ICommandHandler<CheckInItemsToInventory> BuildCommandHandler()
    {
        return new InventoryCommandHandlers(new Repository<InventoryItem>(FakeStore));
    }

    public override IEnumerable<Event> Then()
    {
        yield return new ItemsCheckedInToInventory(_inventoryItemId, _amountToCheckIn);
    }

    public override Exception? ThenException()
    {
        return NoException();
    }    
}