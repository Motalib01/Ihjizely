using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Properties;

public static class PropertyErrors
{

    public static readonly Error NotFound = new(
        "Property.NotFound",
        "The property with the specified identifier was not found.");

    public static readonly Error InvalidType = new(
        "Property.InvalidType",
        "The specified property type is invalid.");

    public static readonly Error MissingDetails = new(
        "Property.MissingDetails",
        "The property details are required for this property type.");

    public static readonly Error AlreadyDeleted = new(
        "Property.AlreadyDeleted",
        "The property has already been deleted.");

    public static readonly Error InvalidDiscount = new(
        "Property.InvalidDiscount",
        "The discount specified for the property is invalid.");

    public static readonly Error InvalidLocation = new(
        "Property.InvalidLocation",
        "The provided property location is invalid.");

    public static readonly Error InvalidFacilities = new(
        "Property.InvalidFacilities",
        "One or more provided facilities are invalid.");


    public static readonly Error DescriptionRequired = new(
        "Property.DescriptionRequired",
        "A description is required for the property.");

    public static readonly Error TitleRequired = new(
         "Property.TitleRequired",
         "Title is required.");

    public static readonly Error InvalidCurrency = new(
         "Property.InvalidCurrency",
         "Currency is not valid.");

    public static readonly Error InvalidImage = new(
         "Property.InvalidImage",
         "One or more image URLs are invalid.");

    public static readonly Error CannotPostAd = new(
         "Property.CannotPostAd",
         "You must have an active subscription to post an ad.");

    public static readonly Error AdQuotaExceeded = new(
        "Property.AdQuotaExceeded",
        "You have used all of your ad quota for the current subscription.");


}