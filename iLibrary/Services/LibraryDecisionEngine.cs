using iLibrary.Models;

namespace iLibrary.Services;

public class DecisionResult
{
    public string Status { get; set; } = "NORMAL";
    public string Message { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}

public class LibraryDecisionEngine
{
    public DecisionResult Evaluate(LibraryData data)
    {
        if (data.LibraryStatus == "EMERGENCY")
        {
            return new DecisionResult
            {
                Status = "CRITICAL",
                Message = "Emergency mode is active.",
                Action = "Lockdown Protocols"
            };
        }

        if (data.AvailableSeats == 0)
        {
            return new DecisionResult
            {
                Status = "FULL",
                Message = "Library seats are fully occupied.",
                Action = "Display Full Warning"
            };
        }

        if (data.AvailableSeats <= 5)
        {
            return new DecisionResult
            {
                Status = "WARNING",
                Message = "Seats running low.",
                Action = "Display Low Seat Alert"
            };
        }

        return new DecisionResult
        {
            Status = "AVAILABLE",
            Message = "Library operations normal.",
            Action = "None"
        };
    }
}