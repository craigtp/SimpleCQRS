﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleCQRS.Core;

namespace SimpleCQRS.Test;

[TestFixture]
public class When_Deactivating_An_Inventory_Item_That_Is_Already_Deactivated : EventSpecification<DeactivateInventoryItem>
{
    private readonly Guid _inventoryItemId = Guid.NewGuid();
    public override IEnumerable<Event> Given()
    {
        yield return new InventoryItemCreated(_inventoryItemId, _inventoryItemId.ToString());
        yield return new InventoryItemDeactivated(_inventoryItemId);
    }

    public override DeactivateInventoryItem When()
    {
        return new DeactivateInventoryItem(_inventoryItemId, 0);
    }

    public override ICommandHandler<DeactivateInventoryItem> OnHandler()
    {
        return new InventoryCommandHandlers(new Repository<InventoryItem>(FakeStore));
    }

    public override IEnumerable<Event> Expect()
    {
        throw new Exception();
    }
}