using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleCQRS.Core;

namespace SimpleCQRS.Test
{
    [TestFixture]
    public class When_Creating_An_Inventory_Item : EventSpecification<CreateInventoryItem>
    {
        private readonly Guid _inventoryItemId = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            return NoEvents();
        }

        protected override CreateInventoryItem When()
        {
            return new CreateInventoryItem(_inventoryItemId, _inventoryItemId.ToString());
        }

        protected override ICommandHandler<CreateInventoryItem> BuildCommandHandler()
        {
            return new InventoryCommandHandlers(new Repository<InventoryItem>(FakeStore));
        }

        protected override IEnumerable<Event> Then()
        {
            yield return new InventoryItemCreated(_inventoryItemId, _inventoryItemId.ToString());
            //yield return new InventoryItemCreated(Guid.NewGuid(), "This breaks the test!");
        }

        protected override Exception? ThenException()
        {
            return NoException();
        }
    }
}