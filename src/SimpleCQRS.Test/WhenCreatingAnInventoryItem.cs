using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

    public override ICommandHandler<CreateInventoryItem> BuildCommandHandler()
    {
        return new InventoryCommandHandlers(new Repository<InventoryItem>(FakeStore));
    }

    public override IEnumerable<Event> Then()
    {
        yield return new InventoryItemCreated(_inventoryItemId, _inventoryItemId.ToString());
        //yield return new InventoryItemCreated(_inventoryItemId, "This breaks the test!");
    }

    public override Expression<Predicate<Exception>> ThenException()
    {
        return NoException();
    }
}