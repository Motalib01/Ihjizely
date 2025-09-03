using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Booking;

public class PricingService
{
    public static Money CalculateTotalPrice(
        Property property,
        DateTime startDate,
        DateTime endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("End date must be after start date.");

        var numberOfDays = (endDate - startDate).Days;
        if (numberOfDays == 0)
            numberOfDays = 1; // at least one day booking

        var dailyAmount = property.Price.Amount;

        // 🔹 Apply property discount if it exists and > 0
        if (property.Discount is not null && property.Discount.Value > 0)
        {
            var discountRate = property.Discount.Value / 100m; // e.g. 20 => 20%
            dailyAmount -= dailyAmount * discountRate;
        }

        var totalAmount = dailyAmount * numberOfDays;

        return new Money(totalAmount, property.Price.Currency);
    }
}