using Bogus;
using Icon.Application.Abstractions.Authentication;
using Icon.Application.Abstractions.Authentication.Models;
using Moq;

namespace Icon.Tests.Unit.Generators;

internal static class AuthenticationGenerator
{
    private static readonly Faker Faker = new();

    public static string UserId => Faker.Random.Guid().ToString();
    public static string Email => Faker.Internet.Email();
    public static string Password => Faker.Internet.Password(10, prefix: "A1!");
    public static string FirstName => Faker.Name.FirstName();
    public static string LastName => Faker.Name.LastName();
    public static string FullName => $"{FirstName} {LastName}";

    public static IdentityUserModel CreateIdentityUserModel(
        string? userId = null,
        string? email = null,
        string? fullName = null)
    {
        return new IdentityUserModel(
            userId ?? UserId,
            email ?? Email,
            fullName ?? FullName);
    }

    public static ITokenResult CreateTokenResult()
    {
        var mock = new Mock<ITokenResult>();
        mock.Setup(x => x.AccessToken).Returns(Faker.Random.AlphaNumeric(64));
        mock.Setup(x => x.RefreshToken).Returns(Faker.Random.AlphaNumeric(64));
        mock.Setup(x => x.AccessTokenExpiration).Returns(DateTime.UtcNow.AddHours(1));
        mock.Setup(x => x.RefreshTokenExpiration).Returns(DateTime.UtcNow.AddDays(7));
        return mock.Object;
    }
}
