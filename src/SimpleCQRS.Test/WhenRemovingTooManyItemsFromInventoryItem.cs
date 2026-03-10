using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleCQRS.Core;

namespace SimpleCQRS.Test
{
    [TestFixture]
    public class When_Removing_Too_Many_Items_From_An_Inventory_Item : EventSpecification<RemoveItemsFromInventory>
    {
        private readonly Guid _inventoryItemId = Guid.NewGuid();
        private readonly int _amountToCheckIn = 5;
        private readonly int _amountToCheckOut = 10;

        protected override IEnumerable<Event> Given()
        {
            yield return new InventoryItemCreated(_inventoryItemId, _inventoryItemId.ToString());
            yield return new ItemsCheckedInToInventory(_inventoryItemId, _amountToCheckIn);
        }

        protected override RemoveItemsFromInventory When()
        {
            return new RemoveItemsFromInventory(_inventoryItemId, _amountToCheckOut, 2);
        }

        protected override ICommandHandler<RemoveItemsFromInventory> BuildCommandHandler()
        {
            return new InventoryCommandHandlers(new Repository<InventoryItem>(FakeStore));
        }

        protected override IEnumerable<Event> Then()
        {
            return NoEvents();
        }

        protected override Exception? ThenException()
        {
            return new DomainException("Quantity cannot be negative");
        }
    }
}