using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCQRS.Core;

namespace SimpleCQRS.Test;

public class FakeEventStore : IEventStore
{
    private readonly Dictionary<Guid, List<EventDescriptor>> _events = new();
    
    private struct EventDescriptor
    {
        public readonly Event EventData;
        public readonly Guid Id;
        public readonly int Version;
        public readonly bool IsSeededEvent;

        public EventDescriptor(Guid id, Event eventData, int version, bool isSeededEvent)
        {
            EventData = eventData;
            Version = version;
            Id = id;
            IsSeededEvent = isSeededEvent;
        }
    }

    public FakeEventStore(IEnumerable<Event> events)
    {
        var eventArray = events as Event[] ?? events.ToArray();
        var guid = eventArray.Any() ? eventArray.First().Id : Guid.Empty;
        var eventDescriptors = new List<EventDescriptor>();
        var i = 0;
        foreach (var @event in eventArray)
        {
            i++;
            @event.Version = i;
            eventDescriptors.Add(new EventDescriptor(guid, @event, i, true));
        }
        _events.Add(guid, eventDescriptors);
    }

    public void SaveEvents(Guid aggregateId, IEnumerable<Event> events, int expectedVersion)
    {
        // try to get event descriptors list for given aggregate id
        // otherwise -> create empty dictionary
        if (!_events.TryGetValue(aggregateId, out var eventDescriptors))
        {
            eventDescriptors = new List<EventDescriptor>();
            _events.Add(aggregateId, eventDescriptors);
        }
        // check whether latest event version matches current aggregate version
        // otherwise -> throw exception
        else if (eventDescriptors[eventDescriptors.Count - 1].Version != expectedVersion && expectedVersion != -1)
        {
            throw new ConcurrencyException();
        }

        var i = expectedVersion;

        // iterate through current aggregate events increasing version with each processed event
        foreach (var @event in events)
        {
            i++;
            @event.Version = i;

            // push event to the event descriptors list for current aggregate
            eventDescriptors.Add(new EventDescriptor(aggregateId, @event, i, false));
        }
    }

    // collect all processed events for given aggregate and return them as a list
    // used to build up an aggregate from its history (Domain.LoadFromHistory)
    public IEnumerable<Event> GetEventsForAggregate(Guid aggregateId)
    {
        if (!_events.TryGetValue(aggregateId, out var eventDescriptors))
        {
            throw new AggregateNotFoundException();
        }

        return eventDescriptors.Select(desc => desc.EventData).ToList();
    }

    public IEnumerable<Event> PeekChanges()
    {
        return _events
            .SelectMany(x => x.Value)
            .Where(y => y.IsSeededEvent == false)
            .Select(z => z.EventData)
            .ToList();
    }
}