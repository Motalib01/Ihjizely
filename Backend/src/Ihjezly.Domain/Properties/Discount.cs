using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Domain.Properties;


[Owned]
public sealed record Discount(decimal Value);