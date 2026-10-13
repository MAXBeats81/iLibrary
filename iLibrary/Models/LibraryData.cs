namespace iLibrary.Models;

public class LibraryData
{
    public int AvailableSeats { get; set; }
    public int BorrowedBooks { get; set; }
    public int AvailableComputers { get; set; }
    public int Visitors { get; set; }
    public string LibraryStatus { get; set; } = "OPEN";
    public DateTime Timestamp { get; set; }
}