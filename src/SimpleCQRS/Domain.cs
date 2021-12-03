using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCQRS
{
    public class InventoryItem : AggregateRoot
    {
        private Guid _id;
        private bool _activated;
        private string _name;
        private int _quantity;

        protected override void Apply(Event @event)
        {
            switch (@event)
            {
                case InventoryItemCreated e:
                    _id = e.Id;
                    _name = e.Name;
                    _activated = true;
                    break;
                case InventoryItemDeactivated e:
                    _activated = false;
                    break;
                case InventoryItemRenamed e:
                    _name = e.NewName;
                    break;
                case ItemsCheckedInToInventory e:
                    _quantity += e.Count;
                    break;
                case ItemsRemovedFromInventory e:
                    _quantity -= e.Count;
                    break;
            }
        }

        protected override void EnsureValid()
        {
            // All aggregate business logic and validation of invariants lives in this method.  
            var idValid = Id != Guid.Empty;
            var nameValid = !string.IsNullOrEmpty(_name);
            var quantityValid = _quantity >= 0;
            var valid = idValid && nameValid && quantityValid;

            if (!valid)
            {
                var message = string.Join(", ", new[]
                {
                    idValid ? string.Empty : "Id cannot be an empty Guid",
                    nameValid ? string.Empty : "Name cannot be blank",
                    quantityValid ? string.Empty : "Quantity cannot be negative"
                }.Where(s => !string.IsNullOrEmpty(s)));
                throw new InvalidEntityStateException(this, message);
            }
        }

        public void ChangeName(string newName)
        {
            if (string.IsNullOrEmpty(newName)) throw new ArgumentException("newName");
            ApplyChange(new InventoryItemRenamed(_id, newName));
        }

        public void Remove(int count)
        {
            if (count <= 0) throw new InvalidOperationException("cant remove negative count from inventory");
            ApplyChange(new ItemsRemovedFromInventory(_id, count));
        }

        public void CheckIn(int count)
        {
            if (count <= 0) throw new InvalidOperationException("must have a count greater than 0 to add to inventory");
            ApplyChange(new ItemsCheckedInToInventory(_id, count));
        }

        public void Deactivate()
        {
            if (!_activated) throw new InvalidOperationException("already deactivated");
            ApplyChange(new InventoryItemDeactivated(_id));
        }

        public override Guid Id
        {
            get { return _id; }
        }
        
        public InventoryItem()
        {
            // used to create in repository ... many ways to avoid this, eg making private constructor
        }

        public InventoryItem(Guid id, string name)
        {
            ApplyChange(new InventoryItemCreated(id, name));
        }
    }

    public abstract class AggregateRoot
    {
        private readonly List<Event> _changes = new List<Event>();

        public abstract Guid Id { get; }
        public int Version { get; internal set; }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void LoadFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history)
            {
                ApplyChange(e, false);
                Version++;
            }
        }

        protected abstract void Apply(Event @event);

        protected abstract void EnsureValid();

        protected void ApplyChange(Event @event)
        {
            ApplyChange(@event, true);
        }

        // push atomic aggregate changes to local history for further processing (EventStore.SaveEvents)
        private void ApplyChange(Event @event, bool isNew)
        {
            Apply(@event);
            EnsureValid();
            if (isNew) _changes.Add(@event);
        }
    }

    public interface IRepository<T> where T : AggregateRoot, new()
    {
        void Save(AggregateRoot aggregate, int expectedVersion);
        T GetById(Guid id);
    }

    public class Repository<T> : IRepository<T> where T : AggregateRoot, new() //shortcut you can do as you see fit with new()
    {
        private readonly IEventStore _storage;

        public Repository(IEventStore storage)
        {
            _storage = storage;
        }

        public void Save(AggregateRoot aggregate, int expectedVersion)
        {
            _storage.SaveEvents(aggregate.Id, aggregate.GetUncommittedChanges(), expectedVersion);
            aggregate.MarkChangesAsCommitted();
        }

        public T GetById(Guid id)
        {
            var obj = new T();//lots of ways to do this
            var e = _storage.GetEventsForAggregate(id);
            obj.LoadFromHistory(e);
            return obj;
        }
    }

    public class InvalidEntityStateException : Exception
    {
        public InvalidEntityStateException(object entity, string message)
            : base(
                $"Entity {entity.GetType().Name} state change rejected, {message}"
            )
        { }
    }
}
