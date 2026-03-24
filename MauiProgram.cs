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
				.GetResult(); // Ensure the database is initialized before returning the context
			return context;
		});

		builder.Services.AddSingleton<AuthService>();
		builder.Services.AddSingleton<OtpService>();
		builder.Services.AddSingleton<SessionService>();
		builder.Services.AddSingleton<RoomService>();
		builder.Services.AddSingleton<ReservationService>();
		builder.Services.AddSingleton<ConnectivityService>();
		builder.Services.Add

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
