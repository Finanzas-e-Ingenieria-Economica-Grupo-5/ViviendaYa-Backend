using Cortex.Mediator.Notifications;
using VIviendaYa.API.Shared.Domain.Model.Events;

namespace VIviendaYa.API.Shared.Application.Internal.EventHandlers;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
{
}