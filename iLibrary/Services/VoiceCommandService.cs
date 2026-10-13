namespace iLibrary.Services;

public class VoiceCommandService
{
    public string ProcessCommand(string command)
    {
        command = command.ToLower().Trim();

        if (command.Contains("status") || command.Contains("library status"))
        {
            return "STATUS";
        }
        if (command.Contains("seats") || command.Contains("available seats"))
        {
            return "SEATS";
        }
        if (command.Contains("refresh"))
        {
            return "REFRESH";
        }
        if (command.Contains("emergency"))
        {
            return "EMERGENCY";
        }

        return "UNKNOWN";
    }
}