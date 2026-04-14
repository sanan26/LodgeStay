using Microsoft.Extensions.Logging;
using LodgeStay.Data;
using LodgeStay.Services;

namespace LodgeStay;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "lodgestay.db");
        builder.Services.AddSingleton<DatabaseContext>(s =>
        {
            var context = new DatabaseContext(dbPath);
            Task.Run(async () => await context.InitializeAsync())
                .GetAwaiter()
                .GetResult();
            return context;
        });

        // Sprint 1 Services
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<OtpService>();
        builder.Services.AddSingleton<SessionService>();
        builder.Services.AddSingleton<RoomService>();
        builder.Services.AddSingleton<ReservationService>();
        builder.Services.AddSingleton<ConnectivityService>();
        builder.Services.AddSingleton<CalendarExportService>();
        builder.Services.AddSingleton<ShareService>();

        // Sprint 2 Services
        builder.Services.AddSingleton<GuestService>();
        builder.Services.AddSingleton<LoyaltyService>();
        builder.Services.AddSingleton<PreferenceService>();
        builder.Services.AddSingleton<GroupBookingService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
