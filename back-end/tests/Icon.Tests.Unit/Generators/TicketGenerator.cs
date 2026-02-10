using Bogus;
using Icon.Domain.Entities;
using Icon.Domain.Enums;
using Icon.Domain.ValueObjects;

namespace Icon.Tests.Unit.Generators;

internal static class TicketGenerator
{
    private static readonly Faker Faker = new();

    public static string UserId => Faker.Random.Guid().ToString();
    public static string Title => Faker.Lorem.Sentence(3);
    public static string Description => Faker.Lorem.Paragraph();

    public static Ticket CreateTicket(string? userId = null)
    {
        return Ticket.Create(
            userId ?? UserId,
            TicketTitle.Create(Title),
            TicketDescription.Create(Description),
            Faker.PickRandom<TicketPriority>(),
            Faker.Date.Future());
    }

    public static Ticket CreateCompletedTicket(string? userId = null)
    {
        var ticket = CreateTicket(userId);
        ticket.MarkAsCompleted();
        return ticket;
    }

    public static List<Ticket> CreateTickets(int count, string? userId = null)
    {
        var resolvedUserId = userId ?? UserId;
        return Enumerable.Range(0, count)
            .Select(_ => CreateTicket(resolvedUserId))
            .ToList();
    }
}
