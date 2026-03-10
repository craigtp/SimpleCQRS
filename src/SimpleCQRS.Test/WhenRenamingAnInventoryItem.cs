using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleCQRS.Core;

namespace SimpleCQRS.Test
{
    [TestFixture]
    public class When_Renaming_An_Inventory_Item : EventSpecification<RenameInventoryItem>
    {
        private readonly Guid _inventoryItemId = Guid.NewGuid();
        private readonly string _originalName = "Original Name";
        private readonly string _newName = "New Name";

        protected override IEnumerable<Event> Given()
        {
            yield return new InventoryItemCreated(_inventoryItemId, _originalName);
        }

        protected override RenameInventoryItem When()
        {
            return new RenameInventoryItem(_inventoryItemId, _newName, 1);
        }

        protected override ICommandHandler<RenameInventoryItem> BuildCommandHandler()
        {
            return new InventoryCommandHandlers(new Repository<InventoryItem>(FakeStore));
        }

        protected override IEnumerable<Event> Then()
        {
            yield return new InventoryItemRenamed(_inventoryItemId, _newName);
        }

        protected override Exception? ThenException()
        {
            return NoException();
        }
    }
}