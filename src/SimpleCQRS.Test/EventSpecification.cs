using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimpleCQRS.Core;

// Assert.That(We.Understand) video: 32m18s
// See also: https://github.com/rmacdonaldsmith/CQRS-MassTransit/blob/7a4649ccf77eb149a79a73b74bbd8795ad2acc6a/MassTransit/CQRS.DomainTesting/FakeEventStore.cs

namespace SimpleCQRS.Test;

public abstract class EventSpecification<TCommand> where TCommand : Command
{
    protected FakeEventStore? FakeStore;
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
        var actual = new List<Event>();
        
        try
        {
            handler.Handle(When());
            actual = FakeStore.PeekChanges().ToList();
        }
        catch (Exception exception)
        {
            var expectedException = ThenException();
            if (expectedException == null ||
                expectedException.GetType() != exception.GetType() ||
                expectedException.Message != exception.Message)
            {
                Assert.Fail(GetTestResultText(false, $"Received an exception ({exception.GetType()}) that was not expected."));
            }
            Assert.Pass(GetTestResultText(true));
        }

        var compareResult = new ObjectComparer().Compare(expected, actual, ["Version"]);

        if (compareResult.Success)
        {
            Assert.Pass(GetTestResultText(true));
        }
        else
        {
            Assert.Fail(GetTestResultText(false, $"Actual events do not match expected events: \n{compareResult.Differences}"));
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
    
    private string GetTestResultText(bool success, string failureText = null)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Specification: " + GetType().Name.Replace("_", " "));
        sb.AppendLine();
        sb.AppendLine("Given:");
        var existingEvents = Given().ToList();
        if (!existingEvents.Any())
        {
            sb.AppendLine("\t" + "[No existing events]");
        }
        else
        {
            var firstEvent = true;
            foreach (var @event in existingEvents)
            {
                sb.AppendLine("\t" + (firstEvent ? string.Empty : "and ") + @event);
                firstEvent = false;
            }
        }

        sb.AppendLine();
        sb.AppendLine("When:");
        sb.AppendLine("\t" + When());
        sb.AppendLine();
        sb.AppendLine("Expect:");

        var expected = Then().ToList();
        if (!expected.Any())
        {
            sb.AppendLine("\t" + "[No events]");
        }
        else
        {
            var firstEvent = true;
            foreach (var @event in expected)
            {
                sb.AppendLine("\t" + (firstEvent ? string.Empty : "and ") + @event);
                firstEvent = false;
            }
        }

        var expectedException = ThenException();
        if (expectedException != null)
        {
            sb.AppendLine("\t" + $"and an Exception of type {expectedException.GetType()} is thrown.");
        }

        sb.AppendLine();
        sb.AppendLine("Result:");
        var resultText = success ? "PASSED" : "FAILED";
        sb.AppendLine($"\t[{resultText}]");
        if (!success)
        {
            sb.AppendLine();
            sb.AppendLine($"{failureText}");
        }

        sb.AppendLine();

        return sb.ToString();
    }
}