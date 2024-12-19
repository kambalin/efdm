using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace EFDM.DAL.Converters;

public class DateTimeOffsetConverterUtc : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public DateTimeOffsetConverterUtc()
        : base(
            d => d.ToUniversalTime(),
            d => d.ToUniversalTime())
    {
    }
}
