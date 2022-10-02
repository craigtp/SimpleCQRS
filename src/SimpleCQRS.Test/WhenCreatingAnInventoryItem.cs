using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleCQRS.Core;

namespace SimpleCQRS.Test;

[TestFixture]
public class When_Creating_An_Inventory_Item : EventSpecification<CreateInventoryItem>
{
    private readonly Guid _inventoryItemId = Guid.NewGuid();
    public override IEnumerable<Event> Given()
    {
        yield break;
    }

    public override CreateInventoryItem When()
    {
        return new CreateInventoryItem(_inventoryItemId, _inventoryItemId.ToString());
    }

    public override ICommandHandler<CreateInventoryItem> OnHandler()
    {
        return new InventoryCommandHandlers(new Repository<InventoryItem>(FakeStore));
    }

    public override IEnumerable<Event> Expect()
    {
        yield return new InventoryItemCreated(_inventoryItemId, _inventoryItemId.ToString());
    }
}