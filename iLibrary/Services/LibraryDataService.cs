using iLibrary.Models;

namespace iLibrary.Services;

public class LibraryDataService
{
    private readonly Random random = new();

    public async Task<LibraryData> GetLibraryDataAsync()
    {
        // Simulate network latency (remote API / IoT simulation)
        await Task.Delay(1000);

        return new LibraryData
        {
            AvailableSeats = random.Next(0, 45),
            BorrowedBooks = random.Next(20, 150),
            AvailableComputers = random.Next(0, 20),
            Visitors = random.Next(10, 60),
            LibraryStatus = "OPEN",
            Timestamp = DateTime.Now
        };
    }
}