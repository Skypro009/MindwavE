using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

namespace MindwavE
{
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("NunitoSans.ttf", "NunitoSans");
                    fonts.AddFont("NunitoSansItalic.ttf", "NunitoSans_Italic");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            
            // Services
            builder.Services.AddSingleton<Services.IAiService, Services.AiService>();
            
            // Pages
            builder.Services.AddTransient<ChatPage>();
            builder.Services.AddSingleton<HomePage>(); // Usually Singleton for main tabs

            return builder.Build();
        }
    }
}
