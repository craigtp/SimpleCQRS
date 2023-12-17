using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleCQRS.Core;

namespace SimpleCQRS.Test;

[TestFixture]
public class When_Deactivating_An_Inventory_Item : EventSpecification<DeactivateInventoryItem>
{
    private readonly Guid _inventoryItemId = Guid.NewGuid();

    public override IEnumerable<Event> Given()
    {
        yield return new InventoryItemCreated(_inventoryItemId, _inventoryItemId.ToString());
    }

    public override DeactivateInventoryItem When()
    {
        return new DeactivateInventoryItem(_inventoryItemId, 1);
    }

    protected override ICommandHandler<DeactivateInventoryItem> BuildCommandHandler()
    {
        return new InventoryCommandHandlers(new Repository<InventoryItem>(FakeStore));
    }

    public override IEnumerable<Event> Then()
    {
        yield return new InventoryItemDeactivated(_inventoryItemId);
    }

    public override Exception? ThenException()
    {
        return NoException();
    }
}