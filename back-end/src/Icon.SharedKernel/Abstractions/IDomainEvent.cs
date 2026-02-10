using MediatR;

namespace Icon.SharedKernel.Abstractions;

/// <summary>
/// Marker for something notable that happened in the domain (e.g. "ticket was completed").
/// Handlers can react to these events without the raiser knowing about them.
/// </summary>
public interface IDomainEvent : INotification;
