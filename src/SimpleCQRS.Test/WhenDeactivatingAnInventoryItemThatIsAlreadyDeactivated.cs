﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleCQRS.Core;

namespace SimpleCQRS.Test
{
    [TestFixture]
    public class When_Deactivating_An_Inventory_Item_That_Is_Already_Deactivated : EventSpecification<DeactivateInventoryItem>
    {
        private readonly Guid _inventoryItemId = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            yield return new InventoryItemCreated(_inventoryItemId, _inventoryItemId.ToString());
            yield return new InventoryItemDeactivated(_inventoryItemId);
        }

        protected override DeactivateInventoryItem When()
        {
            return new DeactivateInventoryItem(_inventoryItemId, 0);
        }

        protected override ICommandHandler<DeactivateInventoryItem> BuildCommandHandler()
        {
            return new InventoryCommandHandlers(new Repository<InventoryItem>(FakeStore));
        }

        protected override IEnumerable<Event> Then()
        {
            return NoEvents();
        }

        protected override Exception ThenException()
        {
            return new InvalidOperationException("already deactivated");
        }
    }
}