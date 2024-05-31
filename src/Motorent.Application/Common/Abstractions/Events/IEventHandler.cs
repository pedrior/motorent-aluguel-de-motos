using Motorent.Domain.Common;

namespace Motorent.Application.Common.Abstractions.Events;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent;