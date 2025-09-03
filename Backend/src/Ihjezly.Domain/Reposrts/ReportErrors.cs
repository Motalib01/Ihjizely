using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.NewFolder;

public static class ReportErrors
{
    public static readonly Error NotFound = new(
        "Report.NotFound",
        "The specified report could not be found."
    );
}