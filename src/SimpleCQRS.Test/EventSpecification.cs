using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleCQRS.Core;

// Assert.That(We.Understand) video: 32m18s

namespace SimpleCQRS.Test;

public abstract class EventSpecification<TCommand> where TCommand : Command
{
    protected Exception? caught;
    protected FakeEventStore FakeStore;
    public abstract IEnumerable<Event> Given();
    public abstract TCommand When();
    public abstract ICommandHandler<TCommand> OnHandler();
    public abstract IEnumerable<Event> Expect();

    [Test]
    public void RunSpecification()
    {
        caught = null;
        FakeStore = new FakeEventStore(Given());
        var handler = OnHandler();
        try
        {
            handler.Handle(When());
            var produced = FakeStore.PeekChanges().ToList();
            var expected = Expect().ToList();
            CompareEvents(expected, produced);
        }
        catch (Exception ex)
        {
            caught = ex;
        }
    }
    
    [TearDown]
    public void TearDown()
    {
        TestHelpers.PrintTest(this);
    }

    private void CompareEvents(IEnumerable<Event> expected, IEnumerable<Event> produced)
    {
        Assert.That(expected.Count(), Is.EqualTo(produced.Count()));
        TestHelpers.AssertAllPropertiesAreEqual(expected, produced);
    }
}