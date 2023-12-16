using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using SimpleCQRS.Core;

// Assert.That(We.Understand) video: 32m18s

namespace SimpleCQRS.Test;

public abstract class EventSpecification<TCommand> where TCommand : Command
{
    protected FakeEventStore? FakeStore;
    public abstract IEnumerable<Event> Given();
    public abstract TCommand When();
    public abstract ICommandHandler<TCommand> BuildCommandHandler();
    public abstract IEnumerable<Event> Then();
    public abstract Expression<Predicate<Exception>> ThenException();

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
        catch (Exception exception)
        {
            if (ThenException().Compile()(exception))
            {
                Assert.Pass();
            }

            var exceptionType = ThenException().Parameters[0].Type;
            Assert.Fail($"Received an exception ({exceptionType}) that failed the provided predicate.");
        }
        
        CompareEvents(expected, produced);
    }

    public IEnumerable<Event> NoEvents()
    {
        return Enumerable.Empty<Event>();
    }

    public Expression<Predicate<Exception>> NoException()
    {
        return exception => false;
    }
    
    [TearDown]
    public void TearDown()
    {
        TestHelpers.PrintTest(this);
    }

    private void CompareEvents(IEnumerable<Event> expected, IEnumerable<Event> produced)
    {
        Assert.That(expected.Count(), Is.EqualTo(produced.Count()));
        Assert.True(TestHelpers.DeepEquals(expected,produced, new List<string>{"Version"}));
    }
}