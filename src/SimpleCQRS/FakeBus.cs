﻿using System;
using System.Collections.Generic;
using System.Threading;
// ReSharper disable InconsistentNaming

namespace SimpleCQRS
{
    public class FakeBus : ICommandSender, IEventPublisher
    {
        private readonly Dictionary<Type, List<Action<Message>>> _routes = new();

        public void RegisterHandler<T>(Action<T> handler) where T : Message
        {
            List<Action<Message>> handlers;

            if (!_routes.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Action<Message>>();
                _routes.Add(typeof(T), handlers);
            }

            if (typeof(T).IsAssignableTo(typeof(Command)) && handlers.Count > 0)
            {
                // Don't allow more than one command handler to be registered for each command.
                throw new InvalidOperationException("Only one handler per command is allowed.");
            }

            handlers.Add((x => handler((T)x)));
        }

        public void Send<T>(T command) where T : Command
        {
            List<Action<Message>> handlers;

            if (_routes.TryGetValue(typeof(T), out handlers))
            {
                if (handlers.Count != 1) throw new InvalidOperationException("cannot send to more than one handler");
                handlers[0](command);
            }
            else
            {
                throw new InvalidOperationException("no handler registered");
            }
        }

        public void Publish<T>(T @event) where T : Event
        {
            List<Action<Message>> handlers;

            if (!_routes.TryGetValue(@event.GetType(), out handlers)) return;

            foreach (var handler in handlers)
            {
                //dispatch on thread pool for added awesomeness
                var handlerClosure = handler;
                ThreadPool.QueueUserWorkItem(_ => handlerClosure(@event));
            }
        }
    }

    public interface Handles<T>
    {
        void Handle(T message);
    }

    public interface ICommandSender
    {
        void Send<T>(T command) where T : Command;
    }

    public interface IEventPublisher
    {
        void Publish<T>(T @event) where T : Event;
    }
}