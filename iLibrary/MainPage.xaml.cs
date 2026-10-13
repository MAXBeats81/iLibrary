using System.Globalization;
using CommunityToolkit.Maui.Media;
using iLibrary.Services;
using iLibrary.Models;

namespace iLibrary;

public partial class MainPage : ContentPage
{
    private readonly LibraryDataService dataService = new();
    private readonly VoiceCommandService voiceCommandService = new();
    private readonly BlockchainService blockchainService = new();
    private readonly LibraryDecisionEngine decisionEngine = new();

    private CancellationTokenSource? cancellationTokenSource;
    private readonly ISpeechToText speechToText = SpeechToText.Default;

    public MainPage()
    {
        InitializeComponent();
        NetworkLabel.Text = Connectivity.Current.NetworkAccess.ToString();
    }

    private async void RefreshData_Clicked(object? sender, EventArgs e)
    {
        try
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlertAsync("Connection Error", "Internet connection is unavailable.", "OK");
                return;
            }

            StatusLabel.Text = "Loading library data...";

            var data = await dataService.GetLibraryDataAsync();

            // Updated clean value styling
            SeatsLabel.Text = $"{data.AvailableSeats} available";
            BooksLabel.Text = $"{data.BorrowedBooks} books";
            ComputersLabel.Text = $"{data.AvailableComputers} available";
            VisitorsLabel.Text = $"{data.Visitors} visitors";

            var decision = decisionEngine.Evaluate(data);
            StatusLabel.Text = $"Status: {decision.Status} - {decision.Message}";

            blockchainService.AddTransaction("Library telemetry data refreshed");
            UpdateBlockchainDisplay();

            NetworkLabel.Text = Connectivity.Current.NetworkAccess.ToString();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private async void SpeakCommand_Clicked(object? sender, EventArgs e)
    {
        try
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            VoiceResultLabel.Text = "Listening... Speak now.";

            bool isGranted = await speechToText.RequestPermissions(cancellationTokenSource.Token);
            if (!isGranted)
            {
                VoiceResultLabel.Text = "Microphone or speech permission denied.";
                await DisplayAlertAsync("Permission", "Please allow speech and microphone permissions in device settings.", "OK");
                return;
            }

            var recognitionResult = await speechToText.ListenAsync(
                CultureInfo.CurrentCulture,
                new Progress<string>(partialText =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        VoiceResultLabel.Text = $"Listening: {partialText}";
                    });
                }),
                cancellationTokenSource.Token);

            recognitionResult.EnsureSuccess();

            string recognizedText = recognitionResult.Text;
            VoiceResultLabel.Text = $"Recognized: {recognizedText}";
            await ProcessVoiceCommand(recognizedText);
        }
        catch (Exception ex)
        {
            VoiceResultLabel.Text = "Speech recognition error.";
            await DisplayAlertAsync("Voice Error", ex.Message, "OK");
        }
    }

    private async Task ProcessVoiceCommand(string command)
    {
        string intent = voiceCommandService.ProcessCommand(command);
        switch (intent)
        {
            case "STATUS":
                ShowStatus_Clicked(this, EventArgs.Empty);
                break;
            case "SEATS":
                await DisplayAlertAsync("Seat Availability", SeatsLabel.Text, "OK");
                blockchainService.AddTransaction("Seat availability checked via voice");
                UpdateBlockchainDisplay();
                break;
            case "REFRESH":
                RefreshData_Clicked(this, EventArgs.Empty);
                break;
            case "EMERGENCY":
                Emergency_Clicked(this, EventArgs.Empty);
                break;
            default:
                await DisplayAlertAsync("Unknown Command", $"The app heard:\n\n'{command}'\n\nand did not recognize the action.", "OK");
                break;
        }
    }

    private async void ShowStatus_Clicked(object? sender, EventArgs e)
    {
        blockchainService.AddTransaction("Library status viewed");
        UpdateBlockchainDisplay();
        await DisplayAlertAsync("Library Status", $"{SeatsLabel.Text}\n{BooksLabel.Text}\n{ComputersLabel.Text}\n{VisitorsLabel.Text}\n\n{StatusLabel.Text}", "OK");
    }

    private async void Emergency_Clicked(object? sender, EventArgs e)
    {
        blockchainService.AddTransaction("Emergency mode activated");
        UpdateBlockchainDisplay();
        StatusLabel.Text = "STATUS: EMERGENCY PROTOCOL ACTIVE";
        await DisplayAlertAsync("Emergency Mode", "Emergency mode has been activated across the library system.", "OK");
    }

    private void UpdateBlockchainDisplay()
    {
        var latest = blockchainService.GetLatestBlock();
        BlockchainLabel.Text = $"Block #{latest.Index}\n" +
                               $"Transaction: {latest.Transaction}\n" +
                               $"Hash: {latest.Hash[..12]}...\n" +
                               $"Chain Valid: {blockchainService.VerifyChain()}";
    }
}