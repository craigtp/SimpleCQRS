using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleCQRS.Core;

namespace SimpleCQRS.Test;

[TestFixture]
public class When_Deactivating_An_Item_That_Does_Not_Exist : EventSpecification<DeactivateInventoryItem>
{
    private readonly Guid _inventoryItemId = Guid.NewGuid();

    public override IEnumerable<Event> Given()
    {
        return NoEvents();
    }

    public override DeactivateInventoryItem When()
    {
        return new DeactivateInventoryItem(_inventoryItemId, 0);
    }

    protected override ICommandHandler<DeactivateInventoryItem> BuildCommandHandler()
    {
        return new InventoryCommandHandlers(new Repository<InventoryItem>(FakeStore));
    }

    public override IEnumerable<Event> Then()
    {
        return NoEvents();
    }
    
    public override Exception? ThrownException()
    {
        return new AggregateNotFoundException();
    }

    // public override Expression<Predicate<Exception>> ThenException()
    // {
    //     return exception => exception.GetType() == typeof (InvalidOperationException) &&
    //                         exception.Message == "already deactivated";
    // }
}