using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Icon.Infrastructure.Data.Converters;

public sealed class UlidToStringConverter : ValueConverter<Ulid, string>
{
    public UlidToStringConverter() : base(ulid => ulid.ToString(), str => Ulid.Parse(str)) { }
}
