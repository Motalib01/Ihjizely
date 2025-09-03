using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.SavedProperties;

public static class SavedPropertyErrors
{
    public static readonly Error NotFound = new(
        "SavedProperty.NotFound",
        "The saved property could not be found.");

    public static readonly Error AlreadySaved = new(
        "SavedProperty.AlreadySaved",
        "This property is already saved by the user.");

    public static Error Unauthorized = new(
        "SavedProperty.Unauthorized",
        "You are not authorized to delete this saved property.");
}