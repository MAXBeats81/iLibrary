using CommunityToolkit.Maui;

namespace iLibrary;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit(); // Registers Community Toolkit features

        return builder.Build();
    }
}