using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleCQRS.Core;

// Assert.That(We.Understand) video: 32m18s

namespace SimpleCQRS.Test;

public abstract class EventSpecification<TCommand> where TCommand : Command
{
    protected FakeEventStore? FakeStore;
    private Exception? _caughtException;
    public abstract IEnumerable<Event> Given();
    public abstract TCommand When();
    protected abstract ICommandHandler<TCommand> BuildCommandHandler();
    public abstract IEnumerable<Event> Then();
    public abstract Exception? ThenException();

    [Test]
    public void RunSpecification()
    {
        FakeStore = new FakeEventStore(Given());
        var handler = BuildCommandHandler();
        var expected = Then().ToList();
        var produced = new List<Event>();
        
        try
        {
            handler.Handle(When());
            produced = FakeStore.PeekChanges().ToList();
        }
        catch (Exception? exception)
        {
            _caughtException = exception;
        }
        
        CompareEvents(expected, produced);

        var expectedException = ThenException();
        if (expectedException == null) return;
        if (expectedException.GetType() != _caughtException?.GetType() ||
            expectedException.Message != _caughtException.Message)
        {
            Assert.Fail();
        }

    }

    protected IEnumerable<Event> NoEvents()
    {
        return Enumerable.Empty<Event>();
    }

    protected Exception? NoException()
    {
        return null;
    }
    
    [TearDown]
    public void TearDown()
    {
        TestHelpers.PrintTest(this);
    }

    private static void CompareEvents(IEnumerable<Event> expected, IEnumerable<Event> produced)
    {
        Assert.That(expected.Count(), Is.EqualTo(produced.Count()));
        Assert.True(TestHelpers.DeepEquals(expected,produced, new List<string>{"Version"}));
    }
}