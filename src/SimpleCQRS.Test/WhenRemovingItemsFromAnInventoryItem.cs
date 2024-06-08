using System;
using System.Collections.Generic;
using SimpleCQRS.Core;

namespace SimpleCQRS.Test
{
    public class When_Removing_Items_From_An_Inventory_Item : EventSpecification<RemoveItemsFromInventory>
    {
        private readonly Guid _inventoryItemId = Guid.NewGuid();
        private readonly int _amountToCheckIn = 10;
        private readonly int _amountToCheckOut = 5;

        public override IEnumerable<Event> Given()
        {
            yield return new InventoryItemCreated(_inventoryItemId, _inventoryItemId.ToString());
            yield return new ItemsCheckedInToInventory(_inventoryItemId, _amountToCheckIn);
        }

        public override RemoveItemsFromInventory When()
        {
            return new RemoveItemsFromInventory(_inventoryItemId, _amountToCheckOut, 2);
        }

        protected override ICommandHandler<RemoveItemsFromInventory> BuildCommandHandler()
        {
            return new InventoryCommandHandlers(new Repository<InventoryItem>(FakeStore));
        }

        public override IEnumerable<Event> Then()
        {
            yield return new ItemsRemovedFromInventory(_inventoryItemId, _amountToCheckOut);
        }

        public override Exception? ThenException()
        {
            return NoException();
        }    
    }
}